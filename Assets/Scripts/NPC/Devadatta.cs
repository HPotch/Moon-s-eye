using UnityEngine;

public class Devadatta : MonoBehaviour
{
    [SerializeField] private TextAsset readFile;
    [SerializeField] private TextAsset nextSaicho;
    [SerializeField] private NPCDialogue saichoOakes;
    private NPCDialogue _dialogue;
    private bool _changed = false;
    private bool _changedBySaicho = false;

    private void Awake()
    {
        _dialogue = GetComponent<NPCDialogue>();
    }

    public void LoadRead()
    {
        if (_changedBySaicho)
        {
            saichoOakes.LoadText(nextSaicho);
        }
        if (_changed) return;
        _changed = true;
        _dialogue.LoadText(readFile);
    }

    public void SaichoChange()
    {
        _changedBySaicho = true;
    }
}