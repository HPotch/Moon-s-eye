using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    // Settings
    [SerializeField] private List<string> dialogueText;
    [SerializeField] private string filePath = "";
    [SerializeField] private List<KeyCode> nextKeys = new List<KeyCode>();

    [SerializeField] private List<KeyCode> exitKeys = new List<KeyCode>();

    // References
    [SerializeField] private GameObject dialoguePrefab;
    private GameObject _dialogue;
    private Dialogue _dialogueScript;

    // Private variables
    private bool _talking = false;
    private string _currentDialogue = "";
    private bool _dialogueStarted = false;
    private string _startTalk = "";

    // Dictionaries
    private Dictionary<string, Classes.Talk> _talks = new Dictionary<string, Classes.Talk>();
    private Dictionary<string, Classes.Question> _questions = new Dictionary<string, Classes.Question>();

    private void Awake()
    {
        if (dialoguePrefab == null) Debug.LogError("NPCDialogue: No Dialogue Prefab assigned!");
        SpawnDialogue();
        DecodeDialogue.DecodeTextFile(filePath, out _talks, out _questions, out _startTalk);
        PrintDecodedDictionaries();
    }
    
    private void PrintDecodedDictionaries()
    {
        Debug.Log("========== DECODED TALKS ==========");
        foreach (KeyValuePair<string, Classes.Talk> kvp in _talks)
        {
            string key = kvp.Key;
            Classes.Talk t = kvp.Value;
            
            // Join the list of questions into a single comma-separated string for easy reading
            string questionsStr = t.NextQuestions != null && t.NextQuestions.Count > 0 
                ? string.Join(", ", t.NextQuestions) 
                : "None";

            Debug.Log($"Talk Key: [{key}] | Text: '{t.Text}' | TalkNext: {t.TalkNext} | NextTalk: '{t.NextTalk}' | NextQuestions: [{questionsStr}] | End: {t.End}");
        }

        Debug.Log("========== DECODED QUESTIONS ==========");
        foreach (KeyValuePair<string, Classes.Question> kvp in _questions)
        {
            string key = kvp.Key;
            Classes.Question q = kvp.Value;

            Debug.Log($"Question Key: [{key}] | Text: '{q.Text}' | NextTalk: '{q.NextTalk}' | End: {q.End}");
        }
    }
    
    public void MatchVibe()
    {
        _dialogueScript.SetText("Come and match my vibe man!");
        _talking = true;
        _dialogueStarted = false;
    }

    public void StartDialogue()
    {
        SetTalk(_startTalk);
        _dialogueStarted = true;
    }

    private void Update()
    {
        if (_talking)
        {
            HandleExit();
            HandleNext();
        }
        
        _dialogue.gameObject.SetActive(_talking);
    }

    private void HandleExit()
    {
        foreach (var key in exitKeys)
        {
            if (!Input.GetKeyDown(key)) continue;
            Exit();
            break;
        }
    }
    
    private void HandleNext()
    {
        if (!_dialogueStarted) return;
        foreach (var key in nextKeys)
        {
            if (!Input.GetKeyDown(key)) continue;
            if (_dialogueScript.IsDone())
            {
                Classes.Talk cd = _talks[_currentDialogue];
                if (cd.End) { Exit(); return; }
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
        List<Classes.Question> nextQuestions;
        foreach (string key in cd.NextQuestions)
        {
            GameManager.Instance.dqc.SpawnDialogueQuestion(_questions[key], this);
        }

        _talking = false;
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
        GameManager.Instance.talkingWith = null;
    }

    private void SpawnDialogue()
    {
        _dialogue = Instantiate(dialoguePrefab, transform);
        _dialogueScript = _dialogue.GetComponent<Dialogue>();
    }
    
}
