using UnityEngine;

namespace Targeting.TargetChoosingStrategy
{
    public interface ITargetChoosingStrategy
    {
        float Evaluate(Transform self, Transform target, Vector3 targetPositionOnCollision, float projectileTravelTime);
        
        float CurrentTargetBias { get; }
    }
}