using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dialogue))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class DialogueQuestion : MonoBehaviour
{
    // Settings
    [SerializeField] private AnimationCurve sizeCurve;
    [SerializeField] private float sizeTime = 0.3f;
    [SerializeField] private Vector3 animationAxis = Vector3.up;
    [SerializeField] private Sprite selectSprite;
    [SerializeField] private Sprite noSelectSprite;
    [SerializeField] private AudioClip confirm;
    [SerializeField] private AudioClip select;
    [NonSerialized] public float waitTime = 0f;
    
    // References
    [NonSerialized] public DialogueQuestionContainer dqc;
    [NonSerialized] public Classes.Question Question;
    [NonSerialized] public NPCDialogue NPCDialogue;
    private Dialogue _dialogue;
    private BoxCollider2D _collider;
    private RectTransform _rectTransform;
    private Image _image;
    
    // Private variables
    private float _sizeTimer = 0f;
    private Vector3 _startSize;
    private bool _show = true;
    [NonSerialized] public bool Selected = false;
    private bool _justSelected = false;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        
        _startSize = transform.localScale;
        transform.localScale = Vector3.zero;

        noSelectSprite ??= _image.sprite;
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
        _collider.size = new Vector2(_rectTransform.rect.width, _rectTransform.rect.height);

        if ((GameManager.Instance.mouseOver == gameObject && GameManager.Instance.currentInputMode == GameManager.InputMode.Keyboard) || Selected)
        {
            if (_justSelected)
            {
                AudioManager.Instance.StartClip(select);
                _justSelected = false;
            }
            
            _image.sprite = selectSprite;
            if (Input.GetMouseButtonDown(0)) Select();
            else
            {
                foreach (var key in GameManager.Instance.confirmKeys)
                {
                    if (Input.GetKeyDown(key)) Select();
                }
            }
            
        }
        else
        {
            _justSelected = true;
            _image.sprite = noSelectSprite;
        }

        
        
        _sizeTimer += (Time.deltaTime / sizeTime) * (_show ? 1f : -1f);
        _sizeTimer = Mathf.Clamp(_sizeTimer, -waitTime, 1f);

        Vector3 axis = new Vector3(_startSize.x * animationAxis.x, _startSize.y * animationAxis.y, _startSize.z * animationAxis.z);
        transform.localScale = Util.LerpWithoutClampV3(axis, _startSize, sizeCurve.Evaluate(_sizeTimer));
        
        if (!_show && _sizeTimer <= 0f) Destroy(gameObject);
    }

    public void Select()
    {
        AudioManager.Instance.StartClip(confirm);
        if (Question.End)
        {
            NPCDialogue.Exit();
            NPCDialogue.onDialogueFinished?.Invoke();
        }
        else NPCDialogue.SetTalk(Question.NextTalk);
        dqc.Empty();
    }

    public void OnOff(bool on)
    {
        _show = on;
    }

}
