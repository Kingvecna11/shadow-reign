using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _sprite;
    private Vector2 _moveInput = Vector2.zero;
    [SerializeField] private float maxSpeed = 10;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 60f;

    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float lowJumpMultiplier = 3f;
    [SerializeField] private float fallMultiplier = 4f;
    private bool isHoldingJump = false;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundRadius = 0.05f;

    // TODO: split jump into 2 animations. one for the initial jump and one for falling
    string animationState = "player_jump";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_rb.linearVelocity.y < 0)
        {
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (_rb.linearVelocity.y > 0 && !isHoldingJump)
        {
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        // TODO: fix getting stuck on walls due to collision. the player should slide down walls.
        float targetSpeed = _moveInput.x * maxSpeed;
        float speedDiff = targetSpeed - _rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = accelRate * speedDiff * Time.fixedDeltaTime;

        switch (_animator.GetBool("Attack"))
        {
            case true:
                _rb.linearVelocity = Vector2.zero;
                break;
            case false:
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x + movement, _rb.linearVelocity.y);
                break;
        }
        //animationState = (_moveInput.x, IsGrounded(), isAttacking) switch
        //{
        //    (_, _, true) => "player_light1",
        //    (_, false, _) => "player_jump",
        //    (0.0f, _, _) => "player_idle",
        //    (> 0.0f or < 0.0f, _, _) => "player_run",
        //    _ => "player_idle"
        //};
        //Debug.Log($"playing {animationState}");
        //_animator.Play(animationState);

        switch (_moveInput.x)
        {
            case (< 0.0f):
                _sprite.flipX = true;
                _animator.SetBool("Running", true);
                break;
            case (> 0.0f):
                _sprite.flipX = false;
                _animator.SetBool("Running", true);
                break;
            case (0.0f):
                _animator.SetBool("Running", false);
                break;
        }

        _animator.SetBool("Jumping", !IsGrounded());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && IsGrounded())
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
        }

        if (context.performed)
        {
            isHoldingJump = true;
        }

        if (context.canceled)
        {
            isHoldingJump = false;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        // TODO: make movement stop when attacking. add attack chain. prevent player from spamming attack
        if (context.started && IsGrounded())
        {
            _animator.SetBool("Attack", true);
        }
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

}
