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

        

        Rigidbody2D body;
        RaycastHit2D hit;

        Vector2 endRope;

        public Hand hand;
        bool ropeSaved;

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

            ropeSaved = false;
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


        #if UNITY_EDITOR
        void OnGUI()
        {
            var rect = new Rect(10, 10, 150, 100);
            GUI.Label(rect, "Rope State : "+ State);
        }
        #endif


        void OnConnectRope()
        {
            SetConnection();
            State = RopeState.Hang;
        }

        void SetConnection()
        {
            
            target = hit.rigidbody;
            targetOffset = hit.rigidbody.transform.InverseTransformPoint(hit.point);
            SetHandData();
        }


        void LateUpdate()
        {
            float handRotationSpeed = 90;
            var pointPosition = shootPoint.position;
            if (State == RopeState.Hang)
            {
                Vector2 pos = transform.position;
                // currentJoint.connectedAnchor = GetTargetWorld;
                // var dt = Time.deltaTime;
                // currentJoint.distance = Mathf.MoveTowards(currentJoint.distance, 0, ropeSpeed * dt);
                var toTarget = TargetWorld - pos;
                body.velocity = toTarget.normalized * ropeSpeed;
                ropeRender.DrawRope(pointPosition, TargetWorld);
                if (toTarget.magnitude < minDistance)
                {
                    Clear();
                }
                
                hand.transform.position = TargetWorld;
                handRotationSpeed = 90;
            }

            if (State == RopeState.Disconnected)
            {
                var handFar = Vector2.Distance(hand.transform.position, shootPoint.position) > 0.2;
                if (handFar && !ropeSaved)
                {
                    hand.transform.position = Vector2.MoveTowards(hand.transform.position, shootPoint.position, Time.deltaTime * ropeSpeed );
                    ropeRender.DrawRope(shootPoint.transform.position, hand.transform.position);
                }
                else
                {
                    hand.transform.position = shootPoint.position;
                    ropeRender.HideRope();
                    ropeSaved = true;
                }
                handRotationSpeed = 2;
            }
            
            
            
            if (State == RopeState.Shooting)
            {
                endRope = Vector2.MoveTowards(endRope, TargetWorld, Time.deltaTime * ropeSpeed * 2);
                if (Vector2.Distance(TargetWorld, endRope) < 0.1)
                {
                    endRope = TargetWorld;
                    OnConnectRope();
                }
                hand.transform.position = endRope;
                ropeRender.DrawRope(pointPosition, endRope);
                handRotationSpeed = 90;
            }
            hand.transform.rotation = Quaternion.RotateTowards(hand.transform.rotation,shootPoint.transform.rotation,Time.deltaTime * handRotationSpeed * 10);
        }

        Vector2 TargetWorld => target.transform.TransformPoint(targetOffset);

        void Update()
        {
            if (State == RopeState.Hang || State == RopeState.Shooting)
            {
                SetHandData();
            }
        }

        void SetHandData()
        {
            Vector2 pointPosition = shootPoint.position;
            var targetWorld = TargetWorld;
            ropeArm.up = targetWorld - pointPosition;
            
        }


        void Clear()
        {
            State = RopeState.Disconnected;
        }
        

        public void Detach()
        {
            Clear();
        }
    }
}
