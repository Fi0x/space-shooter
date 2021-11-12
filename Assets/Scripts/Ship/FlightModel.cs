using System;

namespace Ship
{
    public static class FlightModel
    {
        public static event EventHandler<FlightModelChangedEventArgs> FlightModelChangedEvent;
        
        public static void StoreCustomFlightModel(ShipMovementHandler smh)
        {
            foreach (var mode in Modes)
            {
                if (!mode.name.Equals("Custom")) continue;

                var m = mode.mode;
                m.RotPitch = smh.pitchSpeed;
                m.RotRoll = smh.rollSpeed;
                m.RotYaw = smh.yawSpeed;
                m.AccForward = smh.accelerationForwards;
                m.AccBackwards = smh.accelerationBackwards;
                m.AccLateral = smh.accelerationLateral;
                m.MaxSpeed = smh.maxSpeed;
                m.MaxBoost = smh.maxSpeedBoost;
                m.StabilizationFactor = smh.stabilizationMultiplier;
            }
        }

        public static void LoadFlightModel(ShipMovementHandler smh, string modelName)
        {
            foreach (var mode in Modes)
            {
                if(!mode.name.Equals(modelName)) continue;
                
                SetFlightModel(smh, mode);
                break;
            }
        }

        public static void NextFlightModel(ShipMovementHandler smh)
        {
            var newMode = Modes[0];
            for (var idx = 0; idx < Modes.Length; idx++)
            {
                if(!Modes[idx].name.Equals(smh.currentFlightModel)) continue;
                if(idx + 1 == Modes.Length) break;
                
                newMode = Modes[idx + 1];
            }
            
            SetFlightModel(smh, newMode);
        }

        private static void SetFlightModel(ShipMovementHandler smh, (string name, Mode mode) newMode)
        {
            smh.pitchSpeed = newMode.mode.RotPitch;
            smh.rollSpeed = newMode.mode.RotRoll;
            smh.yawSpeed = newMode.mode.RotYaw;
            smh.accelerationForwards = newMode.mode.AccForward;
            smh.accelerationBackwards = newMode.mode.AccBackwards;
            smh.accelerationLateral = newMode.mode.AccLateral;
            smh.maxSpeed = newMode.mode.MaxSpeed;
            smh.maxSpeedBoost = newMode.mode.MaxBoost;
            smh.stabilizationMultiplier = newMode.mode.StabilizationFactor;

            smh.currentFlightModel = newMode.name;

            var eventArgs = new FlightModelChangedEventArgs
            {
                NewMaxSpeed = newMode.mode.MaxSpeed,
                NewBoostSpeed = newMode.mode.MaxBoost
            };
            FlightModelChangedEvent?.Invoke(null, eventArgs);
        }

        private static readonly (string name, Mode mode)[] Modes =
        {
            ("Custom", new Mode
            {
                RotPitch = 0,
                RotRoll = 0,
                RotYaw = 0,
                AccForward = 0,
                AccBackwards = 0,
                AccLateral = 0,
                MaxSpeed = 0,
                MaxBoost = 0,
                StabilizationFactor = 0
            }),
            ("Normal", new Mode
            {
                RotPitch = 1.2f,
                RotRoll = 0.2f,
                RotYaw = 0.8f,
                AccForward = 40,
                AccBackwards = 35,
                AccLateral = 30,
                MaxSpeed = 150,
                MaxBoost = 100,
                StabilizationFactor = 2
            }),
            ("Hyper", new Mode
            {
                RotPitch = 2f,
                RotRoll = 0.5f,
                RotYaw = 1.2f,
                AccForward = 50,
                AccBackwards = 45,
                AccLateral = 40,
                MaxSpeed = 150,
                MaxBoost = 75,
                StabilizationFactor = 200
            }),
            ("Dominik", new Mode
            {
                RotPitch = 2f,
                RotRoll = 0.5f,
                RotYaw = 1.2f,
                AccForward = 50,
                AccBackwards = 45,
                AccLateral = 40,
                MaxSpeed = 150,
                MaxBoost = 75,
                StabilizationFactor = 200
            }),
            ("Waldemar", new Mode
            {
                RotPitch = 1.2f,
                RotRoll = 0.2f,
                RotYaw = 0.8f,
                AccForward = 40,
                AccBackwards = 35,
                AccLateral = 20,
                MaxSpeed = 150,
                MaxBoost = 100,
                StabilizationFactor = 2
            }),
            ("Tobi", new Mode
            {
                RotPitch = 1.2f,
                RotRoll = 0.2f,
                RotYaw = 0.8f,
                AccForward = 40,
                AccBackwards = 35,
                AccLateral = 35,
                MaxSpeed = 150,
                MaxBoost = 100,
                StabilizationFactor = 2
            })
        };
        
        public class FlightModelChangedEventArgs : EventArgs
        {
            public float NewMaxSpeed;
            public float NewBoostSpeed;
        }

        private class Mode
        {
            public float RotPitch;
            public float RotRoll;
            public float RotYaw;

            public float AccForward;
            public float AccBackwards;
            public float AccLateral;

            public float MaxSpeed;
            public float MaxBoost;
            public float StabilizationFactor;
        }
    }
}