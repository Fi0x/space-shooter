using System;
using System.Collections.Generic;
using Components;
using HealthSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class StationPart : MonoBehaviour
{
    [Header("SnapPoints")]
    public SnapPoint topSnapPoint;
    public SnapPoint bottomSnapPoint;
    [SerializeField] private List<Transform> turretSpawnPoints;

    [Header("StationSettings")]
    public GameObject turretPrefab;
    [SerializeField] private List<Health> healthTargets;
    
    [Header("DebrisRemoval")]
    [SerializeField] private float deleteAsteroidRadius = 200f;
    [SerializeField] private LayerMask mask;

    private readonly Collider[] collisions = new Collider[100];

    private void Start()
    {
        DeleteAsteroids();
    }

    public void SnapStationPartToMe(StationPart other, bool snapToTop)
    {
        if (snapToTop)
        {
            other.bottomSnapPoint.snapTarget = topSnapPoint;
            topSnapPoint.snapTarget = other.bottomSnapPoint;
            topSnapPoint.SnapAndAlignOtherToMe();
        }
        else
        {
            other.topSnapPoint.snapTarget = bottomSnapPoint;
            bottomSnapPoint.snapTarget = other.topSnapPoint;
            bottomSnapPoint.SnapAndAlignOtherToMe();
        }
        
    }

    public void GenerateTurrets(float probability)
    {
        foreach (var spawnPoint in turretSpawnPoints)
        {
            bool generateTurret = Random.Range(0f, 1f) < probability;
            if (generateTurret)
            {
                SpawnSingleTurret(spawnPoint);
            }
        }
    }

    public void SpawnSingleTurret(Transform spawnTransform)
    {
        var turret = Instantiate(turretPrefab, transform);
        turret.transform.position = spawnTransform.position;
        turret.transform.rotation = spawnTransform.rotation;
    }

    public List<Health> GetTargets()
    {
        return healthTargets;
    }

    public void DeleteAsteroids()
    {
        var size = Physics.OverlapSphereNonAlloc(transform.position, deleteAsteroidRadius, collisions, mask);
        foreach (var c in collisions)
        {
            if(c == null) return;
            //Debug.Log("Deleted" + c.gameObject.name);
            Destroy(c.gameObject);
        }
    }
}
