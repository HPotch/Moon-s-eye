using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitUntil(() => GameManager.Instance.pianoEnabled);
                yield return new WaitForSeconds(_piano.PlaySequenceStartTime);
                _piano.PlaySequence(sequence);
                yield return new WaitUntil(() => _piano.IsPlayingSequence());
                yield return new WaitUntil(() => _piano.CheckSequenceLength(sequence));
                _good = _piano.CheckSequence(sequence) ? 1 : 0;
                if (_good == 0)
                {
                    switch (i)
                    {
                        case 0:
                            _dialogue.SetText("What do you mean man??"); break;
                        case 1:
                            _dialogue.SetText("I still don't get it."); break;
                        case 2:
                            _dialogue.SetText("Man I don't like you"); break;
                    }

                    yield return new WaitForSeconds(3f);
                    _dialogue.SetText("Let me play that again.");
                    _piano.ClearSequence();
                }
                else break;
            }
        }
        _dialogue.StartDialogue(_good == 1);
        GameManager.Instance.pianoEnabled = false;
    }
}
