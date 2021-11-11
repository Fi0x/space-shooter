using System.Collections;
using System.Collections.Generic;
using Ship;
using UnityEngine;

public class CanopyTargetReticleManager : MonoBehaviour
{
    [SerializeField] private float uiRadius = 1f;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private Sprite crosshair;
    [SerializeField] private Sprite crosshairHitmarker;
    [SerializeField] private Camera cameraRef;


    private IEnumerator animationCoroutine = null;

    private GameObject spriteObject;

    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        this.spriteObject = new GameObject("Target Crosshair");
        this.spriteObject.transform.parent = this.transform;

        this.spriteRenderer = this.spriteObject.AddComponent<SpriteRenderer>();
        this.spriteRenderer.sprite = this.crosshair;
    }

    // Update is called once per frame
    void Update()
    {
        var ownPosition = this.cameraRef.transform.position;
        var spritePositionOnSphere = ownPosition +
                                     (this.weaponManager.Target - ownPosition).normalized *
                                     this.uiRadius;
        this.spriteObject.transform.position = spritePositionOnSphere;
        this.spriteObject.transform.LookAt(this.transform, this.transform.parent.up);
    }

    public void NotifyAboutEnemyHit()
    {
        if (this.animationCoroutine != null)
        {
            this.StopCoroutine(this.animationCoroutine);
        }

        this.animationCoroutine = this.SpriteAnimator();
        this.StartCoroutine(this.animationCoroutine);
    }

    IEnumerator SpriteAnimator()
    {
        this.spriteRenderer.sprite = this.crosshairHitmarker;
        yield return new WaitForSeconds(0.1f);
        this.spriteRenderer.sprite = this.crosshair;
    }




}
