using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Player
{
    [Serializable] class InputSettings
    {
        public KeyCode[] jumpKey = {KeyCode.Space};
        public KeyCode[] leftKey = {KeyCode.LeftArrow, KeyCode.A};
        public KeyCode[] rightKey = {KeyCode.RightArrow, KeyCode.D};
        public KeyCode[] downKey = {KeyCode.DownArrow, KeyCode.S};
        public KeyCode[] upKey = {KeyCode.UpArrow, KeyCode.W};
    }

    public static class InputExtensions
    {
        public static bool AnyGetKey(this KeyCode[] keys)
        {
            var length = keys.Length;
            for (var i = 0; i < length; i++)
            {
                if (Input.GetKey(keys[i]))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool AnyGetKeyDown(this KeyCode[] keys)
        {
            var length = keys.Length;
            for (var i = 0; i < length; i++)
            {
                if (Input.GetKeyDown(keys[i]))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool AnyGetKeyUp(this KeyCode[] keys)
        {
            var length = keys.Length;
            for (var i = 0; i < length; i++)
            {
                if (Input.GetKeyUp(keys[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static float AsAxis(this bool value) => value ? 1 : 0;
    }
    
    public interface IPlayerInput
    {
        bool PressJump();
        float MoveDir();
        bool GoDown();

        bool ReleaseJump();
    }
    
    public class PlayerInput : MonoBehaviour, IPlayerInput
    {
        [SerializeField] private InputSettings settings;
        
        

        public bool PressJump()
        {
            return settings.jumpKey.AnyGetKeyDown();
        }

        public float MoveDir()
        {
            return settings.rightKey.AnyGetKeyDown().AsAxis() - settings.leftKey.AnyGetKeyDown().AsAxis();
        }

        public bool GoDown()
        {
            return settings.downKey.AnyGetKey();
        }

        public bool ReleaseJump()
        {
            return settings.jumpKey.AnyGetKeyUp();
        }
    }
}
