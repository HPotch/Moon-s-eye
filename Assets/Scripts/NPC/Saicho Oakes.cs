using System.Collections;
using UnityEngine;

public class SaichoOakes : MonoBehaviour
{
    [SerializeField] private GameObject devadatta;
    [SerializeField] private TextAsset nextDevadatta;
    [SerializeField] private TextAsset readFile;

    private NPCDialogue _devadattaDialogue;
    private NPCDialogue _dialogue;
    private Devadatta _devadattaScript;
    private bool _changed = false;

    private void Awake()
    {
        _dialogue = GetComponent<NPCDialogue>();
        _devadattaDialogue = devadatta.GetComponent<NPCDialogue>();
        _devadattaScript = devadatta.GetComponent<Devadatta>();
    }

    public void ChangeDevadatta()
    {
        if (_changed) return;
        _changed = true;
        _dialogue.LoadText(readFile);
        _devadattaDialogue.LoadText(nextDevadatta);
        _devadattaScript.SaichoChange();
    }
}
