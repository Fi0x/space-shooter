using System.Collections.Generic;
using System.Linq;
using Manager;
using Ship.Weaponry;
using Targeting.TargetChoosingStrategy;
using UnityEngine;

#nullable enable
namespace Targeting
{
    public class PrimaryTargetChoosingHelper
    {
        private readonly ITargetChoosingStrategy strategy;

        private PrimaryTargetChoosingHelper(ITargetChoosingStrategy strategy)
        {
            this.strategy = strategy;
        }

        public static PrimaryTargetChoosingHelper WithStrategy(ITargetChoosingStrategy strategy)
        {
            return new PrimaryTargetChoosingHelper(strategy);
        }

        public Targetable? GetTargetableToTarget(Transform selfTransform, float ownProjectileSpeed, IEnumerable<Targetable?> allTargetables, Targetable? currentPrimaryTarget = null)
        {
            var currentPrimaryTargetScore = EvaluateTargetable(selfTransform, ownProjectileSpeed, currentPrimaryTarget) ?? float.NegativeInfinity;
            
            var maxTargetScore = float.NegativeInfinity;
            Targetable? maxTarget = null;
            
            foreach (var targetable in allTargetables)
            {
                if (targetable == currentPrimaryTarget)
                {
                    continue;
                }

                var score = EvaluateTargetable(selfTransform, ownProjectileSpeed, targetable);
                if (score != null && score > maxTargetScore)
                {
                    maxTargetScore = score.Value;
                    maxTarget = targetable;
                }
            }

            if (maxTargetScore - this.strategy.CurrentTargetBias > currentPrimaryTargetScore)
            {
                // The current max Score does overcome the Current Target Bias
                return maxTarget;
            }
            else
            {
                // The current max Score does NOT overcome the Current Target Bias
                return currentPrimaryTarget;
            }
        }

        private float? EvaluateTargetable(Transform selfTransform, float ownProjectileSpeed,
            Targetable? targetable)
        {
            float? currentPrimaryTargetScore = null;
            if (targetable != null)
            {
                var positionAndTimeOfCollision = GetPositionAndTimeOfCollision(ownProjectileSpeed, selfTransform.position,
                    targetable.transform.position, targetable.Velocity);

                if (positionAndTimeOfCollision == null)
                {
                    // Cannot hit target here. The primary Target Score stays -Inf
                }
                else
                {
                    currentPrimaryTargetScore = this.strategy.Evaluate(selfTransform, targetable.transform,
                        positionAndTimeOfCollision.Value.pos, positionAndTimeOfCollision.Value.time);
                }
            }

            if (currentPrimaryTargetScore != null)
            {
                Debug.Log($"Eval of {targetable.name} is {(currentPrimaryTargetScore.Value * 100):F2}");
            }
            else if(targetable != null)
            {
                Debug.Log($"Eval of {targetable.name} is null");
            }
            return currentPrimaryTargetScore;
        }

        private static (Vector3 pos, float time)? GetPositionAndTimeOfCollision(float projectileSpeed, Vector3 ownPos, Vector3 theirPos,
            Vector3 theirVelocity)
        {
            if (theirVelocity.magnitude < 0.01f)
            {
                return (theirPos, Vector3.Distance(ownPos, theirPos) / projectileSpeed);
            }
            
            
            var timeOfCollision =
                TargetingCalculationHelper.GetPredictedTimeOfCollision(ownPos, projectileSpeed, theirPos,
                    theirVelocity);

            if (timeOfCollision == null)
            {
                return null;
            }

            var timeOfCollisionNonNull = timeOfCollision.Value;

            return (theirPos + theirVelocity * timeOfCollisionNonNull, timeOfCollisionNonNull);
        }
    }
}