using UnityEngine;

namespace LevelManagement
{
    [CreateAssetMenu(fileName = "OutOfLevelNotifierScriptableObject", menuName = "Level/OutOfLevelSO", order = 0)]

    public class OutOfLevelNotifierScriptableObject : ScriptableObject
    {
        [SerializeField] private Vector3 levelOrigin = Vector3.zero;
        [SerializeField] private float noiseStartDistance = 500;
        [SerializeField] private float warningDistance = 700;
        [SerializeField] private float connectionLostDistance = 900;

        public float GetCurrentDistance(OutOfLevelNotifiable self) 
            => Vector3.Distance(self.transform.position, levelOrigin);

        public enum OutOfLevelState
        {
            NoNoise,
            LowNoise,
            NoiseAndWarning,
            ConnectionLost
        }
        

        public OutOfLevelState GetCurrentState(OutOfLevelNotifiable self)
        {
            var distance = this.GetCurrentDistance(self);
            
            if (distance < noiseStartDistance)
            {
                return OutOfLevelState.NoNoise;
            }

            if (distance < warningDistance)
            {
                return OutOfLevelState.LowNoise;
            }

            if (distance < connectionLostDistance)
            {
                return OutOfLevelState.NoiseAndWarning;
            }

            else
            {
                return OutOfLevelState.ConnectionLost;
            }
        }

        public float GetCurrentNoiseFraction(OutOfLevelNotifiable outOfLevelNotifiable)
        {
            var currentDistance = this.GetCurrentDistance(outOfLevelNotifiable);
            var asFraction = (currentDistance - this.noiseStartDistance) /
                             (this.connectionLostDistance - this.noiseStartDistance);
            if (asFraction > 1f)
            {
                return 1f;
            }

            if (asFraction < 0f)
            {
                return 0f;
            }

            return asFraction;
        }
    }
}