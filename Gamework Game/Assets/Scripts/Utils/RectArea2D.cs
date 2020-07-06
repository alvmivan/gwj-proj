using System;
using System.Data;
using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class RectArea2D : PhysicArea2D
    {
        public BoxCollider2D trigger;

        protected override void Init()
        {
            base.Init();
            trigger = GetComponent<BoxCollider2D>();
            trigger.isTrigger = true;
        }

        private ( Vector2, float) BoxAngle()
        {
            var angle = transform.rotation.eulerAngles.z;
            var size = transform.lossyScale * trigger.size;
            return (size, angle);
        }
        
        public override bool Check<TComponent>(out TComponent component)
        {
            Vector2 pos = transform.position;
            var (size, angle) = BoxAngle();
            var count = Physics2D.OverlapBoxNonAlloc( pos, size, angle, results, layerMask);
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
            Vector2 pos = transform.position;
            var (size, angle) = BoxAngle();
            return Physics2D.OverlapBoxNonAlloc(pos, size, angle, results, layerMask) > 0;
        }


        private void OnDrawGizmosSelected()
        {
            (Vector3 size, float angle) = BoxAngle();
            var rot = Quaternion.Euler(0, 0, angle);
            Gizmos.matrix = Matrix4x4.TRS(transform.position, rot, size+Vector3.forward);
            Gizmos.color = new Color(0.95f, 1f, 0.17f);
            if (CheckAny())
            {
                Gizmos.color = new Color(0.24f, 0.47f, 1f);
            }
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }
    }
}