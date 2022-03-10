using System.Collections;
using System.Collections.Generic;
using Enemy.Station;
using UnityEngine;

public class StationBuilder : MonoBehaviour
{
    [Header("StationSettings")]
    [SerializeField] private int partCount = 3;
    [SerializeField] private float turretProbability = 0.5f;

    [Header("StationParts")]
    [SerializeField] private List<GameObject> endPieces;
    [SerializeField] private List<GameObject> turretPieces;
    [SerializeField] private List<GameObject> connectorPieces;

    private StationPart startPart;
    private StationPart currentPart;
    private StationController controller;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<StationController>();
        BuildStation();
    }

    private void BuildStation()
    {
        //StartPiece
        var startPiece = SpawnStationPart(turretPieces);
        startPart = startPiece.GetComponent<StationPart>();
        controller.parts.Add(startPart);
        startPiece.transform.Rotate(0, Random.Range(-360, 360), 0);
        
        //Build Bottom End Piece
        var bottomPiece = SpawnStationPart(endPieces);
        currentPart = bottomPiece.GetComponent<StationPart>();
        controller.parts.Add(currentPart);
        startPart.SnapStationPartToMe(currentPart, false);
        bottomPiece.transform.Rotate(0, Random.Range(-360, 360), 0);
        
        //Build Pieces in between
        for (int i = 0; i < partCount - 1; i++)
        {
            var newPart = SpawnStationPart((i % 2 == 0) ? connectorPieces : turretPieces);
            currentPart = newPart.GetComponent<StationPart>();
            controller.parts.Add(currentPart);
            startPart.SnapStationPartToMe(currentPart, true);
            startPart = currentPart;
            newPart.transform.Rotate(0, Random.Range(-360, 360), 0);
        }
        
        //Build Top End Piece
        var topPiece = SpawnStationPart(endPieces);
        currentPart = topPiece.GetComponent<StationPart>();
        controller.parts.Add(currentPart);
        startPart.SnapStationPartToMe(currentPart, true);
        topPiece.transform.Rotate(0, Random.Range(-360, 360), 0);
    }

    private GameObject SpawnStationPart(List<GameObject> objects)
    {
        int i = Random.Range(0, objects.Count);
        if (i < 0) return null;
        return Instantiate(objects[i], transform);
    }
}
