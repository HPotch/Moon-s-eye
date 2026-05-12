using System.Collections.Generic;
using System.Linq;
using Minis;
using UnityEngine;
using UnityEngine.UI;

public class Piano : MonoBehaviour
{
    // Settings
    [SerializeField] private KeyCode onOffKey = KeyCode.P;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private KeyCode[] keys;
    [SerializeField] private int[] midiNumbers;
    [SerializeField] private KeyCode octaveKey = KeyCode.LeftShift;
    [SerializeField] private AudioClip pianoSound;
    [SerializeField] private float volume = 5f;
    [SerializeField] private float sequenceTime = 1f;
    
    // References
    private Image _image;

    // Static
    private const int AudioMidiNumber = 66; //(SHOULD BE ADJUSTED TO THE PIANOSOUNDS MIDI NUMBER!)

    // Private variables
    private float _baseFrequency = 0f; // The frequency of the pianoSound, depends on _audioMidiNumber
    private float _sequenceTimer = 0f;
    private List<int> _sequence = new List<int>(); // Sequence recording
    private bool _on = false;
    private MidiDevice _currentDevice;

    private void Awake()
    {
        // Setup
        _image = GetComponent<Image>();
        _baseFrequency = GetFrequency(AudioMidiNumber);
    }

    private void Start()
    {
        if (GameManager.Instance.piano is null) GameManager.Instance.piano = this;
    }

    private void Update()
    {
        UpdateCurrentDevice();
        
        if (Input.GetKeyDown(onOffKey)) _on = !_on;
        if (!_on) return;
        
        SetSprite();
        
        _sequenceTimer -= Time.deltaTime;
        if (_sequenceTimer <= 0f) _sequence.Clear();
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
                var num = i + 1 + (octave ? 7 : 0); // Number in the arrays of the pressed key

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

        _image.sprite = sprites[0]; // Reset to default if no keys are pressed
    }

    private void OnNoteOn(MidiNoteControl note, float velocity)
    {
        if (!midiNumbers.Contains(note.noteNumber)) return;
        foreach (var t in midiNumbers)
        {
            if (note.noteNumber == t)
            {
                
            }
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

    public bool CheckSequence(List<int> referenceSequence)
    {
         return referenceSequence.SequenceEqual(_sequence);
    }
}
