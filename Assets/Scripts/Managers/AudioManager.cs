using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private GameObject audioObject;
    [SerializeField] private GameObject audioParent;

    [SerializeField] private AudioClip loopMusic;
    [SerializeField] private float musicVolume = 0.5f;
    [SerializeField] private float musicFadeTime = 0.75f;
    [SerializeField] private AudioClip ambientSound;
    [SerializeField] private float ambientVolume = 1f;
    

    private float _fadeTimer = 0f;
    private AudioSource _musicSource;
    
    private void Awake()
    {
        // Setup Manager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _musicSource = StartClip(loopMusic, 1f, 1f, true);
        StartClip(ambientSound, ambientVolume, 1f, true);
    }

    private void Update()
    {
        GameManager gm = GameManager.Instance;
        _fadeTimer += Time.deltaTime / musicFadeTime * (gm.pianoEnabled ? 1f : -1f);
        _fadeTimer = Mathf.Clamp01(_fadeTimer);
        
        if (_fadeTimer >= 1f)
        {
            _musicSource.Pause();
            return;
        }

        if (!_musicSource) return;
        if (!_musicSource.isPlaying) _musicSource.UnPause();
        if (_fadeTimer > 0f) _musicSource.volume = musicVolume * (1f - _fadeTimer);
    }

    public AudioSource StartClip(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = false)
    {
        GameObject sound = Instantiate(audioObject, audioParent.transform);
        
        AudioSource source = sound.GetComponent<AudioSource>();
        if(!source) { Debug.LogWarning("soundObject doesnt contain AudioSource!"); return null;}
        
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
        
        AudioObject ao = source.GetComponent<AudioObject>();
        if(!ao) { Debug.LogWarning("soundObject doesnt contain AudioObject!"); return null;}
        ao.soundTime = loop ? Mathf.Infinity : clip.length;
        
        sound.name = clip.name;
        source.Play();
        
        return source;
    }

    public void StartRandomClip(AudioClip[] clips, float volume = 1f, float pitch = 1f)
    {
        StartClip(clips[Random.Range(0, clips.Length - 1)], volume, pitch);
    }

    public void StopClip(AudioClip clip)
    {
        foreach (Transform child in transform)
        {
            if (child.name == clip.name)
            {
                Destroy(child.gameObject);
                return;
            }
        }
    }
}
