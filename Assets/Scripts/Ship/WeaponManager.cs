using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private InputHandler inputHandler;

    public Vector3 Target => this.target.position;

    [SerializeField] private UnityEvent enemyHitEvent = new UnityEvent();
    public UnityEvent EnemyHitEvent => this.enemyHitEvent;
    public UnityEvent<bool> FiremodeChangedEvent { get; } = new UnityEvent<bool>();
    private bool isShooting = false;


    // Update is called once per frame
    void Update()
    {
        if (this.isShooting != this.inputHandler.IsShooting)
        {
            this.isShooting = !this.isShooting;
            this.FiremodeChangedEvent.Invoke(this.isShooting);
        }
    }
}
