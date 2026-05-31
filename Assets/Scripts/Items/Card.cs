using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool destroyOnExit = true;
    [SerializeField] private AnimationCurve popUpAnimation = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float popUpTime = 0.5f;
    [SerializeField] private AnimationCurve exitAnimation = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float exitTime = 0.5f;
    [Header("References")]
    [SerializeField] private RectTransform image;
    [SerializeField] private RectTransform button;
    [SerializeField] private Image blur;

    private float _timer = 0f;
    private bool _popUp = true;
    private bool _exit = false;
    private Color _blurStartColor;
    private Vector2 _buttonStartPos;

    private void Awake()
    {
        _blurStartColor = blur.color;
        image.localScale = Vector3.zero;
        button.localScale = Vector3.zero;
        blur.color = new Color(_blurStartColor.r, _blurStartColor.g, _blurStartColor.b, 0f);
        _buttonStartPos = button.position;
    }
    
    private void Update()
    {
        foreach (var key in GameManager.Instance.exitKeys.Where(Input.GetKeyDown)) _exit = true;
        
        PopUp();
        Exit();
    }

    private void PopUp()
    {
        if (!_popUp) return;
        _timer += Time.deltaTime / popUpTime;
        float t = popUpAnimation.Evaluate(Mathf.Clamp01(_timer));
        float r = Mathf.Lerp(0f, 1f, t);
        image.localScale = new Vector3(r, r, r);
        button.localScale = new Vector3(r, r, r);
        blur.color = new Color(_blurStartColor.r, _blurStartColor.g, _blurStartColor.b, r);
        if (_timer > 1f) _popUp = false;
    }

    private void Exit()
    {
        if (!_exit) return;
        _timer += Time.deltaTime / exitTime;
        float t = exitAnimation.Evaluate(Mathf.Clamp01(_timer));
        float r = Mathf.Lerp(0f, 1f, t);
        image.position = new Vector2(r * image.rect.width, 0f);
        button.position = _buttonStartPos + new Vector2(r * image.rect.width, 0f);
        blur.color = new Color(_blurStartColor.r, _blurStartColor.g, _blurStartColor.b, 1f - r);
        if (!(_timer > 1f)) return;
        _exit = false;
        if (destroyOnExit) Destroy(gameObject);

    }

    public void StartPopUp()
    {
        if (_exit) return;
        _popUp = true;
    }

    public void StartExit()
    {
        if (_popUp) return;
        _timer = 0f;
        _exit = true;
    }
}
