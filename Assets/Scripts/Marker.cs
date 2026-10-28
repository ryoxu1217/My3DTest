using UnityEngine;

public class Marker : MonoBehaviour
{
    public Transform marker;

    void Update()
    {
        marker.position = transform.position + transform.forward * 2f;
    }
}
