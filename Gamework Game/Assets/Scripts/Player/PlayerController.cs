using System;
using Player.RopeMechanics;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] GameObject weaponArmGO;
        IPlayerInput input;
        IPlayerPhysics physics;
        IPlayerArm weaponArm;
        IRopeHand ropeHand;

        void Awake()
        {
            input = GetComponent<IPlayerInput>();
            physics = GetComponent<IPlayerPhysics>();
            weaponArm = weaponArmGO.GetComponent<IPlayerArm>();
            ropeHand = GetComponent<IRopeHand>();
        }


        void Update()
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

            physics.UpdateFaceRender();
            physics.SetEnable(ropeHand.State == RopeState.Disconnected);
            physics.Move(input.MoveDir());
            
        }
        
        
        
        
    }
}