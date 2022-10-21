using System;
using UnityEngine;

namespace Helper
{
    [Serializable]
    public class PID
    {
        public float kP, kI, kD;
        
        [SerializeField] private float p, i, d;
        [SerializeField] private float previousError;

        public PID(float p, float i, float d)
        {
            kP = p;
            kI = i;
            kD = d;
        }

        public float GetOutput(float currentError, float deltaTime)
        {
            p = currentError;
            i += p * deltaTime;
            d = (p - previousError) / deltaTime;
            previousError = currentError;
       
            return p*kP + i*kI + d*kD;
        }
    }
}