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
    /// 2. Click object (road, building, crowd) to zoom/focus on ( hide/translucents others )
    /// </summary>
    public class FeatureThree : BaseTool
    {
        private enum ActiveFeature { None, Rotate, Scale, Translate }
        private ActiveFeature activeFeature;
        private IMouseInput _mouseInput;
        private IBuilding _building { get; set; }
        private GameObject _target;
        private bool _isBuild = false;
        private BuildingBuilder _buildingBuilder;
        private BuildingRotation _buildingRotation;
        public Transform MainCameraPivot;

        public Material materialGhost;


        [Header("UI")]
        [SerializeField] private Transform canvasUI;
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
            if (Input.GetMouseButtonDown(0))
            {
                var mousePosition = _mouseInput.GetMouseGridPosition();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane plane = new Plane(Vector3.up, 0);
                Vector3 worldPosition = Vector3.zero;
                float distance;
                if (plane.Raycast(ray, out distance))
                {
                    worldPosition = ray.GetPoint(distance);


                }
                var walkerObject = Physics.RaycastAll(_mouseInput.GetRay()).Select(h => h.transform.gameObject).FirstOrDefault(g => g.CompareTag("Walker"));
                if (walkerObject)
                {
                    var walker = walkerObject.GetComponent<Walker>();
                    if (walker)
                    {

                        StartCoroutine(Lerp(MainCameraPivot, worldPosition, 1f));
                        return;
                    }
                }

                _building = Dependencies.Get<IBuildingManager>().GetBuilding(mousePosition).FirstOrDefault();
                if (_building != null)
                {



                    toolMaseter.transform.position = worldPosition;

                    RecenterCanvasUI();
                    ResetAllTranslucent();
                    StartCoroutine(Lerp(MainCameraPivot, worldPosition, 1f, SetAllToTranslucent));

                    objectName.text = _building.GetName();
                    _target = _building.Root.gameObject;
                    _buildingRotation = _target.GetComponent<Building>().Rotation;
                    toolMaseter.SetActive(true);
                    return;
                }
            }

            if (_target == null || Input.GetMouseButtonDown(1))
            {
                ResetAllTranslucent();
                toolMaseter.SetActive(false);
            }

            if (_isBuild && Input.GetMouseButtonDown(1))
            {
                _buildingBuilder.DeactivateTool();
                _isBuild = false;
            }
        }

        #region translucent and focus

        IEnumerator Lerp(Transform _target, Vector3 _end, float _duration, System.Action _postAction = null)
        {
            float timeElapsed = 0;
            while (timeElapsed < _duration)
            {
                _target.position = Vector3.Lerp(_target.position, _end, timeElapsed / _duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            _target.position = _end;
            if (_postAction != null) _postAction();

        }

        Dictionary<GameObject, Translucent> dd = new Dictionary<GameObject, Translucent>();
        private void SetAllToTranslucent()
        {
            Building[] buildings = FindObjectsOfType<Building>();
            dd.Clear();
            foreach (var b in buildings)
            {
                dd.Add(b.transform.GetChild(0).gameObject, new Translucent(b.transform.GetChild(0).gameObject, materialGhost));
                dd[b.transform.GetChild(0).gameObject].ChangeMaterials();
            }
            Untranslucent(_target);
        }

        private void Untranslucent(GameObject _object)
        {
            foreach (var b in dd)
            {
                if (b.Key.Equals(_object.transform.GetChild(0).gameObject))
                {
                    b.Value.Reset();
                    break;
                }
            }
        }

        private void ResetAllTranslucent()
        {
            if (dd.Count > 0)
            {
                foreach (var b in dd)
                {
                    b.Value.Reset();
                }
            }
        }

        private void RecenterCanvasUI()
        {
            canvasUI.localRotation = MainCameraPivot.rotation;
        }

        #endregion

        private void OnActivateFeature(ActiveFeature _feature)
        {
            bool _isRotate = _feature.Equals(ActiveFeature.Rotate);
            bool _isScale = _feature.Equals(ActiveFeature.Scale);
            bool _isTranslate = _feature.Equals(ActiveFeature.Translate);

            buttonRotatePlus.gameObject.SetActive(_isRotate);
            buttonRotateMinus.gameObject.SetActive(_isRotate);

            buttonScalePlus.gameObject.SetActive(_isScale);
            buttonScaleMinus.gameObject.SetActive(_isScale);

            if (_feature.Equals(ActiveFeature.Translate))
            {
                Building _dBuilding = _target.GetComponent<Building>();
                BuildingBuilder[] bb = FindObjectsOfType<BuildingBuilder>();
                foreach (var b in bb)
                {
                    if (b.BuildingInfo.Equals(_dBuilding.Info))
                    {
                        _buildingBuilder = b;
                        _isBuild = true;
                        b.ActivateTool();
                        break;
                    }
                }
                _dBuilding.Terminate();
            }
            // buttonTranslateXPlus.gameObject.SetActive(_isTranslate);
            // buttonTranslateXMinus.gameObject.SetActive(_isTranslate);
            // buttonTranslateZPlus.gameObject.SetActive(_isTranslate);
            // buttonTranslateZMinus.gameObject.SetActive(_isTranslate);
        }

        //need to change BuildingRotation "set" to public
        private void OnRotatePlus()
        {
            _target.transform.GetChild(0).localRotation = Quaternion.AngleAxis(90, Vector3.up) * _target.transform.GetChild(0).localRotation;

            _buildingRotation.State++;
            if (_buildingRotation.State > 3) _buildingRotation.State = 0;
        }

        private void OnRotateMinus()
        {
            _target.transform.GetChild(0).localRotation = Quaternion.AngleAxis(-90, Vector3.up) * _target.transform.GetChild(0).localRotation;

            _buildingRotation.State--;
            if (_buildingRotation.State < 0) _buildingRotation.State = 3;
        }


        private void OnScalePlus()
        {
            if (_target.transform.GetChild(0).localScale.x < 1)
                _target.transform.GetChild(0).localScale += new Vector3(0.1f, 0.1f, 0.1f);
        }

        private void OnScaleMinus()
        {
            if (_target.transform.GetChild(0).localScale.x > .5f)
                _target.transform.GetChild(0).localScale += new Vector3(-0.1f, -0.1f, -0.1f);
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
