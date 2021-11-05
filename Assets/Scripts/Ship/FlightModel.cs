using System;
using System.Collections.Generic;

namespace Ship
{
    public static class FlightModel
    {
        public static void StoreCustomFlightModel(ShipMovementHandler smh)
        {
            foreach (var mode in modes)
            {
                if (!mode.name.Equals("Custom")) continue;

                var m = mode.mode;
                m.RotPitch = smh.pitchSpeed;
                m.RotRoll = smh.rollSpeed;
                m.RotYaw = smh.yawSpeed;
                m.accForward = smh.accelerationForwards;
                m.accBackwards = smh.accelerationBackwards;
                m.accLateral = smh.accelerationLateral;
                m.maxSpeed = smh.maxSpeed;
                m.maxBoost = smh.maxSpeedBoost;
                m.stabilizationFactor = smh.stabilizationMultiplier;
            }
        }

        public static void NextFlightModel(ShipMovementHandler smh)
        {
            var newMode = modes[0].mode;
            for (int idx = 0; idx < modes.Length; idx++)
            {
                if(!modes[idx].name.Equals(smh.currentFlightModel)) continue;
                if(idx + 1 == modes.Length) break;

                newMode = modes[idx + 1].mode;
            }
            
            SetFlightModel(smh, newMode);
        }

        private static void SetFlightModel(ShipMovementHandler smh, Mode newMode)
        {
            smh.pitchSpeed = newMode.RotPitch;
            smh.rollSpeed = newMode.RotRoll;
            smh.yawSpeed = newMode.RotYaw;
            smh.accelerationForwards = newMode.accForward;
            smh.accelerationBackwards = newMode.accBackwards;
            smh.accelerationLateral = newMode.accLateral;
            smh.maxSpeed = newMode.maxSpeed;
            smh.maxSpeedBoost = newMode.maxBoost;
            smh.stabilizationMultiplier = newMode.stabilizationFactor;
        }

        private static readonly (string name, Mode mode)[] modes =
        {
            ("Custom", new Mode
            {
                RotPitch = 0,
                RotRoll = 0,
                RotYaw = 0,
                accForward = 0,
                accBackwards = 0,
                accLateral = 0,
                maxSpeed = 0,
                maxBoost = 0,
                stabilizationFactor = 0
            }),
            ("Normal", new Mode
            {
                RotPitch = 0.3f,
                RotRoll = 0.2f,
                RotYaw = 0.2f,
                accForward = 40,
                accBackwards = 35,
                accLateral = 30,
                maxSpeed = 150,
                maxBoost = 100,
                stabilizationFactor = 2
            }),
            ("Hyper", new Mode
            {
                RotPitch = 0.5f,
                RotRoll = 0.5f,
                RotYaw = 0.3f,
                accForward = 50,
                accBackwards = 45,
                accLateral = 40,
                maxSpeed = 150,
                maxBoost = 75,
                stabilizationFactor = 200
            })
        };

        private class Mode
        {
            public float RotPitch;
            public float RotRoll;
            public float RotYaw;

            public float accForward;
            public float accBackwards;
            public float accLateral;

            public float maxSpeed;
            public float maxBoost;
            public float stabilizationFactor;
        }
    }
}