using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
        // Settings end
        [Header("Readonly")]
        //
#if DEBUG
        [SerializeField, ReadOnlyInspector] private SerializedDictionary<(int x, int y, int z),SectorDataDebug> sectorData;
#endif
        [SerializeField, ReadOnlyInspector] private bool isConstructed = false;

        private System.Random random;
        // Start is called before the first frame update
        void Start()
        {
            if (this.asteroidPrefabs.Count == 0)
            {
                Debug.LogError("No Asteroids defined. Cannot spawn Prefabs");
            }
            this.Construct();
        }

        private void Construct()
        {
            if (this.isConstructed)
            {
                this.Teardown();
            }

            if (this.asteroidPrefabs.Count == 0)
            {
                return;
            }
#if DEBUG
            this.sectorData = new SerializedDictionary<(int x, int y, int z), SectorDataDebug>();
#endif
            this.random = new System.Random(this.seed);
            for (int x = 0; x < sectorCount.x; x++)
            {
                float xDensityValue = this.xCrossSectionDensity.Evaluate((float) x / sectorCount.x);
                for (int y = 0; y < sectorCount.y; y++)
                {
                    float yDensityValue = this.yCrossSectionDensity.Evaluate((float) y / sectorCount.y);
                    for (int z = 0; z < sectorCount.z; z++)
                    {
                        double probability = yDensityValue * xDensityValue * this.random.NextDouble();
                        bool populateWithAsteroid = (1 - probability < this.probabilityCutoff);

#if DEBUG
                        var sectorData = new SectorDataDebug(
                            this.sectorCount.x * this.sectorSize.x,
                            (1 + this.sectorCount.x) * this.sectorSize.x,
                            this.sectorCount.y * this.sectorSize.y,
                            (1 + this.sectorCount.y) * this.sectorSize.y,
                            this.sectorCount.z * this.sectorSize.z,
                            (1 + this.sectorCount.z) * this.sectorSize.z,
                            populateWithAsteroid
                        );
                        this.sectorData[(x, y, z)] = sectorData;
#endif
                        if (populateWithAsteroid)
                        {
                            Vector3 position = new Vector3(
                                (.5f + x) * this.sectorSize.x,
                                (.5f + y) * this.sectorSize.y,
                                (.5f + z) * this.sectorSize.z
                            );

                            var rotation = Quaternion.Euler(this.random.Next(360), this.random.Next(360),
                                this.random.Next(360));

                            var prefabToUse = this.asteroidPrefabs[this.random.Next(this.asteroidPrefabs.Count)];

                            Instantiate(prefabToUse, position, rotation, this.transform);

                        }
                    }
                }
            }
            
        }

#if DEBUG
        private void OnDrawGizmos()
        {
            foreach (var entry in this.sectorData.Values)
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
            this.sectorData = null;
#endif 
            this.isConstructed = false;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
#if DEBUG
    [Serializable]
    internal struct SectorDataDebug
    {
        internal SectorDataDebug(int lowerX, int upperX, int lowerY, int upperY, int lowerZ, int upperZ, bool isPopulated = false)
        {
            this.sectorBounds = (lowerX, upperX, lowerY, upperY, lowerZ, upperZ);
            this.corners = new[]
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
