using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 6.5f;

    private PlayerInputActions input;
    private Rigidbody rb;
    private bool isGrounded = true;
    private bool jumpRequested;

    private void Awake()
    {
        Debug.Log("Awake");
        input = new PlayerInputActions();
        Debug.Log(input);
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }


    private void FixedUpdate()
    {
        Vector2 moveInput = input.Player.Move.ReadValue<Vector2>();

        // プレイヤーの向きを基準に移動方向を作成
        Vector3 moveDirection =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        moveDirection.Normalize();

        float currentSpeed = input.Player.Dash.IsPressed() ? runSpeed : speed;
        
        rb.linearVelocity = new Vector3(
            moveDirection.x * currentSpeed,
            rb.linearVelocity.y,
            moveDirection.z * currentSpeed
        );

        if (jumpRequested)
        {
            jumpRequested = false;
            isGrounded = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    private void Update()
    {
        if (input.Player.Jump.IsPressed() && isGrounded)
        {
            jumpRequested = true;
        }
    }
}