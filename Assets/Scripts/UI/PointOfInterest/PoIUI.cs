using UnityEngine;

namespace UI
{
    public class PoIUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float offset = 0f;
        [SerializeField] private PointOfInterest poi;
        [SerializeField] private Vector2 minMaxSize = new Vector2(30f, 60f);
        [SerializeField] private Vector2 rangeValues = new Vector2(50f, 100f);
        [SerializeField] private LayerMask hideBehindLayers;
        
        private Camera cam;

        public void SetPoi(PointOfInterest newPoi)
        {
            this.poi = newPoi;
        }
        
        private void Awake()
        {
            cam = Camera.main;
        }

        private void LateUpdate()
        {
            if (cam != null)
            {
                if (poi == null) return;
                var position = poi.transform.position;
                var camPosition = cam.WorldToScreenPoint(position + Vector3.up * offset);
                transform.position = camPosition;

                if (IsBehindObstacle(position, cam.transform.position))
                {
                    canvasGroup.alpha = 0f;
                    return;
                }
                
                float f = Vector3.Distance(position, cam.transform.position);
                f = Mathf.Clamp(f, rangeValues.x, rangeValues.y);
                f = f / Mathf.Abs(rangeValues.y - rangeValues.x);
                
                float size = Mathf.Lerp(minMaxSize.y, minMaxSize.x, f); 
                GetComponent<RectTransform>().sizeDelta = new Vector2((int)size, (int)size);
                
                
                var camTransform = cam.transform;
                float dot = Vector3.Dot(camTransform.forward,
                    (position - camTransform.position).normalized);
                //dot = 1f;

                canvasGroup.alpha = dot >= 0 ? 1f : 0f;
            }
        }

        private bool IsBehindObstacle(Vector3 position, Vector3 camPosition)
        {
            RaycastHit hitInfo;
            Ray ray = new Ray(camPosition, (position - camPosition).normalized);
            float max = (position - camPosition).magnitude;
            //Debug.DrawLine(position, camPosition, Color.yellow);
            if (Physics.Raycast(ray, out hitInfo, max, hideBehindLayers))
            {
                //Debug.DrawLine(hitInfo.point, hitInfo.collider.gameObject.transform.position, Color.magenta);
                return true;
            }
            return false;
        }
    }
}