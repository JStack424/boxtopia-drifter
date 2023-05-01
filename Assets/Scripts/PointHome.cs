using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointHome : MonoBehaviour
{
    public Transform followObj;

    private void Update()
    {
        transform.position = followObj.transform.position;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position * -1);
    }
}
