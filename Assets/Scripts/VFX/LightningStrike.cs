using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class LightningStrike : MonoBehaviour
{
    [SerializeField] private VisualEffect vfx;
    [SerializeField] public Transform target;

    [ContextMenu("Strike")]
    public void Strike()
    {
        vfx.SetVector3("StartPos", transform.position);
        vfx.SetVector3("EndPos", target.position);
        vfx.Play();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(target.position, 0.3f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(target.position, 0.3f);
        //Selection.activeObject = target.gameObject;
    }
}
