using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Utils;

namespace Player
{
    public interface IPlayerCamera
    {
        Camera Camera { get; }
    }

    internal class PlayerCameraManager : MonoBehaviour, IPlayerCamera
    {
        [SerializeField] private Camera cam;

        private void Reset()
        {
            cam = Camera.main;
        }

        private void Start()
        {
            cam = Camera.main;
            var constraint = cam.gameObject.GetOrCreate<PositionConstraint>();
            var tr = transform;
            constraint.constraintActive = false;
            constraint.translationOffset = cam.transform.position - tr.position;
            constraint.SetSources(new List<ConstraintSource>{new ConstraintSource{sourceTransform = tr, weight = 1}});
            constraint.constraintActive = true;
        }

        public Camera Camera => cam;
    }
}
