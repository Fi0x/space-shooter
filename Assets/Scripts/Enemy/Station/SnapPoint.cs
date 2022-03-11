using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public SnapPoint snapTarget;
    public StationPart snappableStation => GetComponentInParent<StationPart>();
    
    [ContextMenu("SnapMeToOther")]
    public void SnapAndAlign()
    {
        if (snapTarget != null)
        {
            AlignTo(snapTarget);
            SnapTo(snapTarget);
        }
    }
    
    [ContextMenu("SnapOtherToMe")]
    public void SnapAndAlignOtherToMe()
    {
        if (snapTarget != null)
        {
            snapTarget.AlignTo(this);
            snapTarget.SnapTo(this);
        }
    }

    [ContextMenu("AlignMe")]
    public void AlignMeToOther()
    {
        AlignTo(snapTarget);
    }
    
    public void AlignTo(SnapPoint other)
    {
        var stationTransform = snappableStation.transform;
        var rotationOffset = transform.rotation.eulerAngles.z - stationTransform.rotation.eulerAngles.z;
        stationTransform.rotation = other.transform.rotation;
        stationTransform.Rotate(0, 0, 180);
        stationTransform.Rotate(0, 0, -rotationOffset);
    }
    
    public void SnapTo(SnapPoint other)
    {
        var stationTransform = snappableStation.transform;
        var offset = stationTransform.position - transform.position;
        var newPosition = other.transform.position + offset;
        stationTransform.position = newPosition;
    }
}
