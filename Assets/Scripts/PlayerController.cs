using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
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
        float targetSpeed = _moveInput.x * maxSpeed;
        float speedDiff = targetSpeed - _rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = accelRate * speedDiff * Time.fixedDeltaTime;

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x + movement, _rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
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
}
