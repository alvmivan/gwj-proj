using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;


public interface IPlayerArm
{
    void AimTo(Vector2 point);
    Vector2 ShootPoint { get; }
}

class PlayerArm : MonoBehaviour , IPlayerArm
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform shootPoint;

    private Vector2 realDirection = Vector2.down + Vector2.right;
    private Vector2 renderedDirection;

    [SerializeField] private float minAngle = 1;


    public void AimTo(Vector2 point)
    {
        realDirection = (Vector3) point - transform.position;
    }
    
    private void FixedUpdate()
    {
        const float armDampRotation = 50f;
        
        var dt = Time.fixedDeltaTime;
        var currentRotation = transform.rotation;
        renderedDirection = currentRotation.Up();
        var needsRotation = renderedDirection.Dot(realDirection.normalized) < Mathf.Cos(minAngle * Mathf.Deg2Rad);
        if (needsRotation)
        {
            var desiredRotation = Quaternion.FromToRotation(Vector3.up, realDirection);
            transform.rotation = Quaternion.Slerp(currentRotation, desiredRotation, armDampRotation * dt);
        }
    }

    
    
    public Vector2 ShootPoint => shootPoint.position;
}