using System;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private IPlayerInput input;
        private IPlayerPhysics physics;
        
        private void Awake()
        {
            input = GetComponent<IPlayerInput>();
            physics = GetComponent<IPlayerPhysics>();
        }


        private void Update()
        {
            if (input.PressJump())
            {
                physics.Jump();
            }

            if (input.ReleaseJump())
            {
                physics.CutJump();
            }
            physics.Move(input.MoveDir());
        }
        
        
        
        
    }
}