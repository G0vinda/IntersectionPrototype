using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private Transform _follow;

    public void Follow(Transform newFollowTarget)
    {
        _follow = newFollowTarget;
    }

    void Update()
    {
        if(_follow == null)
            return;

        transform.position = _follow.position;    
    }
}
