using System;
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
        public float minDistance = 1;
        public float uselessTime = 0.25f;
        public float ropeLength = 20;
        public float ropeSpeed = 20;
        public RopeRender ropeRender;
        public Transform shootPoint;

        private Vector2 targetOffset; // targetSpace
        private Rigidbody2D target;
        
        public LayerMask wallsMask;

        private SpringJoint2D currentJoint;

        private Rigidbody2D body;

        private void Start()
        {
            body = GetComponent<Rigidbody2D>();
        }

        public RopeState State { get; set; }
        
        public void Shoot(Vector2 direction)
        {
            Clear(); // para evitar bugs
            var hit = Physics2D.Raycast(shootPoint.position, direction, ropeLength, wallsMask);
            
            var hitSomethingToHookOnto = hit.collider != null && hit.rigidbody != null && Vector2.Distance(hit.point, transform.position) > minDistance;
            
            if (hitSomethingToHookOnto)
            {
                State = RopeState.Shooting;
                ThrowRope(hit);
            }
            else
            {
                State = RopeState.Disconnected;
            }
        }

        private void ThrowRope(RaycastHit2D hit)
        {
            // todo : first the rope animation
            OnConnectRope(hit);
        }

        private void OnConnectRope(RaycastHit2D hit)
        {
            State = RopeState.Hang;
            currentJoint = gameObject.AddComponent<SpringJoint2D>();
            currentJoint.distance = Vector2.Distance(transform.position, hit.point);
            currentJoint.autoConfigureDistance = false;
            currentJoint.autoConfigureConnectedAnchor = false;
            currentJoint.connectedAnchor = hit.point;
            target = hit.rigidbody;
            targetOffset = hit.rigidbody.transform.InverseTransformPoint(hit.point);
            
        }


        private void LateUpdate()
        {
            if (State == RopeState.Hang)
            {
                Vector2 pos = transform.position;
                Vector2 targetWorld = target.transform.TransformPoint(targetOffset);
                currentJoint.connectedAnchor = targetWorld;
                var dt = Time.deltaTime;
                currentJoint.distance = Mathf.MoveTowards(currentJoint.distance, 0, ropeSpeed * dt);
                ropeRender.DrawRope(shootPoint.position, currentJoint.connectedAnchor);
                var toTarget = targetWorld - pos;
                //body.velocity = toTarget.normalized * ropeSpeed;
                if (toTarget.magnitude < minDistance)
                {
                    Clear();
                }
            }
        }


        private void Clear()
        {
            if(currentJoint)
            {
                ropeRender.HideRope();
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
