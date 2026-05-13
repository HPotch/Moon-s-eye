using UnityEngine;

public class DialogueQuestionContainer : MonoBehaviour
{
    [SerializeField] private GameObject _dialogueQuestionPrefab;
    
    private void Update()
    {
        GameManager.Instance.dqc ??= this;
    }

    public void SpawnDialogueQuestion(Classes.Question question, NPCDialogue dialogue)
    {
        GameObject dqp = Instantiate(_dialogueQuestionPrefab, transform);
        DialogueQuestion dq = dqp.GetComponent<DialogueQuestion>();
        dq.Question = question;
        dq.NPCDialogue = dialogue;
        dq.dqc = this;
    }

    public void Empty()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
