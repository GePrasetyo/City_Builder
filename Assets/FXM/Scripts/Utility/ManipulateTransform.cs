using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace CityBuilderCore
{
    public class ManipulateTransform : MonoSingleton<ManipulateTransform>
    {
        private enum ActiveFeature { None, Rotate, Scale, Translate }
        private ActiveFeature activeFeature;
        public IBuilding _building { get; set; }
        private GameObject _target;
        private bool _isBuild = false;
        private float maxScaleBuilding = 1.0f, minScaleBuilding = .5f;
        private BuildingBuilder _buildingBuilder;
        private BuildingRotation _buildingRotation;
        private Vector3 offset = new Vector3(-.4f, 1.0f, 0f);


        [Header("UI")]
        [SerializeField] private GameObject toolMaster;
        [SerializeField] private TMP_Text objectName;
        [SerializeField] private Button buttonClose;
        [SerializeField] private Button buttonRotate;
        [SerializeField] private Button buttonRotatePlus;
        [SerializeField] private Button buttonRotateMinus;

        [SerializeField] private Button buttonScale;
        [SerializeField] private Button buttonScalePlus;
        [SerializeField] private Button buttonScaleMinus;

        [SerializeField] private Button buttonTranslate;

        private void Start()
        {
            toolMaster.SetActive(false);
            buttonClose.onClick.AddListener(() => toolMaster.SetActive(false));

            //rotation
            buttonRotatePlus.onClick.AddListener(() => OnRotatePlus());
            buttonRotateMinus.onClick.AddListener(() => OnRotateMinus());

            //scale
            buttonScalePlus.onClick.AddListener(() => OnScalePlus());
            buttonScaleMinus.onClick.AddListener(() => OnScaleMinus());

            buttonRotate.onClick.AddListener(() => OnActivateFeature(ActiveFeature.Rotate));
            buttonScale.onClick.AddListener(() => OnActivateFeature(ActiveFeature.Scale));
            buttonTranslate.onClick.AddListener(() => OnActivateFeature(ActiveFeature.Translate));
        }

        private void Update()
        {
            if (_target == null || Input.GetMouseButtonDown(1))
            {
                toolMaster.SetActive(false);
            }

            if (_isBuild && Input.GetMouseButtonDown(1))
            {
                _buildingBuilder.DeactivateTool();
                _isBuild = false;
            }
        }

        public void SelectedBuilding(IBuilding _b, Vector3 worldPosition)
        {
            _building = _b;
            toolMaster.transform.position = worldPosition += offset;
            objectName.text = _building.GetName();
            _target = _building.Root.gameObject;

            // maxScaleBuilding = _b.Info.Size.x;
            // minScaleBuilding = _b.Info.Size.x / 2;

            _buildingRotation = _target.GetComponent<Building>().Rotation;
            toolMaster.SetActive(true);
        }

        /// <summary>
        /// enable selected feature and disable others
        /// </summary>
        /// <param name="_feature">selected feature</param>

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
        }

        #region  rotation

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
        #endregion

        #region scale
        private void OnScalePlus()
        {
            if (_target.transform.GetChild(0).localScale.x < maxScaleBuilding)
                _target.transform.GetChild(0).localScale += new Vector3(0.1f, 0.1f, 0.1f);
        }

        private void OnScaleMinus()
        {
            if (_target.transform.GetChild(0).localScale.x > minScaleBuilding)
                _target.transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
        }

        #endregion

    }
}
