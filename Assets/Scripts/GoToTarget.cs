using UnityEngine;
using UnityEngine.InputSystem;

public class GoToTarget : MonoBehaviour
{
    public Transform target;

    public float height = 0f;
    public float distance = 0f;
    public float sideOffset = 0f;

    void LateUpdate()
    {
        Vector3 offset = new Vector3(
            sideOffset,
            height,
            -distance
        );

        transform.position = target.position + offset;
    }
}