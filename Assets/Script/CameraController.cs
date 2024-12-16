using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;

    [SerializeField]
    private Vector3 offset = new Vector3();

    [SerializeField]
    private float pitch = 2f;

    private int currentAngleIndex = 0;
    private Vector3[] offsets;

    private void Start()
    {
        offsets = new Vector3[]
        {
            offset,
            new Vector3(offset.z, offset.y, -offset.x),
            new Vector3(-offset.x, offset.y, -offset.z),
            new Vector3(-offset.z, offset.y, offset.x)
        };
    }

    private void LateUpdate()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 currentOffset = offsets[currentAngleIndex];
        transform.position = target.position - currentOffset;
        transform.LookAt(target.position + Vector3.up * pitch);
    }

    public void RotateCamera()
    {
        currentAngleIndex = (currentAngleIndex + 1) % offsets.Length;
    }
}
