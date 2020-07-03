using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class Clock
    {
        public static float Now() => Time.time;
    }
    public struct Stamp<T>
    {
        public T Value;
        public float Time;

        public static implicit operator Stamp<T>(T t)
        {
            return new Stamp<T>
            {
                Value = t,
                Time = Clock.Now()
            };
        }
    }
    
    public class VarTimeline<T> 
    {
     
        private readonly List<Stamp<T>> timeline;

        public VarTimeline(int cacheSize = 2, T initialValue = default)
        {
            timeline = new List<Stamp<T>>(cacheSize) {initialValue};
        }

        private void RotateListAndAppend(Stamp<T> elem)
        {
            var lastIndex = timeline.Count -1;
            for (var i = 0; i < lastIndex; i++)
            {
                timeline[i] = timeline[i + 1];
            }
            timeline[lastIndex] = elem;
        }

        public IReadOnlyList<Stamp<T>> Timeline => timeline;

        /// <summary>
        /// cuando lo asignaron
        /// </summary>
        public float ValueMoment => timeline.Last().Time;
        
        /// <summary>
        ///hace cuanto lo asignaron
        /// </summary>
        public float ValueTime => Clock.Now() - timeline.Last().Time;
    
        public T Value
        {
            get => timeline.Last().Value;
            set
            {
                if (FullCache())
                {
                    RotateListAndAppend(value);
                }
                else
                {
                    timeline.Add(value);
                }            
            }
        }

        private bool FullCache()
        {
            return timeline.Capacity == timeline.Count;
        }
    }
}