using System;

namespace Ship
{
    public static class FlightModel
    {
        public static event EventHandler<FlightModelChangedEventArgs> FlightModelChangedEvent;
        
        public static void StoreCustomFlightModel(ShipMovementHandler2Settings smh)
        {
            foreach (var (name, mode) in Modes)
            {
                if (!name.Equals("Custom")) continue;

                mode.RotPitch = smh.PitchSpeed;
                mode.RotRoll = smh.RollSpeed;
                mode.RotYaw = smh.YawSpeed;
                mode.AccForward = smh.AccelerationForwards;
                mode.AccBackwards = smh.AccelerationBackwards;
                mode.AccLateral = smh.AccelerationLateral;
                mode.MaxSpeed = smh.MaxSpeed;
                mode.MaxBoost = smh.MaxSpeedBoost;
                mode.StabilizationFactor = smh.StabilizationMultiplier;
            }
        }

        public static void LoadFlightModel(ShipMovementHandler2Settings smh, string modelName)
        {
            foreach (var mode in Modes)
            {
                if(!mode.name.Equals(modelName)) continue;
                
                SetFlightModel(smh, mode);
                break;
            }
        }

        public static void NextFlightModel(ShipMovementHandler2Settings smh)
        {
            var newMode = Modes[0];
            for (var idx = 0; idx < Modes.Length; idx++)
            {
                if(!Modes[idx].name.Equals(smh.ProfileName)) continue;
                if(idx + 1 == Modes.Length) break;
                
                newMode = Modes[idx + 1];
            }
            
            SetFlightModel(smh, newMode);
        }

        private static void SetFlightModel(ShipMovementHandler2Settings smh, (string name, Mode mode) newMode)
        {
            smh.ApplyNewProfile(newMode.mode, newMode.name);

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

        internal class Mode
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