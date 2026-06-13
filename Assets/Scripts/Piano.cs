using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Minis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Piano : MonoBehaviour
{
    // Settings
    
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private KeyCode[] keys;
    [SerializeField] private int[] midiNumbers;
    [SerializeField] private KeyCode octaveKey = KeyCode.LeftShift;
    [SerializeField] private AudioClip pianoSound;
    [SerializeField] private float volume = 5f;
    [SerializeField] private float sequenceTime = 1f;
    [SerializeField] private float playSequenceTime = 0.2f;
    public readonly float PlaySequenceStartTime = 1f;
    
    // References
    private Image _image;
    private RectTransform _rt;

    // Static
    private const int AudioMidiNumber = 66; //(SHOULD BE ADJUSTED TO THE PIANOSOUNDS MIDI NUMBER!)

    // Private variables
    private float _baseFrequency = 0f; // The frequency of the pianoSound, depends on _audioMidiNumber
    private float _sequenceTimer = 0f;
    private List<int> _sequence = new List<int>(); // Sequence recording
    private MidiDevice _currentDevice;
    private bool _playingSequence = false;
    private Vector3 _startLocalPos;
    private GameManager _gm;

    private void Awake()
    {
        // Setup
        _image = GetComponent<Image>();
        _baseFrequency = GetFrequency(AudioMidiNumber);
        
        _startLocalPos = _startLocalPos = transform.localPosition; 
        _gm = GameManager.Instance;
    }

    private void Start()
    {
        _gm.piano ??= this;
    }

    private void Update()
    {
        UpdateCurrentDevice();
        if (!(_gm.overlayEnabled) && _gm.pianoKeys.Any(Input.GetKeyDown))
        {
            _gm.pianoEnabled = !_gm.pianoEnabled;
            ClearSequence();
        }

        if (_currentDevice is not null)
        {
            _gm.pianoEnabled = false; // Skip keyboard input if midi device is attached
            _gm.MIDIAttached = true;
        }
        else _gm.MIDIAttached = false;
        
        if (_gm.pianoEnabled) SetSprite();
        
        _sequenceTimer -= Time.deltaTime;
        if (_sequenceTimer <= 0f) _sequence.Clear();
        
        CameraController camControl = _gm.camcontrol;
        transform.localPosition = _startLocalPos;
        if (camControl) transform.position += new Vector3(0f, camControl.CamOffsetY, 0f); 
    }
    
    private void UpdateCurrentDevice()
    {
        if (_currentDevice == MidiDevice.current) return;
        if (_currentDevice is not null) _currentDevice.onWillNoteOn -= OnNoteOn;
        _currentDevice = MidiDevice.current;
        if (_currentDevice is not null) _currentDevice.onWillNoteOn += OnNoteOn;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void SetSprite()
    {
        if (Input.anyKey)
        {
            for (var i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
            
                if (!Input.GetKey(key)) continue;
                var octave = Input.GetKey(octaveKey);
                var num = i + 1 + (octave ? 12 : 0); // Number in the arrays of the pressed key

                if (num >= sprites.Length) continue;
                // Set sprite and play sound
                _image.sprite = sprites[num];
            
                if (!Input.GetKeyDown(key)) return;
                PlayClip(num - 1);
                _sequence.Add(num - 1);
            
                _sequenceTimer = sequenceTime;
                return; // Ensure no unnecessary checks are done
            }
        }

        if (!_playingSequence) _image.sprite = sprites[0]; // Reset to default if no keys are pressed
    }

    private void OnNoteOn(MidiNoteControl note, float velocity)
    {
        // Handle midi input
        if (!midiNumbers.Contains(note.noteNumber)) return; // Skip if the note is not registered
        for (var i = 0; i < midiNumbers.Length; i++)
        {
            if (note.noteNumber != midiNumbers[i]) continue;
            
            // i = now the number of the pressed key in the lists
            _image.sprite = sprites[i + 1];
            PlayClip(i);
            _sequence.Add(i);
            _sequenceTimer = sequenceTime;
            return; // Ensure no unnecessary checks are done
        }
    }
    
    private void PlayClip(int pressedKey)
    {
        var targetFrequency = GetFrequency(midiNumbers[pressedKey]);
        var pitch = targetFrequency / _baseFrequency;
        AudioManager.Instance.StartClip(pianoSound, volume, pitch);
    }

    private float GetFrequency(int midiNumber)
    {
        // Formula for converting midi numbers to pitch
        var midiPower = (midiNumber - 69f) / 12f;
        var frequency = 440f * Mathf.Pow(2f, midiPower);
        return frequency;
    }

    public void ClearSequence()
    {
        _sequence.Clear();
    }
    
    public bool CheckSequence(List<int> referenceSequence)
    {
        if (_sequence.Count < referenceSequence.Count) return false; // If the sequence is shorter than the reference, it is definitely wrong, so we can skip the check
        
        // Put the last [reference length] numbers of the sequence in a list, only the last part is checked
        var lastSequence = new List<int>();
        for (var i = referenceSequence.Count; i > 0; i--)
        {
            lastSequence.Add(_sequence[^i]);
        }
        return referenceSequence.SequenceEqual(lastSequence);
    }

    public bool CheckSequenceLength(List<int> referenceSequence)
    {
        return _sequence.Count >= referenceSequence.Count;
    }

    public void PlaySequence(List<int> sequence)
    {
        StartCoroutine(PlaySequenceRoutine(sequence));
    }

    private IEnumerator PlaySequenceRoutine(List<int> sequence)
    {
        _playingSequence = true;
        foreach (var t in sequence)
        {
            if (!_gm.pianoEnabled) break;
            PlayNote(t);
            yield return new WaitForSeconds(playSequenceTime);
        }
        _playingSequence = false;
    }

    private void PlayNote(int num) // num = the number in the arrays
    {
        _image.sprite = sprites[num + 1];
        PlayClip(num);
    }

    public bool IsPlayingSequence()
    {
        return _playingSequence;
    }
}
