using UnityEngine;

namespace Targeting.TargetChoosingStrategy
{
    public class BasicHitScanTargetChoosingStrategy : ITargetChoosingStrategy
    {

        private const float AngleWeight = 0.1f;

        // Both have to be > 0 to avoid positive Infinity
        private const float AngleOffset = 0.1f;


        public BasicHitScanTargetChoosingStrategy(float targetBias = 10)
        {
            this.CurrentTargetBias = targetBias;
        }
        
        // See https://www.desmos.com/calculator/lm4oibbmxo
        public float Evaluate(Transform self, Transform target, Vector3 targetPositionOnCollision, float _)
        {
            var angleDegrees = Vector3.Angle(self.forward, targetPositionOnCollision - target.position);

            return (AngleWeight /
                (angleDegrees + AngleOffset));
        }

        public float CurrentTargetBias { get; }
    }
}