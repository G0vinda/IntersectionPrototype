using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private Transform _follow;
    private float? _fixedXPosition;

    public void Follow(Transform newFollowTarget, float? fixedXPosition = null)
    {
        _follow = newFollowTarget;
        _fixedXPosition = fixedXPosition;
    }

    void Update()
    {
        if(_follow == null)
            return;

        var followPosition = _fixedXPosition != null ? new Vector3(_fixedXPosition.Value, _follow.position.y, _follow.position.z) : _follow.position;
        transform.position = followPosition;               
    }
}
