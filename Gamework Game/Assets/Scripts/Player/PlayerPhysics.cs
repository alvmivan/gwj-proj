using System;
using UnityEngine;
using Utils;

namespace Player
{
    public interface IPlayerPhysics
    {
        void Jump();
        void CutJump();
        void Move(float direction);
    }


    internal class PlayerPhysics : MonoBehaviour, IPlayerPhysics
    {
        //inspector fields
        [Header("Jump")]
        public float jumpVelocity = 5f;
        public float cacheJumpTime = 0.2f;
        public float coyoteTime = 0.2f;
        public int maxAirJumps = 1;
        public PhysicArea2D feetArea;
        [Space(20), Header("Run")] 
        public float airAcceleration = 5;
        public float groundAcceleration = 10;
        public float breakAcceleration = 20;
        public float maxSpeed = 10;
        
        //variables
        private Rigidbody2D body;
        private VarTimeline<bool> isGrounded;
        private VarTimeline<bool> jump;
        private int currentAirJumps;
        private float inputDirection;
        
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

        public AnimationCurve debugInputDirection;
        public void Move(float direction)
        {
            inputDirection = direction;
            debugInputDirection.AddKey(Time.time, inputDirection);
        }

        private void Start()
        {
            ResetAirJumps();
        }

        private void Update()
        {
            UpdateGrounded();
            UpdateJump();
        }

        private void FixedUpdate()
        {
            var dt = Time.fixedDeltaTime;
            UpdateRun(dt);
        }

        private void UpdateRun(float dt)
        {
            var velocity = body.velocity;
            var desiredSpeed = maxSpeed * inputDirection;
            var wannaIncreaseSpeed = Mathf.Abs(inputDirection) > 0 && desiredSpeed * velocity.x > 0;
            // en el piso se mueve normal
            // en el aire con menos aceleracion
            // si soltas el correr se frena un poco mas rapido
            float currentAcceleration;
            if (wannaIncreaseSpeed)
            {
                currentAcceleration = IsLogicallyGrounded() ? groundAcceleration : airAcceleration;
            }
            else
            {
                currentAcceleration = breakAcceleration;
            }
            velocity.x = Mathf.MoveTowards(velocity.x, desiredSpeed, dt * currentAcceleration);
            body.velocity = velocity;
        }

        private void UpdateJump()
        {
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
                        PerformAirJump();
                    }
                }
            }
        }

        private void PerformAirJump()
        {
            currentAirJumps--;
            PerformJump();
        }

        private void ResetAirJumps()
        {
            currentAirJumps = maxAirJumps;
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
            return feetArea.CheckAny();
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