using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RadarManager : MonoBehaviour
{

    [SerializeField] private Transform ownPosition;
    [SerializeField] private float radarRadius = 0.3f;

    [SerializeField] private List<Transform> radarDetectableList = new List<Transform>();

    [SerializeField] private Sprite spriteFront;
    [SerializeField] private Sprite spriteBack;

    private readonly Dictionary<GameObject, GameObject> radarDetectableToUiElementLookUp =
        new Dictionary<GameObject, GameObject>();

    // Update is called once per frame
    void Update()
    {
        foreach (var radarDetectable in this.radarDetectableList)
        {
            if (!radarDetectableToUiElementLookUp.ContainsKey(radarDetectable.gameObject))
            {
                GameObject newSprite = new GameObject();
                newSprite.transform.parent = this.gameObject.transform;
                newSprite.transform.localScale = new Vector3(0.04f, 0.05f, 1);
                SpriteRenderer spriteRenderer = newSprite.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = this.spriteFront;

                radarDetectableToUiElementLookUp.Add(radarDetectable.gameObject, newSprite);
            }

            var spriteObject = radarDetectableToUiElementLookUp[radarDetectable.gameObject];

            Vector3 axes;
            float angle;
            //Quaternion.FromToRotation(ownPosition.forward, radarDetectable.position - ownPosition.position).ToAngleAxis(out angle, out axes);
            axes = Quaternion.LookRotation(radarDetectable.position - ownPosition.position).eulerAngles;

            var differenceInRotation = Quaternion.FromToRotation(this.ownPosition.forward,
                radarDetectable.position - ownPosition.position);

            var euler = differenceInRotation.normalized.eulerAngles;
            var phi = (euler.y) * Mathf.Deg2Rad;
            var theta = (euler.x + 90) * Mathf.Deg2Rad;

            //var posY = this.radarRadius * (float)Math.Sin(eulerY);
            //var posZ = this.radarRadius * (float)Math.Sin(eulerZ);

            spriteObject.transform.localPosition = new Vector3((float)Math.Sin(phi) * radarRadius, (float)Math.Cos(theta)  * radarRadius, 0);

        }
    }


    // see https://answers.unity.com/questions/189724/polar-spherical-coordinates-to-xyz-and-vice-versa.html
    private Vector2 PolarCoordsFromRotation(Vector3 point)
    {
        var polar = new Vector2();
        //calc longitude
        polar.y = Mathf.Atan2(point.x,point.z);

        //this is easier to write and read than sqrt(pow(x,2), pow(y,2))!
        var xzLen = new Vector2(point.x,point.z).magnitude;
        //atan2 does the magic
        polar.x = Mathf.Atan2(-point.y,xzLen);

        //convert to deg
        //polar *= Mathf.Rad2Deg;

        return polar;

    }




}
