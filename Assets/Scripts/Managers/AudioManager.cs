using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private GameObject audioObject;

    [SerializeField] private GameObject audioParent;
    
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
    
    public void StartClip(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        GameObject sound = Instantiate(audioObject, audioParent.transform);
        
        AudioSource source = sound.GetComponent<AudioSource>();
        if(source == null) { Debug.LogWarning("soundObject doesnt contain AudioSource!"); return;}
        
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        
        AudioObject ao = source.GetComponent<AudioObject>();
        if(ao == null) { Debug.LogWarning("soundObject doesnt contain AudioObject!"); return;}
        ao.soundTime = clip.length;
        
        sound.name = clip.name;
        source.Play();
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
