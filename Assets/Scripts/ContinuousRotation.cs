using UnityEngine;

public class ContinuousRotation : MonoBehaviour
{
    public float rotationSpeed;
    public Vector3 rotationAxis;
    private void Update()
    {
        transform.rotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, rotationAxis);
    }
}
