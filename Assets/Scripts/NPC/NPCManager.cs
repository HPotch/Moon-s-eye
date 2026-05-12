using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(NPCDialogue))]
[RequireComponent(typeof(NPCMovement))]
[RequireComponent(typeof(Collider2D))]
public class NPCManager : MonoBehaviour
{
    // Settings
    [SerializeField] private List<int> sequence = new List<int>();
    
    // References
    private Piano _piano;
    private NPCDialogue _dialogue;

    private void Start()
    {
        _piano = GameManager.Instance.piano;
        _dialogue = GetComponent<NPCDialogue>();
    }

    private void Update()
    {
        _piano ??= GameManager.Instance.piano;
    }
    
    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("NPC");
    }

    public void Talk()
    {
        StartCoroutine(TalkRoutine());
    }

    private IEnumerator TalkRoutine()
    {
        _dialogue.MatchVibe();
        yield return new WaitForSeconds(_piano.PlaySequenceStartTime);
        _piano.PlaySequence(sequence);
        yield return new WaitUntil(() => _piano.IsPlayingSequence());
        yield return new WaitUntil(() => _piano.CheckSequence(sequence));
        _dialogue.StartDialogue();
    }
}
