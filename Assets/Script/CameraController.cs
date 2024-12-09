using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;

    [SerializeField]
    private Vector3 offset = new Vector3();

    [SerializeField]
    private float pitch = 2f;

    private void LateUpdate()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = target.position - offset;
        transform.LookAt(target.position + Vector3.up * pitch);

    }
}
