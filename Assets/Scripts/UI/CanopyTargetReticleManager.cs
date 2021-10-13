using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanopyTargetReticleManager : MonoBehaviour
{
    [SerializeField] private float uiRadius = 1f;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private Sprite crosshair;
    [SerializeField] private Sprite crosshairHitmarker;

    private GameObject spriteObject;
    // Start is called before the first frame update
    void Start()
    {
        this.spriteObject = new GameObject("Target Crosshair");
        this.spriteObject.transform.parent = transform;

        var spriteRenderer = this.spriteObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = this.crosshair;
    }

    // Update is called once per frame
    void Update()
    {
        var ownPosition = this.gameObject.transform.position;
        var spritePositionOnSphere = ownPosition +
                                     (weaponManager.Target - ownPosition).normalized *
                                     this.uiRadius;
        this.spriteObject.transform.position = spritePositionOnSphere;
        this.spriteObject.transform.LookAt(this.transform);
    }
}
