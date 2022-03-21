using UnityEngine;

namespace Targeting.TargetChoosingStrategy
{
    public class BasicProjectileTargetChoosingStrategy : ITargetChoosingStrategy
    {

        private const float AngleWeight = 0.1f;
        private const float ProjectileTravelTimeWeight = 0.37f;

        // Both have to be > 0 to avoid positive Infinity
        private const float AngleOffset = 0.1f;
        private const float ProjectileTravelTimeOffset = 0.4f;
        

        public BasicProjectileTargetChoosingStrategy(float targetBias = 10)
        {
            this.CurrentTargetBias = targetBias;
        }
        
        // See https://www.desmos.com/calculator/lm4oibbmxo
        public float Evaluate(Transform self, Transform target, Vector3 targetPositionOnCollision, float projectileTravelTime)
        {
            var angleDegrees = Vector3.Angle(self.forward, targetPositionOnCollision - target.position);

            return ProjectileTravelTimeWeight / (projectileTravelTime + ProjectileTravelTimeOffset) * (AngleWeight /
                (angleDegrees + AngleOffset));
        }

        public float CurrentTargetBias { get; }
    }
}