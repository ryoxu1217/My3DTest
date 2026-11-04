using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 6.5f;
    public float dashEnergyCost = 20f;
    private PlayerInputActions input;
    private Rigidbody rb;
    private PlayerEnergy energy;
    private bool isGrounded = true;
    private bool jumpRequested;

    private void Awake()
    {
        input = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        energy = GetComponent<PlayerEnergy>();
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

        bool dash =
        input.Player.Dash.IsPressed() &&
        energy.HasEnergy(dashEnergyCost * Time.fixedDeltaTime);

        if (dash)
        {
            energy.Consume(
                dashEnergyCost * Time.fixedDeltaTime
            );
        }

        float currentSpeed =
            dash ? runSpeed : speed;

        
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