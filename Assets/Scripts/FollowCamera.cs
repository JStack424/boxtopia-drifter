using System;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Player followObj;
    private void FixedUpdate()
    {
        Vector2 followPos = followObj.transform.position;
        var goalPos = new Vector3(followPos.x, followPos.y, -15);
        var currPos = transform.position;
        var newPos = Vector3.Lerp(currPos, goalPos, Time.fixedDeltaTime * 2);
        transform.position = newPos;

        var cam = GetComponent<Camera>();
        var trailLength = Math.Min(followObj.GetTrailLength(), 10);
        var velocity = followObj.GetComponent<Rigidbody2D>().velocity;
        var goalOrtho = 10 + 0.5f * trailLength + 0.1f * velocity.magnitude;
        var currOrtho = cam.orthographicSize;
        cam.orthographicSize = Mathf.Lerp(currOrtho,goalOrtho, Time.fixedDeltaTime);
    }
}
