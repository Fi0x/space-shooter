using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private InputHandler inputHandler;

    public Vector3 Target => target.position;

    public UnityEvent<bool> FiremodeChangedEvent { get; } = new UnityEvent<bool>();
    private bool isShooting = false;


    // Update is called once per frame
    void Update()
    {
        if (isShooting != this.inputHandler.IsShooting)
        {
            isShooting = !isShooting;
            FiremodeChangedEvent.Invoke(isShooting);
        }
    }
}
