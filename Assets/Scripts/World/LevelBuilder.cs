using System;
using System.Collections.Generic;
using Ship.Sensors;
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
        
        [SerializeField, ReadOnlyInspector] private List<SensorTarget> portals = new List<SensorTarget>();
        public IReadOnlyList<SensorTarget> Portals => this.portals;

        private Random random;

        public void LoadRandomLevel()
        {
            this.seed = new Random().Next();
            
            if (this.asteroidPrefabs.Count == 0)
            {
                Debug.LogError("No Asteroids defined. Cannot spawn Prefabs");
                return;
            }

            if (this.isConstructed)
                this.Teardown();
            
            this.CreateAsteroids();
            this.PlacePortal();
        }

        private void CreateAsteroids()
        {
            var offset = new Vector3(this.sectorCount.x / 2f, this.sectorCount.y / 2f, this.sectorCount.z / 2f);
            offset.Scale(this.sectorSize);

#if DEBUG
            this.sectorData = new SerializedDictionary<(int x, int y, int z), SectorDataDebug>();
#endif
            this.random = new Random(this.seed);
            for (var x = 0; x < this.sectorCount.x; x++)
            {
                var xDensityValue = this.xCrossSectionDensity.Evaluate((float) x / this.sectorCount.x);
                for (var y = 0; y < this.sectorCount.y; y++)
                {
                    var yDensityValue = this.yCrossSectionDensity.Evaluate((float) y / this.sectorCount.y);
                    for (var z = 0; z < this.sectorCount.z; z++)
                    {
                        var probability = yDensityValue * xDensityValue * this.random.NextDouble();
                        var populateWithAsteroid = (1 - probability < this.probabilityCutoff);

#if DEBUG
                        var newSectorData = new SectorDataDebug(
                            this.sectorCount.x * this.sectorSize.x,
                            (1 + this.sectorCount.x) * this.sectorSize.x,
                            this.sectorCount.y * this.sectorSize.y,
                            (1 + this.sectorCount.y) * this.sectorSize.y,
                            this.sectorCount.z * this.sectorSize.z,
                            (1 + this.sectorCount.z) * this.sectorSize.z,
                            populateWithAsteroid
                        );
                        sectorData[(x, y, z)] = newSectorData;
#endif
                        if(!populateWithAsteroid) continue;
                        
                        var position = new Vector3(
                            (.5f + x) * this.sectorSize.x,
                            (.5f + y) * this.sectorSize.y,
                            (.5f + z) * this.sectorSize.z
                        ) - offset;

                        var rotation = Quaternion.Euler(this.random.Next(360), this.random.Next(360), this.random.Next(360));
                        var prefabToUse = this.asteroidPrefabs[this.random.Next(this.asteroidPrefabs.Count)];

                        Instantiate(prefabToUse, position, rotation, this.transform);
                    }
                }
            }
            
            isConstructed = true;
        }

        private void PlacePortal()
        {
            var position = new Vector3(
                this.sectorSize.x * this.sectorCount.x * this.probabilityCutoff * 0.7f,
                this.sectorSize.y * this.sectorCount.y * this.probabilityCutoff * 0.7f,
                this.sectorSize.z * this.sectorCount.z * this.probabilityCutoff * 0.7f);
            for (var x = 0; x < 2; x++)
            {
                for (var y = 0; y < 2; y++)
                {
                    for (var z = 0; z < 2; z++)
                    {
                        var rotation = Quaternion.Euler(this.random.Next(360), this.random.Next(360), this.random.Next(360));
                        
                        var gate = Instantiate(this.jumpGatePrefab, position, rotation, this.transform);
                        var sensorTarget = gate.GetComponent<SensorTarget>();
                        sensorTarget.TargetDestroyedEvent += target => this.portals.Remove(target);
                        sensorTarget.Init(SensorTarget.TargetType.JumpGate, SensorTarget.TargetAllegiance.Friendly);
                        this.portals.Add(sensorTarget);
                        RadarManager.InvokeRadarObjectSpawnedEvent(gate);

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
            this.SectorBounds = (lowerX, upperX, lowerY, upperY, lowerZ, upperZ);
            this.corners = new[]
            {
                new Vector3(this.SectorBounds.x, this.SectorBounds.y, this.SectorBounds.Z),
                new Vector3(this.SectorBounds.X, this.SectorBounds.Y, this.SectorBounds.z),
                new Vector3(this.SectorBounds.X, this.SectorBounds.Y, this.SectorBounds.Z),
                new Vector3(this.SectorBounds.x, this.SectorBounds.y, this.SectorBounds.z),
            };
            this.isPopulated = isPopulated;
        }

        private Vector3[] corners;
        public (int x, int X, int y, int Y, int z, int Z) SectorBounds;
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
