using System;
using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class CircleArea : PhysicArea2D
    {
        private CircleCollider2D trigger;

        protected override void Init()
        {
            base.Init();
            trigger = GetComponent<CircleCollider2D>();
            trigger.isTrigger = true;
        }

        private ( Vector2, float ) CenterRadius()
        {
            var localCenter = trigger.offset;
            var localRadius = trigger.radius;
            var localScale = transform.localScale;
            var worldCenter = transform.position + transform.rotation * (localCenter);
            var worldRadius = localRadius * Mathf.Min(Mathf.Abs(localScale.x), Mathf.Abs(localScale.y));
            return (worldCenter, worldRadius);
        }
        
        public override bool Check<TComponent>(out TComponent component)
        {
            var (center, radius) = CenterRadius();
            var end = center+Vector2.one.normalized*radius;
            Debug.DrawLine(center, end,Color.red,1);
            var count = Physics2D.OverlapCircleNonAlloc(center, radius, results, layerMask);
            for (var i = 0; i < count; ++i)
            {
                component = results[i].GetComponent<TComponent>();
                if (component)
                {
                    return true;
                }
            }
            component = null;
            return false;
        }


        public override bool CheckAny()
        {
            var (center, radius) = CenterRadius();
            return Physics2D.OverlapCircleNonAlloc(center, radius, results, layerMask) > 0;
        }

        private void OnDrawGizmosSelected()
        {
            var (center, radius) = CenterRadius();
            Gizmos.color = Color.yellow;
            if(CheckAny())
                Gizmos.color = Color.blue;
            Gizmos.DrawSphere(center,radius);
        }
    }
}