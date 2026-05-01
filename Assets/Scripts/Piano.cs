using System;
using UnityEngine;

public class Piano : MonoBehaviour
{
    // Settings
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private KeyCode[] keys;
    [SerializeField] private int[] midiNumbers;
    [SerializeField] private KeyCode octaveKey = KeyCode.LeftShift;
    [SerializeField] private AudioClip pianoSound;
    [SerializeField] private float volume = 5f;
    
    // References
    private SpriteRenderer _sr;

    
    // Static
    private static int _audioMidiNumber = 66; //(SHOULD BE ADJUSTED TO THE PIANOSOUNDS MIDI NUMBER!)
    
    // Private variables
    private float _baseFrequency = 0f; // The frequency of the pianoSound, depends on _audioMidiNumber
    
    private void Awake()
    {
        // Setup
        _sr = GetComponent<SpriteRenderer>();
        _baseFrequency = GetFrequency(_audioMidiNumber);
    }

    private void Update()
    {
        SetSprite();
    }

    private void SetSprite()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            var key = keys[i];
            if (Input.GetKey(key))
            {
                bool octave = Input.GetKey(octaveKey);
                var num = i + 1 + (octave ? 7 : 0); // Number in the arrays of the pressed key
                
                if (num < sprites.Length)
                {
                    // Set sprite and play sound
                    _sr.sprite = sprites[num];
                    if (Input.GetKeyDown(key)) PlaySound(i + (octave ? 7 : 0));
                    return; // Ensure no unnecessary checks are done
                }
                
            }
        }

        _sr.sprite = sprites[0]; // Reset to default if no keys are pressed
    }

    
    private void PlaySound(int pressedKey)
    {
        float targetFrequency = GetFrequency(midiNumbers[pressedKey]);
        
        float pitch = targetFrequency / _baseFrequency;
        AudioManager.Instance.StartSound(pianoSound, volume, pitch);
    }

    private float GetFrequency(int midiNumber)
    {
        // Formula for converting midi numbers to pitch
        float midiPower = (midiNumber - 69f) / 12f;
        float frequency = 440f * Mathf.Pow(2f, midiPower);
        return frequency;
    }
}
