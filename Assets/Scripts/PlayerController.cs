using System.Linq;
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
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float jumpTime = 0.8f;
    [SerializeField] private float jumpHeight = .2f;
    [SerializeField] private float jumpSpeedInfluence = 0.5f;
    [SerializeField] private float baseSpeed = 0.5f;
    
    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 2f;
    
    [Header("Input")]
    [SerializeField] private KeyCode[] moveUpKeys = {KeyCode.W, KeyCode.UpArrow, KeyCode.I};
    [SerializeField] private KeyCode[] moveDownKeys = {KeyCode.S, KeyCode.DownArrow, KeyCode.K};
    [SerializeField] private KeyCode[] moveLeftKeys = {KeyCode.A, KeyCode.LeftArrow, KeyCode.J};
    [SerializeField] private KeyCode[] moveRightKeys = {KeyCode.D, KeyCode.RightArrow, KeyCode.L};

    // References
    [SerializeField] private Transform NPCs;
    private Rigidbody2D _rb;
    private float _startTimer;
    private float _endTimer;
    private Vector2 _lastMove;
    private Transform _sprite;
    private float _jumpTimer = 0f;
    private bool _jump = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>().transform;
    }

    private void FixedUpdate()
    {
        GameManager GM = GameManager.Instance;
        var inputMode = GM.currentInputMode;
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
        
        if (GM.pianoEnabled || GM.talkingWith || GM.inventoryEnabled) moveVelocity = Vector2.zero;

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

            _jump = true;
        }

        HandleInteraction();
        _rb.linearVelocity *= baseSpeed + (HandleAnimation(moveVelocity) * jumpSpeedInfluence); // Jumping also influences speed
        
    }

    private void HandleInteraction()
    {
        GameManager gm = GameManager.Instance;
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

        gm.closestNPC = null;
        if (!(closestDistance < interactionDistance) || closestNPC is null || closestNPC == gm.talkingWith) return;
        gm.closestNPC = closestNPC;

        if (gm.inventoryEnabled || gm.pianoEnabled) return;
        NPCManager manager = closestNPC.GetComponent<NPCManager>();
        foreach (var key in gm.confirmKeys.Where(Input.GetKeyDown))
        {
            manager.Talk();
            gm.talkingWith = closestNPC;
        }
    }

    private float HandleAnimation(Vector2 move)
    {
        if (!_jump) return 0f;
        _jumpTimer += Time.deltaTime / jumpTime;
        float y = jumpCurve.Evaluate(_jumpTimer);
        _sprite.localPosition = new Vector2(_sprite.localPosition.x, y * jumpHeight);
        
        if (!(_jumpTimer >= 1f)) return y;
        if (move == Vector2.zero) _jump = false;
        _jumpTimer = 0f;
        return y;
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
