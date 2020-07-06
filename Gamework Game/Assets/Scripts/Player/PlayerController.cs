using System;
using Player.RopeMechanics;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameObject weaponArmGO;
        private IPlayerInput input;
        private IPlayerPhysics physics;
        private IPlayerArm weaponArm;
        private IRopeHand ropeHand;
        
        private void Awake()
        {
            input = GetComponent<IPlayerInput>();
            physics = GetComponent<IPlayerPhysics>();
            weaponArm = weaponArmGO.GetComponent<IPlayerArm>();
            ropeHand = GetComponent<IRopeHand>();
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
            
            
            
            
            // weaponArm.AimTo(input.WorldMouse());

            if (input.PressRopeShoot())
            {
                ropeHand.Shoot(input.MouseDirection(transform.position));
            }

            if (input.ReleaseRopeShoot())
            {
                ropeHand.Detach();   
            }


            physics.SetEnable(ropeHand.State != RopeState.Disconnected);
            physics.Move(input.MoveDir());
            
        }
        
        
        
        
    }
}