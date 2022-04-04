using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class LightningStrike : MonoBehaviour
{
    [SerializeField] private VisualEffect vfx;
    [SerializeField] public Transform source;
    [SerializeField] public Transform target;

    [ContextMenu("Strike")]
    public void Strike()
    {
        vfx.SetVector3("StartPos", source.position);
        vfx.SetVector3("EndPos", target.position);
        vfx.Play();
    }

    private void OnDrawGizmos()
    {
        if (target == null)
        {
            return;
        }
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(target.position, 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(source.position, 0.2f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(target.position, 0.2f);
        //Selection.activeObject = target.gameObject;
    }
}
