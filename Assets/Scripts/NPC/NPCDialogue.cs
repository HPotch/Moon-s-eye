using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class NPCDialogue : MonoBehaviour
{
    // Settings
    [Header("Dialogue")]
    [SerializeField] private string goodFile = ""; // The good file that is decoded to dialogue
    [SerializeField] private string badFile = ""; // The bad file that is decoded to dialogue
    [SerializeField] private float questionWaitTime = 0.3f;
    [SerializeField] private Vector2 dialogueOffset = new Vector2(0f, 1f);
    
    // Events
    [Header("Events")] [SerializeField] private UnityEvent onDialogueFinished;

    // References
    [Header("References")]
    [SerializeField] private GameObject dialoguePrefab;
    private GameObject _dialogue;
    private Dialogue _dialogueScript;

    // Private variables
    private bool _talking = false;
    private bool _dialogueStarted = false;
    private string _currentDialogue = ""; // Key in _talks dictionary for the current dialogue
    private string _startTalk = ""; // Key in _talks dictionary for the start dialogue
    private bool _justStarted = false; // Turns to true the frame that the dialogue starts

    // Dictionaries
    private Dictionary<string, Classes.Talk> _talks = new Dictionary<string, Classes.Talk>();
    private Dictionary<string, Classes.Question> _questions = new Dictionary<string, Classes.Question>();

    private void Awake()
    {
        if (dialoguePrefab == null) Debug.LogError("NPCDialogue: No Dialogue Prefab assigned!");
        SpawnDialogue();
    }
    
    public void MatchVibe()
    {
        SetText("Bond with me, press p to play piano and match my vibe!");
        _talking = true;
        _dialogueStarted = false;
    }

    public void StartDialogue(bool good)
    {
        if (_startTalk == "")
        {
            if (good) DecodeDialogue.DecodeTextFile(goodFile, out _talks, out _questions, out _startTalk);
            else DecodeDialogue.DecodeTextFile(badFile, out _talks, out _questions, out _startTalk);
        }
        SetTalk(_startTalk);
        _dialogueStarted = true;
        _justStarted = true;
    }

    private void Update()
    {
        if (_talking)
        {
            HandleNext();
        }
        HandleExit();
        
        _dialogue.gameObject.SetActive(_talking);
        _justStarted = false;
    }

    private void HandleExit()
    {
        if (GameManager.Instance.exitKeys.Any(key => Input.GetKeyDown(key)) && !GameManager.Instance.pianoEnabled)
            Exit();
    }
    
    private void HandleNext()
    {
        if (!(_dialogueStarted) || _justStarted) return;
        foreach (var key in GameManager.Instance.confirmKeys)
        {
            if (!Input.GetKeyDown(key)) continue;
            if (_dialogueScript.IsDone())
            {
                Classes.Talk cd = _talks[_currentDialogue];
                if (cd.End)
                {
                    Exit();
                    onDialogueFinished.Invoke();
                    return;
                }
                if (cd.TalkNext) { SetTalk(cd.NextTalk); return; }
                SpawnQuestions(cd);
            }
            else
            {
                _dialogueScript.FinishText();
            }
        }
    }

    private void SpawnQuestions(Classes.Talk cd)
    {
        float waitTime = 0f;
        foreach (string key in cd.NextQuestions)
        {
            GameManager.Instance.dqc.SpawnDialogueQuestion(_questions[key], this, waitTime);
            waitTime += questionWaitTime;
        }
        _talking = false;
    }

    public void SetText(string text)
    {
        _dialogueScript.SetText(text);
    }
    
    public void SetTalk(string key)
    {
        _currentDialogue = key;
        _dialogueScript.SetText(_talks[_currentDialogue].Text);
        _talking = true;
    }
    
    public void Exit()
    {
        _talking = false;
        _dialogueStarted = false;
        GameManager.Instance.talkingWith = null;
        GameManager.Instance.dqc.Empty();
        GameManager.Instance.pianoEnabled = false;
    }

    private void SpawnDialogue()
    {
        _dialogue = Instantiate(dialoguePrefab, transform);
        _dialogue.transform.position += new Vector3(dialogueOffset.x, dialogueOffset.y, 0f);
        _dialogueScript = _dialogue.GetComponent<Dialogue>();
    }
    
}
