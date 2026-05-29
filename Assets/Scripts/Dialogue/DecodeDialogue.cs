using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class DecodeDialogue
{
    // Script-wide variables (used for debug printing!)
    private static int _lineNum = 0;
    private static string _filePath;
    
    // Return variables, these are edited and returned at the end
    private static string _startTalk = "";
    private static Dictionary<string, Classes.Talk> _talks = new Dictionary<string, Classes.Talk>();
    private static Dictionary<string, Classes.Question> _questions = new Dictionary<string, Classes.Question>();
    
    // Enums, used for easy mode switching
    private enum DecodeMode {
        CheckType, // Checks whether line is a talk or a question
        CheckKey, // Checks the key that is assigned to the line in a dictionary
        CheckVal, // Check the "value" or the text that this line is set to
        CheckRef // Check the references, so the next dialogues. Talks can have talks and questions as ref, but questions can only follow with a talk.
    };

    private enum TypeMode
    {
        Talk,
        Question
    }
    
    public static void DecodeText(string fileContent, string sourceName, out Dictionary<string, Classes.Talk> talks, out Dictionary<string, Classes.Question> questions, out string startTalk)
    {
        Clear();
        _filePath = sourceName;
        _lineNum = 1;
        
        if (string.IsNullOrEmpty(fileContent))
        {
            Debug.LogError($"DecodeDialogue: The file content for '{sourceName}' is empty!");
            talks = _talks;
            questions = _questions;
            startTalk = _startTalk;
            return;
        }

        string[] lines = fileContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        
        foreach (string line in lines) 
        {
            if (string.IsNullOrWhiteSpace(line)) continue; 

            DecodeLine(line);
            _lineNum += 1;
        }
        
        if (_startTalk == "") Debug.LogError(_filePath + ": No line defines a start, one Talk line should have '>' right after the type declaration, to mark the start.");
        
        talks = _talks; 
        questions = _questions;
        startTalk = _startTalk;
    }

    private static void Clear()
    {
        _startTalk = "";
        _talks.Clear();
        _questions.Clear();
    }

    private static void DecodeLine(string line)
    {
        // Variables to decode, these variables are extracted from each line
        bool isStart = false; // Defines if this is the line dialogue starts at
        TypeMode typeMode = TypeMode.Talk; // Is this a talk or a question?
        DecodeMode mode = DecodeMode.CheckType; // The current mode the decoding is in, modes are explained in the declaring of DecodeMode
        string key = ""; // The key that is used in the dictionaries
        string val = ""; // The value - the text that this line is given
        bool talkNext = false; // Is the reference a talk?
        string nextTalk = ""; // If the reference is a talk, this will be the next talk !This can only be 1 talk, not multiple!
        List<String> nextQuestions = new List<string>(); // If the reference isn't a talk, this list contains the possible questions as follow-up
        bool isEnd = false; // Does this line mark an end? Is the conversations over after this line?
        
        // Decode
        foreach (var c in line) // Loop through all characters of the line
        {
            switch (mode) // Do all the checks
            {
                case DecodeMode.CheckType:
                    if (c == '/') typeMode = TypeMode.Talk;
                    else if (c == '?') typeMode = TypeMode.Question;
                    else { Debug.LogError(_filePath + " : Line " + _lineNum.ToString() + " doesn't start with a valid mode: '/'(talk) or '?'(question). Aborting line decoding."); return; }
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

        // Error messages
        if (key == "") { Debug.LogError(_filePath + " Line: " + _lineNum.ToString() + " doesn't contain a key. Keys should be defined after defining a type, they can contain any symbols, but ends with a ':'. Aborting line decoding."); return; }
        if (val == "") Debug.LogWarning(_filePath + " Line: " + _lineNum.ToString() + " doesn't have a value, the dialogue box will be empty. Values are assigned between ':' and ';' and after type- and key defining.");
        if (nextTalk == "" && nextQuestions.Count == 0 && !isEnd) { Debug.LogError(_filePath + " Line : " + _lineNum.ToString() + " doesn't have a reference to a next step, should contain one or multiple references to Talks or Questions, or mark an end '<'. Aborting line decoding."); return; }
        
        // Create a new Talk or Question out of the variables at the start of this function
        if (typeMode == TypeMode.Talk) CreateTalk(val, talkNext, nextQuestions, nextTalk, isEnd, key);
        else CreateQuestion(val, nextTalk, isEnd, key);

        if (isStart) _startTalk = key; // Set the start to a Talk
    }

    private static void CreateTalk(string text, bool talkNext, List<string> nextQuestions, string nextTalk, bool end, string key)
    {
        Classes.Talk t = new Classes.Talk
        {
            Text = text,
            TalkNext = talkNext,
            NextQuestions = nextQuestions,
            NextTalk = nextTalk,
            End = end
        };
        _talks.Add(key, t);
    }

    private static void CreateQuestion(string text, string nextTalk, bool end, string key)
    {
        Classes.Question q = new Classes.Question
        {
            Text = text,
            NextTalk = nextTalk,
            End = end
        };
        _questions.Add(key, q);
    }
}
