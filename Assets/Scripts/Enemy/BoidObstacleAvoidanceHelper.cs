using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class BoidObstacleAvoidanceHelper
    {
        private Vector3[] directions;
        public IEnumerable<Vector3> Directions => this.directions;

        //float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        // 1+Math.Qqrt(5) = 3.23606797749979f
        private const float GoldenRatio = 3.23606797749979f / 2f;
        private readonly int numViewDirections;
        
        public BoidObstacleAvoidanceHelper(int directionsCount)
        {
            this.numViewDirections = directionsCount;
            this.directions = BuildDirectionsArray();
        }
        
        private Vector3[] BuildDirectionsArray()
        {
            var returnArray = new Vector3[numViewDirections];

            const float angleIncrement = Mathf.PI * 2 * GoldenRatio;

            for (var i = 0; i < numViewDirections; i++)
            {
                var t = (float)i / numViewDirections;
                var inclination = Mathf.Acos(1 - 2 * t);
                var azimuth = angleIncrement * i;

                var x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
                var y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
                var z = Mathf.Cos(inclination);
                returnArray[i] = new Vector3(x, y, z);
                //Debug.DrawRay(transform.position, directions[i] *  10, Color.red);
            }

            return returnArray;
        }
    }
}