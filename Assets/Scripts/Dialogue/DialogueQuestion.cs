using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Dialogue))]
public class DialogueQuestion : MonoBehaviour
{
    public DialogueQuestionContainer dqc;
    public Classes.Question Question;
    [FormerlySerializedAs("_NPCDialogue")] public NPCDialogue NPCDialogue;
    private Dialogue _dialogue;

    private void Start()
    {
        _dialogue = GetComponent<Dialogue>();
        if (Question == null) Debug.LogError("DialogueAnswer has no question assigned");
        _dialogue.SetText(Question.Text);
    }

    public void Select()
    {
        if (Question.End) NPCDialogue.Exit();
        else NPCDialogue.SetTalk(Question.NextTalk);
        dqc.Empty();
    }
}
