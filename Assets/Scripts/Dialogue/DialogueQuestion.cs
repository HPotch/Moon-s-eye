using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Dialogue))]
public class DialogueQuestion : MonoBehaviour
{
    // Settings
    [SerializeField] private AnimationCurve sizeCurve;
    [SerializeField] private float sizeTime = 0.3f;
    [NonSerialized] public float waitTime = 0f;
    
    // References
    [NonSerialized] public DialogueQuestionContainer dqc;
    [NonSerialized] public Classes.Question Question;
    [NonSerialized] public NPCDialogue NPCDialogue;
    private Dialogue _dialogue;
    
    // Private variables
    private float _sizeTimer = 0f;
    private Vector3 _startSize;
    private bool _show = true;

    private void Awake()
    {
        _startSize = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void Start()
    {
        _dialogue = GetComponent<Dialogue>();
        _dialogue.type = false;
        if (Question == null) Debug.LogError("DialogueAnswer has no question assigned");
        _dialogue.SetText(Question.Text);
        _sizeTimer = -waitTime;
    }

    private void Update()
    {
        _sizeTimer += (Time.deltaTime / sizeTime) * (_show ? 1f : -1f);
        _sizeTimer = Mathf.Clamp(_sizeTimer, -waitTime, 1f);
        
        transform.localScale = Util.LerpWithoutClampV3(new Vector3(_startSize.x, 0f, _startSize.z), _startSize, sizeCurve.Evaluate(_sizeTimer));
        
        if (!_show && _sizeTimer <= 0f) Destroy(gameObject);
    }

    public void Select()
    {
        if (Question.End) NPCDialogue.Exit();
        else NPCDialogue.SetTalk(Question.NextTalk);
        dqc.Empty();
    }

    public void OnOff(bool on)
    {
        _show = on;
    }
}
