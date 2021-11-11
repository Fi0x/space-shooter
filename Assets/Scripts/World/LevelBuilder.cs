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
        [SerializeField, ReadOnlyInspector] private bool isConstructed;

        private System.Random random;
        // Start is called before the first frame update
        void Start()
        {
            if (this.asteroidPrefabs.Count == 0)
            {
                Debug.LogError("No Asteroids defined. Cannot spawn Prefabs");
                return;
            }

            if (this.isConstructed) this.Teardown();

            this.Construct();
        }

        private void Construct()
        {
            var offset = new Vector3(this.sectorCount.x / 2f, this.sectorCount.y / 2f, this.sectorCount.z / 2f);
            offset.Scale(this.sectorSize);

#if DEBUG
            this.sectorData = new SerializedDictionary<(int x, int y, int z), SectorDataDebug>();
#endif
            this.random = new System.Random(this.seed);
            for (int x = 0; x < this.sectorCount.x; x++)
            {
                float xDensityValue = this.xCrossSectionDensity.Evaluate((float) x / this.sectorCount.x);
                for (int y = 0; y < this.sectorCount.y; y++)
                {
                    float yDensityValue = this.yCrossSectionDensity.Evaluate((float) y / this.sectorCount.y);
                    for (int z = 0; z < this.sectorCount.z; z++)
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
                            ) - offset;

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
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }
#if DEBUG
            this.sectorData = null;
#endif
            this.isConstructed = false;
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
                new Vector3(this.sectorBounds.x, this.sectorBounds.y, this.sectorBounds.Z),
                new Vector3(this.sectorBounds.X, this.sectorBounds.Y, this.sectorBounds.z),
                new Vector3(this.sectorBounds.X, this.sectorBounds.Y, this.sectorBounds.Z),
                new Vector3(this.sectorBounds.x, this.sectorBounds.y, this.sectorBounds.z),
            };
            this.isPopulated = isPopulated;
        }

        private Vector3[] corners;
        [SerializeField] public (int x, int X, int y, int Y, int z, int Z) sectorBounds;
        [SerializeField] public bool isPopulated;

        public void DrawGizmos()
        {
            Gizmos.color = this.isPopulated ? Color.green : Color.yellow;
            Gizmos.DrawLine(this.corners[0], this.corners[1]);
            Gizmos.DrawLine(this.corners[2], this.corners[3]);
        }
    }
#endif
}
