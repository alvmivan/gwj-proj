using UnityEngine;

namespace Utils
{
    public static class GOExtensions
    {
        public static TComponent GetOrCreate<TComponent>(this GameObject go) where TComponent : Component
        {
            var component = go.GetComponent<TComponent>();
            return component ? component : go.AddComponent<TComponent>();
        }
    }
}