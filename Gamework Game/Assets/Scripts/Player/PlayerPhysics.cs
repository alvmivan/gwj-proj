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
        Vector2 Velocity { get; }
        void SetEnable(bool enable);
        bool IsGrounded();
        void UpdateFaceRender();
    }


    internal class PlayerPhysics : MonoBehaviour, IPlayerPhysics
    {
        //inspector fields
        [Header("Jump")] public float jumpVelocity = 5f;
        public float cacheJumpTime = 0.2f;
        public float coyoteTime = 0.2f;
        public int maxAirJumps = 1;
        public PhysicArea2D feetArea;
        [Space(20), Header("Run")] public float airAcceleration = 5;
        public float groundAcceleration = 10;
        public float breakAcceleration = 20;
        public float maxSpeed = 10;
        public Transform renderTransform;

        //variables
        Rigidbody2D body;
        VarTimeline<bool> isGrounded;
        VarTimeline<bool> jump;
        int currentAirJumps;
        float inputDirection;

        void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            isGrounded = new VarTimeline<bool>();
            jump = new VarTimeline<bool>();
        }

        bool IsLogicallyGrounded()
        {
            return isGrounded.Value && isGrounded.StillValid(coyoteTime);
        }

        bool WannaJump()
        {
            return jump.Value && jump.StillValid(cacheJumpTime);
        }

        bool CanDoSecondJump()
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

        public Vector2 Velocity => body.velocity;


        public void SetEnable(bool enable)
        {
            enabled = enable;
        }

        public bool IsGrounded()
        {
            return IsLogicallyGrounded();
        }

        void Start()
        {
            ResetAirJumps();
        }

        void Update()
        {
            UpdateGrounded();
            UpdateJump();
            UpdateFaceRender();
        }

        public void UpdateFaceRender()
        {
            if (Mathf.Abs(Velocity.x) < 0.01) return;
            var scale = renderTransform.localScale;
            scale.x = Mathf.Sign(Velocity.x);
            renderTransform.localScale = scale;

        }

        void FixedUpdate()
        {
            var dt = Time.fixedDeltaTime;
            UpdateRun(dt);
            
        }


        void UpdateRun(float dt)
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

        void UpdateJump()
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

        void PerformAirJump()
        {
            currentAirJumps--;
            PerformJump();
        }

        void ResetAirJumps()
        {
            currentAirJumps = maxAirJumps;
        }

        void UpdateGrounded()
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

        bool IsPhysicallyGrounded()
        {
            return feetArea.CheckAny();
        }

        void PerformJump()
        {
            jump.Value = false;
            isGrounded.Value = false;
            var velocity = body.velocity;
            velocity.y = jumpVelocity;
            body.velocity = velocity;
        }
    }
}