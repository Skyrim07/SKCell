#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SKCell
{
    [ExecuteInEditMode]
    [AddComponentMenu("SKCell/UI/SKUIModelViewer")]
    public class SKUIModelViewer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Range(0.1f, 1f)] public float resolution;
        [SerializeField] Transform scene;

        [SKFolder("Gallery Behaviour")]
        public float sensitivity = 0.1f;
        public bool xRotation = true;
        public bool yRotation = true;
        public bool applyInertia = true;
        public float damping = 1f;



        [SKFolder("References")]
        [SerializeField] Camera cam;
        [SerializeField] RawImage rawImage;



        [HideInInspector]
        public RectTransform rectTransform;

        private RenderTexture rt;
        SKVariableMonitor<Vector2> sizeMonitor;
        SKVariableMonitor<float> resMonitor;
        private Vector3 lastMousePos, rotAngle;

        private void Start()
        {
            if(rectTransform==null)
                rectTransform = GetComponent<RectTransform>();
            ReloadRT();
            sizeMonitor = new SKVariableMonitor<Vector2>(() =>
            {
                return rectTransform.sizeDelta;
            });

            sizeMonitor.onValueChanged += OnSizeChanged;
            resMonitor = new SKVariableMonitor<float>(() =>
            {
                return resolution;
            });

            resMonitor.onValueChanged += OnResChanged;
        }

        private void OnDestroy()
        {
            if(sizeMonitor !=null)
                sizeMonitor.Dispose();
            if (resMonitor != null)
                resMonitor.Dispose();
        }


        private void Initialize()
        {
            rectTransform = GetComponent<RectTransform>();
            ReloadRT();
        }
        private void OnSizeChanged(Vector2 size)
        {
            ReloadRT();
        }
        private void OnResChanged(float res)
        {
            ReloadRT();
        }
        private void ReloadRT()
        {
            int xRes = (int)(rectTransform.sizeDelta.x * resolution);
            int yRes = (int)(rectTransform.sizeDelta.y * resolution);
            if (rt != null)
                RenderTexture.ReleaseTemporary(rt);
            rt = RenderTexture.GetTemporary(xRes, yRes);
            cam.targetTexture = rt;
            rawImage.texture = rt;
        }

#if UNITY_EDITOR

        [SKInspectorButton("Generate Structure")]
        public void GenerateStructure()
        {
            string pathSuffix = "/UIModelViewer.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SKAssetLibrary.PREFAB_PATH + pathSuffix);
            if (prefab == null)
            {
                SKUtils.EditorLogError("SKUIModelViewer Resource Error: UIModelViewer prefab lost.");
                return;
            }
            GameObject viewer = Instantiate(prefab);
            viewer.name = $"SKUIViewer-{GetHashCode()}";
            viewer.transform.SetParent(transform.parent);
            viewer.transform.CopyFrom(transform);
            viewer.transform.SetSiblingIndex(transform.GetSiblingIndex());
            viewer.GetComponent<SKUIModelViewer>().Initialize();
            Selection.activeGameObject = viewer;
            DestroyImmediate(this.gameObject);
        }
#endif
        public void OnPointerDown(PointerEventData eventData)
        {
            if (applyInertia)
                StopCoroutine(nameof(InertiaCR));
            lastMousePos = Input.mousePosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (applyInertia)
                ApplyInertia(rotAngle);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePos;
            rotAngle = new Vector3(yRotation ? mouseDelta.y * sensitivity : 0, xRotation ? -mouseDelta.x * sensitivity : 0, 0);
            scene.transform.Rotate(rotAngle, Space.World);
            lastMousePos = Input.mousePosition;
        }
        private void ApplyInertia(Vector3 formerRotAngle)
        {
            SKUtils.StartCoroutine(InertiaCR(formerRotAngle));
        }
        IEnumerator InertiaCR(Vector3 formerRotAngle)
        {
            float force = 1f;
            while (force >= 0.001f)
            {
                force -= damping * Time.fixedDeltaTime;
                scene.transform.Rotate(rotAngle * force, Space.World);
                yield return new WaitForFixedUpdate();
            }
        }


    }
}
