using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public SnapPoint snapTarget;
    public StationPart snappableStation => GetComponentInParent<StationPart>();
    
    public void SnapAndAlign()
    {
        if (snapTarget != null)
        {
            AlignTo(snapTarget);
            SnapTo(snapTarget);
        }
    }
    
    public void SnapAndAlignOtherToMe()
    {
        if (snapTarget != null)
        {
            snapTarget.AlignTo(this);
            snapTarget.SnapTo(this);
        }
    }
    
    public void AlignTo(SnapPoint other)
    {
        var stationTransform = snappableStation.transform;
        var rotationOffset = transform.rotation.eulerAngles.x - stationTransform.rotation.eulerAngles.x;
        stationTransform.rotation = other.transform.rotation;
        //stationTransform.Rotate(180, 0, 0);
        stationTransform.Rotate(-rotationOffset, 0, 0);
    }
    
    public void SnapTo(SnapPoint other)
    {
        var stationTransform = snappableStation.transform;
        var offset = stationTransform.position - transform.position;
        var newPosition = other.transform.position + offset;
        stationTransform.position = newPosition;
    }
}
