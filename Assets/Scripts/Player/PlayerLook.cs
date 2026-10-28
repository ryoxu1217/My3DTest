using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public float sensitivity = 0.2f;
    public float smoothSpeed = 15f;

    private float targetY;

    void Start()
    {
        targetY = transform.eulerAngles.y;
    }

    void Update()
    {
        float mouseX = Mouse.current.delta.ReadValue().x;

        targetY += mouseX * sensitivity;

        Quaternion targetRotation =
            Quaternion.Euler(0f, targetY, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            smoothSpeed * Time.deltaTime
        );
    }
}