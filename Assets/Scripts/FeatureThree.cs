using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


namespace CityBuilderCore
{
    /// <summary>
    /// 3. Able to rotate, scale, translate these items in the scene 
    /// </summary>
    public class FeatureThree : BaseTool
    {
        private enum ActiveFeature { None, Rotate, Scale, Translate }
        private ActiveFeature activeFeature;
        private IMouseInput _mouseInput;
        private IBuilding _building { get; set; }
        private GameObject _target;

        private IHighlightManager _highlighting;

        [Header("UI")]
        [SerializeField] private GameObject toolMaseter;
        [SerializeField] private TMP_Text objectName;
        [SerializeField] private Button buttonClose;
        [SerializeField] private Button buttonRotate;
        [SerializeField] private Button buttonRotatePlus;
        [SerializeField] private Button buttonRotateMinus;

        [SerializeField] private Button buttonScale;
        [SerializeField] private Button buttonScalePlus;
        [SerializeField] private Button buttonScaleMinus;

        [SerializeField] private Button buttonTranslate;
        [SerializeField] private Button buttonTranslateXPlus;
        [SerializeField] private Button buttonTranslateXMinus;
        [SerializeField] private Button buttonTranslateZPlus;
        [SerializeField] private Button buttonTranslateZMinus;
        private void Start()
        {
            toolMaseter.SetActive(false);
            _mouseInput = Dependencies.Get<IMouseInput>();
            _highlighting = Dependencies.Get<IHighlightManager>();
            buttonClose.onClick.AddListener(() => toolMaseter.SetActive(false));

            //rotation
            buttonRotatePlus.onClick.AddListener(() => OnRotatePlus());
            buttonRotateMinus.onClick.AddListener(() => OnRotateMinus());

            //scale
            buttonScalePlus.onClick.AddListener(() => OnScalePlus());
            buttonScaleMinus.onClick.AddListener(() => OnScaleMinus());

            //translate
            buttonTranslateXPlus.onClick.AddListener(() => OnTranslateXPlus());
            buttonTranslateXMinus.onClick.AddListener(() => OnTranslateXMinus());
            buttonTranslateZPlus.onClick.AddListener(() => OnTranslateZPlus());
            buttonTranslateZMinus.onClick.AddListener(() => OnTranslateZMinus());

            buttonRotate.onClick.AddListener(() => OnActivateFeature(ActiveFeature.Rotate));
            buttonScale.onClick.AddListener(() => OnActivateFeature(ActiveFeature.Scale));
            buttonTranslate.onClick.AddListener(() => OnActivateFeature(ActiveFeature.Translate));
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                var mousePosition = _mouseInput.GetMouseGridPosition();

                var walkerObject = Physics.RaycastAll(_mouseInput.GetRay()).Select(h => h.transform.gameObject).FirstOrDefault(g => g.CompareTag("Walker"));
                if (walkerObject)
                {
                    var walker = walkerObject.GetComponent<Walker>();
                    if (walker)
                    {
                        Debug.Log(walker.name);
                        return;
                    }
                }

                _building = Dependencies.Get<IBuildingManager>().GetBuilding(mousePosition).FirstOrDefault();
                if (_building != null)
                {

                    Plane plane = new Plane(Vector3.up, 0);
                    Vector3 worldPosition = Vector3.zero;
                    float distance;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (plane.Raycast(ray, out distance))
                    {
                        worldPosition = ray.GetPoint(distance);
                        toolMaseter.transform.position = worldPosition;
                    }
                    objectName.text = _building.GetName();
                    _target = _building.Root.gameObject;
                    toolMaseter.SetActive(true);
                    return;
                }
            }

            if (_target == null) toolMaseter.SetActive(false);
        }

        private void OnActivateFeature(ActiveFeature _feature)
        {
            bool _isRotate = _feature.Equals(ActiveFeature.Rotate);
            bool _isScale = _feature.Equals(ActiveFeature.Scale);
            bool _isTranslate = _feature.Equals(ActiveFeature.Translate);

            buttonRotatePlus.gameObject.SetActive(_isRotate);
            buttonRotateMinus.gameObject.SetActive(_isRotate);

            _highlighting.Clear();
            // _highlighting.Highlight(_rotation.RotateBuildingPoint(mousePoint, BuildingInfo.AccessPoint, BuildingInfo.Size), HighlightType.Info);

            buttonScalePlus.gameObject.SetActive(_isScale);
            buttonScaleMinus.gameObject.SetActive(_isScale);

            buttonTranslateXPlus.gameObject.SetActive(_isTranslate);
            buttonTranslateXMinus.gameObject.SetActive(_isTranslate);
            buttonTranslateZPlus.gameObject.SetActive(_isTranslate);
            buttonTranslateZMinus.gameObject.SetActive(_isTranslate);
        }
        private void OnRotatePlus()
        {
            _target.transform.GetChild(0).localRotation = Quaternion.AngleAxis(90, Vector3.up) * _target.transform.GetChild(0).localRotation;
        }

        private void OnRotateMinus()
        {
            _target.transform.GetChild(0).localRotation = Quaternion.AngleAxis(-90, Vector3.up) * _target.transform.GetChild(0).localRotation;
        }


        private void OnScalePlus()
        {
            _target.transform.localScale += new Vector3(1, 1, 1);
        }

        private void OnScaleMinus()
        {
            if (_target.transform.localScale.x > 1)
                _target.transform.localScale += new Vector3(-1, -1, -1);
        }


        private void OnTranslateXPlus()
        {
            _target.transform.localPosition += new Vector3(1, 0, 0);
        }

        private void OnTranslateXMinus()
        {
            _target.transform.localPosition += new Vector3(-1, 0, 0);
        }
        private void OnTranslateZPlus()
        {
            _target.transform.localPosition += new Vector3(0, 0, 1);
        }

        private void OnTranslateZMinus()
        {
            _target.transform.localPosition += new Vector3(0, 0, -1);
        }
    }
}
