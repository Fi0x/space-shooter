using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public static class BoidCalculationsHelper
    {
        public static void RebuildNeighbourLists(this Boid self)
        {
            var cohesion = self.CohesionNeighbours;
            var alignment = self.AlignmentNeighbours;
            var avoidance = self.AvoidanceNeighbours;
            var flock = self.ParentFlock;
            
            foreach (var list in new[] {cohesion, alignment, avoidance})
            {
                list.Clear();
            }

            foreach (var entry in flock.allUnits)
            {
                if (entry == self)
                {
                    continue;
                }

                var distanceBetweenSelfAndEntry = Vector3.Distance(self.transform.position, entry.transform.position);
                if (distanceBetweenSelfAndEntry <= flock.cohesionDistance)
                {
                    cohesion.Add(entry);
                }

                if (distanceBetweenSelfAndEntry <= flock.avoidanceDistance)
                {
                    avoidance.Add(entry);
                }

                if (distanceBetweenSelfAndEntry <= flock.alignmentDistance)
                {
                    alignment.Add(entry);
                }
            }
        }

        public static Vector3 CalculateMoveVector(this Boid self)
        {
            var cohesion = CalculateCohesionVector(self.transform, self.CohesionNeighbours, self.FovAngle) *
                           self.ParentFlock.cohesionWeight;
            var alignment = CalculateAlignmentVector(self.transform, self.AlignmentNeighbours, self.ParentFlock,
                self.FovAngle) * self.ParentFlock.alignmentWeight;
            var avoidance = CalculateAvoidanceVector(self.transform, self.AvoidanceNeighbours, self.FovAngle) *
                            self.ParentFlock.avoidanceWeight;
            return cohesion + alignment + avoidance;
        }


        private static Vector3 CalculateCohesionVector(Transform ownTransform, IReadOnlyList<Boid> cohesionNeighbours, float fovAngle)
        {
            var cohesionVector = Vector3.zero;
            if (cohesionNeighbours.Count == 0)
            {
                return cohesionVector;
            }

            var neighboursInFOV = 0;
            foreach (var neighbour in cohesionNeighbours)
            {
                if (IsInFOV(neighbour.transform.position, ownTransform.position, ownTransform.forward, fovAngle))
                {
                    neighboursInFOV++;
                    cohesionVector += neighbour.transform.position;
                }
            }

            cohesionVector /= neighboursInFOV;
            cohesionVector -= ownTransform.position;
            cohesionVector = cohesionVector.normalized; 
            return cohesionVector;
        }

        private static Vector3 CalculateAlignmentVector(Transform ownTransform, IReadOnlyList<Boid> alignmentNeighbours, BoidController ownFlock, float fovAngle)
        {
            var alignmentVector = (ownFlock.roamingPosition - ownTransform.position).normalized;
            if (alignmentNeighbours.Count == 0)
            {
                return alignmentVector;
            }

            var neighboursInFOV = 0;
            foreach (var t in alignmentNeighbours)
            {
                if (IsInFOV(t.transform.position, ownTransform.position, ownTransform.forward, fovAngle ))
                {
                    neighboursInFOV++;
                    alignmentVector += t.transform.forward;
                }
            }

            alignmentVector /= neighboursInFOV; // ??? Warum wird das gemacht wenn danach sowieso .normalized aufgerufen wird?
            alignmentVector = alignmentVector.normalized;
            return alignmentVector;
        }

        private static Vector3 CalculateAvoidanceVector(Transform ownTransform, IReadOnlyList<Boid> avoidanceNeighbours, float fovAngle)
        {
            var avoidanceVector = Vector3.zero;
            if (avoidanceNeighbours.Count == 0)
            {
                return avoidanceVector;
            }

            var neighboursInFOV = 0;
            foreach (var t in avoidanceNeighbours)
            {
                if (IsInFOV(t.transform.position, ownTransform.position, ownTransform.forward, fovAngle))
                {
                    neighboursInFOV++;
                    avoidanceVector += (ownTransform.position - t.transform.position);
                }
            }

            avoidanceVector /= neighboursInFOV;
            avoidanceVector = avoidanceVector.normalized;
            return avoidanceVector;
        }
        
        private static bool IsInFOV(Vector3 theirPosition, Vector3 ownPosition, Vector3 ownForward, float fovAngle) => 
            Vector3.Angle(ownForward, theirPosition - ownPosition) <= fovAngle;

        public static Vector3 CalculateFlockCenter(this BoidController flock)
        {
            var returnValue = Vector3.zero;
            foreach (var boid in flock.allUnits)
            {
                returnValue += boid.transform.position;
            }

            return returnValue / flock.allUnits.Count;
        }
        
    }
}