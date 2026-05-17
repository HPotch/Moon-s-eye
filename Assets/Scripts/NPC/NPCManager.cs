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
    [SerializeField] private bool skipVibing = false;
    
    // References
    private Piano _piano;
    private NPCDialogue _dialogue;
    private Tooltip _tooltip;

    // Private variables
    private int _good = -1;
    
    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("NPC");
        _tooltip = GetComponentInChildren<Tooltip>();
    }
    
    private void Start()
    {
        _piano = GameManager.Instance.piano;
        _dialogue = GetComponent<NPCDialogue>();
    }

    private void Update()
    {
        _piano ??= GameManager.Instance.piano;
        _tooltip.OnOff(GameManager.Instance.closestNPC == gameObject);
    }

    public void Talk()
    {
        StartCoroutine(TalkRoutine());
    }

    private IEnumerator TalkRoutine()
    {
        if (skipVibing) _good = 1;
        if (_good == -1)
        {
            _dialogue.MatchVibe();
            yield return new WaitUntil(() => GameManager.Instance.pianoEnabled);
            yield return new WaitForSeconds(_piano.PlaySequenceStartTime);
            _piano.PlaySequence(sequence);
            yield return new WaitUntil(() => _piano.IsPlayingSequence());
            yield return new WaitUntil(() => _piano.CheckSequenceLength(sequence));
            _good = _piano.CheckSequence(sequence) ? 1 : 0;
        }
        _dialogue.StartDialogue(_good == 1);
    }
}
