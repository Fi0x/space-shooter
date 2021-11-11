using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = System.Random;

namespace World
{
    /**
     * This Class is concerned with building the level,
     * from procedurally generating asteroids, to placing structures like stations and portals
     */
    public class LevelBuilder : MonoBehaviour
    {
        [Header("Level Size", order = 0)]
        [Space(-10, order = 1)]
        [Header("  Total Level, Not just the Playable area", order = 2)]
        //
        [SerializeField] private Vector3Int sectorSize;
        [SerializeField] private Vector3Int sectorCount;
        [SerializeField] private int seed = 1337;

        [Header("Asteroid Density Functions", order = 0)]
        [Space(-10, order = 1)]
        [Header("  Density Functions that Evaluate a value [0, 1]", order = 2)]
        [Space(-10, order = 3)]
        [Header("  and map a probability onto [0,1]", order = 4)]
        //
        [SerializeField] private AnimationCurve xCrossSectionDensity;
        [SerializeField] private AnimationCurve yCrossSectionDensity;
        [SerializeField, Range(0, 1)] private float probabilityCutoff;
        [SerializeField] private List<GameObject> asteroidPrefabs;
        [SerializeField] private GameObject jumpGatePrefab;
        // Settings end
        [Header("Readonly")]
        //
#if DEBUG
        [SerializeField, ReadOnlyInspector] private SerializedDictionary<(int x, int y, int z),SectorDataDebug> sectorData;
#endif
        [SerializeField, ReadOnlyInspector] private bool isConstructed;

        private Random random;

        public void LoadRandomLevel()
        {
            seed = new Random().Next();
            
            if (asteroidPrefabs.Count == 0)
            {
                Debug.LogError("No Asteroids defined. Cannot spawn Prefabs");
                return;
            }

            if (isConstructed)
                Teardown();
            
            CreateAsteroids();
            PlacePortal();
        }

        private void CreateAsteroids()
        {
            var offset = new Vector3(sectorCount.x / 2f, sectorCount.y / 2f, sectorCount.z / 2f);
            offset.Scale(sectorSize);

#if DEBUG
            sectorData = new SerializedDictionary<(int x, int y, int z), SectorDataDebug>();
#endif
            random = new Random(seed);
            for (var x = 0; x < sectorCount.x; x++)
            {
                var xDensityValue = xCrossSectionDensity.Evaluate((float) x / sectorCount.x);
                for (var y = 0; y < sectorCount.y; y++)
                {
                    var yDensityValue = yCrossSectionDensity.Evaluate((float) y / sectorCount.y);
                    for (var z = 0; z < sectorCount.z; z++)
                    {
                        var probability = yDensityValue * xDensityValue * random.NextDouble();
                        var populateWithAsteroid = (1 - probability < probabilityCutoff);

#if DEBUG
                        var newSectorData = new SectorDataDebug(
                            sectorCount.x * sectorSize.x,
                            (1 + sectorCount.x) * sectorSize.x,
                            sectorCount.y * sectorSize.y,
                            (1 + sectorCount.y) * sectorSize.y,
                            sectorCount.z * sectorSize.z,
                            (1 + sectorCount.z) * sectorSize.z,
                            populateWithAsteroid
                        );
                        sectorData[(x, y, z)] = newSectorData;
#endif
                        if(!populateWithAsteroid) continue;
                        
                        var position = new Vector3(
                            (.5f + x) * sectorSize.x,
                            (.5f + y) * sectorSize.y,
                            (.5f + z) * sectorSize.z
                        ) - offset;

                        var rotation = Quaternion.Euler(random.Next(360), random.Next(360), random.Next(360));
                        var prefabToUse = asteroidPrefabs[random.Next(asteroidPrefabs.Count)];

                        Instantiate(prefabToUse, position, rotation, transform);
                    }
                }
            }
            
            isConstructed = true;
        }

        private void PlacePortal()
        {
            var position = new Vector3(
                sectorSize.x * sectorCount.x * probabilityCutoff * 0.7f,
                sectorSize.y * sectorCount.y * probabilityCutoff * 0.7f,
                sectorSize.z * sectorCount.z * probabilityCutoff * 0.7f);
            for (var x = 0; x < 2; x++)
            {
                for (var y = 0; y < 2; y++)
                {
                    for (var z = 0; z < 2; z++)
                    {
                        var rotation = Quaternion.Euler(random.Next(360), random.Next(360), random.Next(360));
                        Instantiate(jumpGatePrefab, position, rotation, transform);

                        position.z *= -1;
                    }

                    position.y *= -1;
                }

                position.x *= -1;
            }
        }

#if DEBUG
        private void OnDrawGizmos()
        {
            foreach (var entry in sectorData.Values)
            {
                entry.DrawGizmos();
            }
        }
#endif

        private void Teardown()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
#if DEBUG
            sectorData = null;
#endif 
            isConstructed = false;
        }
    }
#if DEBUG
    [Serializable]
    internal struct SectorDataDebug
    {
        internal SectorDataDebug(int lowerX, int upperX, int lowerY, int upperY, int lowerZ, int upperZ, bool isPopulated = false)
        {
            sectorBounds = (lowerX, upperX, lowerY, upperY, lowerZ, upperZ);
            corners = new[]
            {
                new Vector3(sectorBounds.x, sectorBounds.y, sectorBounds.Z),
                new Vector3(sectorBounds.X, sectorBounds.Y, sectorBounds.z),
                new Vector3(sectorBounds.X, sectorBounds.Y, sectorBounds.Z),
                new Vector3(sectorBounds.x, sectorBounds.y, sectorBounds.z),
            };
            this.isPopulated = isPopulated;
        }

        private Vector3[] corners;
        [SerializeField] public (int x, int X, int y, int Y, int z, int Z) sectorBounds;
        [SerializeField] public bool isPopulated;

        public void DrawGizmos()
        {
            Gizmos.color = isPopulated ? Color.green : Color.yellow;
            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[2], corners[3]);
        }
    }
#endif
}
