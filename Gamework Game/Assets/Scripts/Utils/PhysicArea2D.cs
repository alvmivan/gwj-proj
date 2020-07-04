using System;
using UnityEngine;

namespace Utils
{
        public delegate void TriggerHandler(Collider2D collider);
    public abstract class PhysicArea2D : MonoBehaviour
    {
        private void OnValidate() => Init();
        private void Start() => Init();
        private void Reset() => Init();
        protected virtual void Init()
        {
            gameObject.layer = 31;
        }
        
        private protected readonly Collider2D[] results = new Collider2D[128];
        
        [SerializeField] private protected LayerMask layerMask = -1;
        public abstract bool Check<TComponent>(out TComponent component) where  TComponent : Component;
        public abstract bool CheckAny();
        public event TriggerHandler OnEnter;
        public event TriggerHandler OnExit;
        public event TriggerHandler OnStay;


        private void OnTriggerEnter2D(Collider2D other)
        {
            OnEnter?.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            OnExit?.Invoke(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            OnStay?.Invoke(other);
        }
    }
}