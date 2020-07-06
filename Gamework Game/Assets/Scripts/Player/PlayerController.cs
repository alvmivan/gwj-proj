using System;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameObject weaponArmGO;
        private IPlayerInput input;
        private IPlayerPhysics physics;
        private IPlayerArm weaponArm;
        
        private void Awake()
        {
            input = GetComponent<IPlayerInput>();
            physics = GetComponent<IPlayerPhysics>();
            weaponArm = weaponArmGO.GetComponent<IPlayerArm>();
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
            
            weaponArm.AimTo(input.WorldMouse());
            
            
        }
        
        
        
        
    }
}