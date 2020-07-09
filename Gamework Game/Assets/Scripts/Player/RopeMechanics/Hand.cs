using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player.RopeMechanics
{

    public class Hand : MonoBehaviour
    {
        public PlayerRopeHand playerRopeHand;
        
        static readonly int Value = Animator.StringToHash("Value");
        
        Animator anim;
        List<int> values;

        void Start()
        {
            values = new List<int>{0,1,2};
            values[(int) RopeState.Disconnected] = 0;
            values[(int) RopeState.Hang] = 1;
            values[(int) RopeState.Shooting] = 2;
            anim = GetComponent<Animator>();
        }


        void Update()
        {
            anim.SetInteger(Value, values[(int) playerRopeHand.State]);            
        }
    }
}