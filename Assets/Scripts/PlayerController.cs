using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Settings
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private AnimationCurve moveStartCurve;
    [SerializeField] private float moveStartTime = 1f;
    [SerializeField] private AnimationCurve moveEndCurve;
    [SerializeField] private float moveEndTime = 1f;
    
    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 2f;
    
    [Header("Input")]
    [SerializeField] private KeyCode[] moveUpKeys = {KeyCode.W, KeyCode.UpArrow, KeyCode.I};
    [SerializeField] private KeyCode[] moveDownKeys = {KeyCode.S, KeyCode.DownArrow, KeyCode.K};
    [SerializeField] private KeyCode[] moveLeftKeys = {KeyCode.A, KeyCode.LeftArrow, KeyCode.J};
    [SerializeField] private KeyCode[] moveRightKeys = {KeyCode.D, KeyCode.RightArrow, KeyCode.L};
    [SerializeField] private KeyCode[] interactionKeys = {KeyCode.E, KeyCode.U};
    [SerializeField] private KeyCode[] openPianoKeys = {KeyCode.Space, KeyCode.Q, KeyCode.O, KeyCode.Tab};
    [SerializeField] private KeyCode[] runKeys = {KeyCode.LeftShift, KeyCode.RightShift};

    // References
    [SerializeField] private Transform NPCs;
    private Rigidbody2D _rb;
    private float _startTimer;
    private float _endTimer;
    private Vector2 _lastMove;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        var inputMode = GameManager.Instance.currentInputMode;
        Vector2 moveVelocity = Vector2.zero;

        switch (inputMode)
        {
            case GameManager.InputMode.Keyboard:
                moveVelocity = new Vector2(IsHeld(moveLeftKeys, true) + IsHeld(moveRightKeys), IsHeld(moveDownKeys, true) + IsHeld(moveUpKeys)).normalized;
                break;
            case GameManager.InputMode.Controller:
                moveVelocity = Gamepad.current.leftStick.value;
                break;
        }
        
        if (GameManager.Instance.pianoEnabled) moveVelocity = Vector2.zero;

        float moveMultplier = 0f;
        if (moveVelocity == Vector2.zero)
        {
            if (_startTimer / moveStartTime < 1f && _startTimer != 0f) _endTimer = 1f / moveEndTime; // Skips moveEndCurve when not fully sprinting yet.
            
            _startTimer = 0f;
            _endTimer +=  Time.fixedDeltaTime;
            moveMultplier = moveEndCurve.Evaluate(Mathf.Clamp01(_endTimer / moveEndTime));
            
            _rb.linearVelocity = _lastMove * (moveSpeed * Time.fixedDeltaTime * moveMultplier);
        }
        else
        {
            _endTimer = 0f;
            _startTimer += Time.fixedDeltaTime;
            moveMultplier = moveStartCurve.Evaluate(Mathf.Clamp01(_startTimer / moveStartTime));
            
            _rb.linearVelocity = moveVelocity * (moveSpeed * Time.fixedDeltaTime * moveMultplier);
            _lastMove = moveVelocity;
        }

        HandleInteraction();
    }

    private void HandleInteraction()
    {
        GameObject closestNPC = null;
        var closestDistance = float.PositiveInfinity;
        for (var i = 0; i < NPCs.childCount; i++)
        {
            var child = NPCs.GetChild(i);
            var distance = (transform.position - child.position).magnitude;
            if (!(distance < closestDistance)) continue;
            closestNPC = child.gameObject;
            closestDistance = distance;
        }

        GameManager.Instance.closestNPC = null;
        if (!(closestDistance < interactionDistance) || closestNPC is null || closestNPC == GameManager.Instance.talkingWith) return;
        GameManager.Instance.closestNPC = closestNPC;
        
        NPCManager manager = closestNPC.GetComponent<NPCManager>();
        foreach (var key in interactionKeys)
        {
            if (!Input.GetKeyDown(key)) continue;
            manager.Talk();
            GameManager.Instance.talkingWith = closestNPC;
        }
    }

    private int IsHeld(KeyCode[] keys, bool inv = false)
    {
        foreach (KeyCode key in keys)
        {
            if (!Input.GetKey(key)) break;
            return inv ? -1 : 1;
        }
        return 0;
    }
}
