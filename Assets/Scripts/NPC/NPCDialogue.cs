using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

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
    private int _currentDialogue = 0;
    private bool _dialogueStarted = false;

    // Classes
    internal class Talk
    {
        internal string _text;
        internal bool _talkNext;
        internal List<string> _nextQuestions = new List<string>();
        internal string _nextTalk;
        internal bool _end;
    }

    private class Question
    {
        internal string _text;
        internal string _nextTalk;
        internal bool _end;
    }

    private Dictionary<string, Talk> _talks = new Dictionary<string, Talk>();
    private Dictionary<string, Question> _questions = new Dictionary<string, Question>();
    private Talk _startTalk = null;

    private enum DecodeMode {
        CheckType,
        CheckKey,
        CheckVal,
        CheckRef
    };

    private enum TypeMode
    {
        Talk,
        Question
    }

    private void Awake()
    {
        if (dialoguePrefab == null) Debug.LogError("NPCDialogue: No Dialogue Prefab assigned!");
        SpawnDialogue();
        DecodeTextFile();
    }

    private void DecodeTextFile()
    {
        string[] lines = File.ReadAllLines(filePath);
        int lineNum = 0;
        foreach (string line in lines)
        {
            DecodeLine(line, lineNum);
            lineNum += 1;
        }
        PrintDecodedDictionaries();
    }
    
    private void PrintDecodedDictionaries()
    {
        Debug.Log("========== DECODED TALKS ==========");
        foreach (KeyValuePair<string, Talk> kvp in _talks)
        {
            string key = kvp.Key;
            Talk t = kvp.Value;
            
            // Join the list of questions into a single comma-separated string for easy reading
            string questionsStr = t._nextQuestions != null && t._nextQuestions.Count > 0 
                ? string.Join(", ", t._nextQuestions) 
                : "None";

            Debug.Log($"Talk Key: [{key}] | Text: '{t._text}' | TalkNext: {t._talkNext} | NextTalk: '{t._nextTalk}' | NextQuestions: [{questionsStr}] | End: {t._end}");
        }

        Debug.Log("========== DECODED QUESTIONS ==========");
        foreach (KeyValuePair<string, Question> kvp in _questions)
        {
            string key = kvp.Key;
            Question q = kvp.Value;

            Debug.Log($"Question Key: [{key}] | Text: '{q._text}' | NextTalk: '{q._nextTalk}' | End: {q._end}");
        }
    }

    private void DecodeLine(string line, int lineNum = -1)
    {
        // Variables to decode
        bool isStart = false;
        TypeMode typeMode = TypeMode.Talk;
        DecodeMode mode = DecodeMode.CheckType;
        string key = "";
        string val = "";
        bool talkNext = false;
        string nextTalk = "";
        List<String> nextQuestions = new List<string>();
        bool isEnd = false;
        
        // Decode
        foreach (var c in line)
        {
            switch (mode)
            {
                case DecodeMode.CheckType:
                    if (c == '/') typeMode = TypeMode.Talk;
                    else if (c == '?') typeMode = TypeMode.Question;
                    else Debug.LogError(filePath + ": Line " + lineNum.ToString() + " doesn't start with a valid mode: '/'(talk) or '?'(question)");
                    mode = DecodeMode.CheckKey;
                    continue;
                case DecodeMode.CheckKey:
                    if (c == '>' && _startTalk != null)
                    {
                        isStart = true;
                        continue;
                    }

                    if (c == ':')
                    {
                        mode = DecodeMode.CheckVal;
                        continue;
                    }
                    key += c;
                    continue;
                case DecodeMode.CheckVal:
                    if (c == ';')
                    {
                        mode = DecodeMode.CheckRef;
                        continue;
                    }

                    val += c;
                    continue;
                case DecodeMode.CheckRef:
                    if (c == '<')
                    {
                        isEnd = true;
                        continue;
                    }
                    
                    switch (typeMode)
                    {
                        case TypeMode.Talk:
                            switch (c)
                            {
                                case '/':
                                    talkNext = true;
                                    continue;
                                case '?':
                                    talkNext = false;
                                    continue;
                            }

                            if (talkNext)
                            {
                                nextTalk += c;
                            }
                            else
                            {
                                if (c == ';')
                                {
                                    nextQuestions.Add("");
                                    continue;
                                }
                                if (nextQuestions.Count == 0) nextQuestions.Add(c.ToString());
                                else nextQuestions[^1] += c;
                            }
                            continue;
                        case TypeMode.Question:
                            nextTalk += c;
                            continue;
                    }
                    continue;
            }
        }

        if (typeMode == TypeMode.Talk) CreateTalk(val, talkNext, nextQuestions, nextTalk, isEnd, key);
        else CreateQuestion(val, nextTalk, isEnd, key);
    }

    private void CreateTalk(string text, bool talkNext, List<string> nextQuestions, string nextTalk, bool end, string key)
    {
        Talk t = new Talk
        {
            _text = text,
            _talkNext = talkNext,
            _nextQuestions = nextQuestions,
            _nextTalk = nextTalk,
            _end = end
        };
        _talks.Add(key, t);
    }

    private void CreateQuestion(string text, string nextTalk, bool end, string key)
    {
        Question q = new Question
        {
            _text = text,
            _nextTalk = nextTalk,
            _end = end
        };
        _questions.Add(key, q);
    }

    public void MatchVibe()
    {
        _dialogueScript.SetText("Come and match my vibe man!");
        _talking = true;
        _dialogueStarted = false;
    }

    public void StartDialogue()
    {
        _currentDialogue = 0;
        _dialogueScript.SetText(dialogueText[_currentDialogue]);
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
                _currentDialogue += 1;
                if (_currentDialogue >= dialogueText.Count) Exit();
                        
                _dialogueScript.SetText(dialogueText[_currentDialogue]);
            }
            else
            {
                _dialogueScript.FinishText();
            }
        }
    }
    
    private void Exit()
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
