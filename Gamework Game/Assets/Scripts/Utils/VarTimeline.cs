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
        Stamp<T> last;
        public VarTimeline(T initialValue = default) => last = initialValue;
        public T Value { get => last.Value; set => last = value; }

        public bool StillValid(float validityTime)
        {
            return Clock.Now() <= validityTime + last.Time;
        }
    }
}