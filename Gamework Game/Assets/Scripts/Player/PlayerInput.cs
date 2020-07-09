using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Player
{
    [Serializable]
    internal class InputSettings
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
        Vector2 WorldMouse();
        bool PressRopeShoot();
        Vector2 MouseDirection(Vector2 worldPoint);
        bool ReleaseRopeShoot();
    }
    
    public class PlayerInput : MonoBehaviour, IPlayerInput
    {
        [SerializeField] InputSettings settings;
        IPlayerCamera playerCamera;
        const int RopeMouseButton = 1;

        Plane gamePlane = new Plane(Vector3.forward, 0);

        void Start()
        {
            playerCamera = GetComponentInChildren<IPlayerCamera>();
        }

        public Vector2 WorldMouse()
        {
            var mouseRay = playerCamera.Camera.ScreenPointToRay(Input.mousePosition);
            gamePlane.Raycast(mouseRay, out var dist);
            return mouseRay.GetPoint(dist);
        }
        
        public Vector2 MouseDirection(Vector2 worldPoint)
        {
            Vector2 screenSrc = playerCamera.Camera.WorldToScreenPoint(worldPoint);
            Vector2 screenEnd = Input.mousePosition;
            return (screenEnd - screenSrc).normalized;
        }
        

        public bool PressRopeShoot()
        {
            return Input.GetMouseButtonDown(RopeMouseButton);
        }
        public bool ReleaseRopeShoot()
        {
            return Input.GetMouseButtonUp(RopeMouseButton);
        }


        public bool PressJump()
        {
            return settings.jumpKey.AnyGetKeyDown();
        }

        public float MoveDir()
        {
            return settings.rightKey.AnyGetKey().AsAxis() - settings.leftKey.AnyGetKey().AsAxis();
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
