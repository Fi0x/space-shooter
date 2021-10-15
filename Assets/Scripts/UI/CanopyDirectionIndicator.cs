using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanopyDirectionIndicator : MonoBehaviour
{
    [SerializeField] private Sprite progradeSprite;
    [SerializeField] private Sprite retrogradeSprite;
    [SerializeField] private float radius;
    [SerializeField] private Rigidbody ship;
    [SerializeField] private AnimationCurve opacityForSpeed;

    private GameObject prograde, retrograde;
    private SpriteRenderer progradeSR, retrogradeSR;

    private void Start()
    {
        this.prograde = new GameObject("Prograde Sprite");
        this.prograde.AddComponent<SpriteRenderer>();
        this.progradeSR = this.prograde.GetComponent<SpriteRenderer>();
        this.progradeSR.sprite = this.progradeSprite;

        this.retrograde = new GameObject("Retrograde Sprite");
        this.retrograde.AddComponent<SpriteRenderer>();
        this.retrogradeSR = this.retrograde.GetComponent<SpriteRenderer>();
        this.retrogradeSR.sprite = this.retrogradeSprite;

        this.prograde.transform.parent = this.retrograde.transform.parent = this.transform;
    }

    private void Update()
    {
        var ownPosition = this.gameObject.transform.position;
        var shipMovementDirection = ship.velocity.normalized;
        var spritePositionPrograde = ownPosition + shipMovementDirection * radius;
        var spritePositionRetrograde = ownPosition + -shipMovementDirection * radius;

        this.prograde.transform.position = spritePositionPrograde;
        this.prograde.transform.LookAt(this.transform, this.transform.parent.up);

        this.retrograde.transform.position = spritePositionRetrograde;
        this.retrograde.transform.LookAt(this.transform, this.transform.parent.up);


        this.progradeSR.color = this.retrogradeSR.color =
            new Color(1, 1, 1, this.opacityForSpeed.Evaluate(this.ship.velocity.magnitude));
    }
}
