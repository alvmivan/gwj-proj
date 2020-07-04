using System;
using UnityEngine;

namespace Player
{
    
    
    
    public class PlayerAnim : MonoBehaviour
    {
        [SerializeField] private PlayerPhysics playerPhysics;
        private IPlayerPhysics physics;
        public Animator faceAnimation;
        private static readonly int SpeedY = Animator.StringToHash("SpeedY");
        private static readonly int SpeedX = Animator.StringToHash("SpeedX");


        private void Start()
        {
            physics = playerPhysics;
        }

        private void Update()
        {
            var vel = physics.Velocity;
            var grounded = physics.IsGrounded();
            
            faceAnimation.SetFloat(SpeedX,vel.x);   
            faceAnimation.SetFloat(SpeedY,vel.y);
            
        }
        
        
    }
}
