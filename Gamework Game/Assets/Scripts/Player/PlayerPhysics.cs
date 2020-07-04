using System;
using UnityEngine;
using Utils;

namespace Player
{
    public interface IPlayerPhysics
    {
        void Jump();
        void CutJump();
    }
    
    
    public class PlayerPhysics : MonoBehaviour, IPlayerPhysics
    {
        public float jumpVelocity = 5f;
        public float cacheJumpTime = 0.2f;
        public float coyoteTime = 0.2f;
        public int airJumps = 1;
        public PhysicArea2D feetArea;
        private Rigidbody2D body;

        private VarTimeline<bool> isGrounded;
        private VarTimeline<bool> jump;
        
        private int currentAirJumps;
        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            isGrounded = new VarTimeline<bool>();
            jump = new VarTimeline<bool>();
        }

        private bool IsLogicallyGrounded()
        {
            return isGrounded.Value && isGrounded.StillValid(coyoteTime);
        }

        private bool WannaJump()
        {
            return jump.Value && jump.StillValid(cacheJumpTime);
        }
        
        private bool CanDoSecondJump()
        {
            return currentAirJumps > 0;
        }
        
        public void Jump()
        {
            jump.Value = true;
        }

        public void CutJump()
        {
            var vel = body.velocity;
            if (vel.y > 0.01)
            {
                const float jumpReduction = 0.6f;
                vel.y *= jumpReduction;
                body.velocity = vel;
            }
        }

        private void Start()
        {
            ResetAirJumps();
        }

        private void Update()
        {
            UpdateGrounded();
            if (WannaJump())
            {
                if (IsLogicallyGrounded())
                {
                    PerformJump();
                }
                else
                {
                    if (CanDoSecondJump())
                    {
                        currentAirJumps--;
                        PerformJump();
                    }
                }    
            }
            
        }

        private void ResetAirJumps()
        {
            currentAirJumps = airJumps;
        }

        private void UpdateGrounded()
        {
            if (IsPhysicallyGrounded())
            {
                isGrounded.Value = true;
                ResetAirJumps();
            }
            if (!IsPhysicallyGrounded() && IsLogicallyGrounded())
            {
                isGrounded.Value = false;
            }
        }

        private bool IsPhysicallyGrounded()
        {
            return feetArea.SearchForAny();
        }

        private void PerformJump()
        {
            jump.Value = false;
            isGrounded.Value = false;
            var velocity = body.velocity;
            velocity.y = jumpVelocity;
            body.velocity = velocity;
        }
    }

    
}