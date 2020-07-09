using System;
using System.Collections;
using UnityEngine;

namespace Player.RopeMechanics
{
    public enum RopeState
    {
        Shooting,
        Hang,
        Disconnected
    }
    public interface IRopeHand
    {
        void Shoot(Vector2 direction);
        RopeState State { get; }
        void Detach();
    }

    public class PlayerRopeHand : MonoBehaviour , IRopeHand
    {
        public float angleFromUpToThrowRope = 45;
        public float minDistance = 1;
        public float ropeLength = 20;
        public float ropeSpeed = 20;
        public Transform ropeArm;
        public RopeRender ropeRender;
        public Transform shootPoint;

        Vector2 targetOffset; // targetSpace
        Rigidbody2D target;
        
        public LayerMask wallsMask;

        SpringJoint2D currentJoint;

        Rigidbody2D body;
        RaycastHit2D hit;

        Vector2 endRope;

        public Hand hand;
        

        void Start()
        {
            body = GetComponent<Rigidbody2D>();
        }

        public RopeState State { get; set; } = RopeState.Disconnected;

        bool ValidateDirection(Vector2 direction)
        {
            direction.Normalize();
            return direction.y > Mathf.Sin(angleFromUpToThrowRope * Mathf.Deg2Rad);
        }
        
        
        public void Shoot(Vector2 direction)
        {
            if (!ValidateDirection(direction))
            {
                return;
            }
            Clear(); // para evitar bugs
            hit = Physics2D.Raycast(shootPoint.position, direction, ropeLength, wallsMask);
            
            var hitSomethingToHookOnto = hit.collider != null && hit.rigidbody != null && Vector2.Distance(hit.point, transform.position) > minDistance;

            if (hitSomethingToHookOnto)
            {
                SetConnection();
                endRope = shootPoint.position;
                State = RopeState.Shooting;
            }
            else
            {
                State = RopeState.Disconnected;
            }
        }


        void OnGUI()
        {
            var rect = new Rect(10, 10, 150, 100);
            GUI.Label(rect, "Rope State : "+State);
        }


        void OnConnectRope()
        {
            SetConnection();
            State = RopeState.Hang;
            // currentJoint = gameObject.AddComponent<SpringJoint2D>();
            // currentJoint.distance = Vector2.Distance(transform.position, hit.point);
            // currentJoint.autoConfigureDistance = false;
            // currentJoint.autoConfigureConnectedAnchor = false;
            // currentJoint.connectedAnchor = hit.point;
        }

        void SetConnection()
        {
            target = hit.rigidbody;
            targetOffset = hit.rigidbody.transform.InverseTransformPoint(hit.point);
            SetHandRotation();
        }


        void LateUpdate()
        {
            var pointPosition = shootPoint.position;
            if (State == RopeState.Hang)
            {
                Vector2 pos = transform.position;
                // currentJoint.connectedAnchor = GetTargetWorld;
                // var dt = Time.deltaTime;
                // currentJoint.distance = Mathf.MoveTowards(currentJoint.distance, 0, ropeSpeed * dt);
                var toTarget = GetTargetWorld - pos;
                body.velocity = toTarget.normalized * ropeSpeed;
                ropeRender.DrawRope(pointPosition, GetTargetWorld);
                if (toTarget.magnitude < minDistance)
                {
                    Clear();
                }
            }

            if (State == RopeState.Shooting)
            {
                endRope = Vector2.MoveTowards(endRope, GetTargetWorld, Time.deltaTime * ropeSpeed * 2);
                if (Vector2.Distance(GetTargetWorld, endRope) < 0.1)
                {
                    endRope = GetTargetWorld;
                    OnConnectRope();
                }
                ropeRender.DrawRope(pointPosition, endRope);
            }
        }

        Vector2 GetTargetWorld => target.transform.TransformPoint(targetOffset);

        void Update()
        {
            if (State == RopeState.Hang || State == RopeState.Shooting)
            {
                SetHandRotation();
            }
        }

        void SetHandRotation()
        {
            Vector2 pointPosition = shootPoint.position;
            ropeArm.up = GetTargetWorld - pointPosition;
        }


        void Clear()
        {
            hand.Release();
            ropeRender.HideRope();
            if(currentJoint)
            {
                Destroy(currentJoint);
            }
            State = RopeState.Disconnected;
        }
        

        public void Detach()
        {
            Clear();
        }
    }
}
