using System;
using UnityEngine;

namespace Utils
{
    public class PhysicArea2D : MonoBehaviour
    {
        // por ahora lo implemento como circulo
        
        public LayerMask layerMask = -1;
        public float radius = 1;
        private Collider2D[] results = new Collider2D[128];


        public bool SearchFor<TComponent>(out TComponent component) where  TComponent : Component
        {
            var count = Physics2D.OverlapCircleNonAlloc(transform.position, radius, results, layerMask);
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


        public bool SearchForAny()
        {
            return Physics2D.OverlapCircleNonAlloc(transform.position, radius, results, layerMask) > 0;
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(.1f,.8f,.8f);
            if (SearchForAny())
            {
                Gizmos.color = new Color(.8f,.8f,.1f);
            }
            Gizmos.DrawWireSphere(transform.position, radius);
        }
#endif
    }
}