using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationPart : MonoBehaviour
{
    [Header("SnapPoints")]
    public SnapPoint topSnapPoint;
    public SnapPoint bottomSnapPoint;
    [SerializeField] private List<Transform> turretSpawnPoints;

    [Header("StationSettings")]
    public GameObject turretPrefab;

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
}
