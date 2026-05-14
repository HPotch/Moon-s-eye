using UnityEngine;

public class DialogueQuestionContainer : MonoBehaviour
{
    [SerializeField] private GameObject _dialogueQuestionPrefab;
    
    private void Update()
    {
        GameManager.Instance.dqc ??= this;
    }

    public void SpawnDialogueQuestion(Classes.Question question, NPCDialogue dialogue, float waitTime = 0f)
    {
        GameObject dqp = Instantiate(_dialogueQuestionPrefab, transform);
        DialogueQuestion dq = dqp.GetComponent<DialogueQuestion>();
        dq.Question = question;
        dq.NPCDialogue = dialogue;
        dq.dqc = this;
        dq.waitTime = waitTime;
    }

    public void Empty()
    {
        foreach (Transform child in transform)
        {
            DialogueQuestion dq = child.GetComponent<DialogueQuestion>();
            dq?.OnOff(false);
        }
    }
}
