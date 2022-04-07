using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace CityBuilderCore
{
    public class OnClickFocus : MonoSingleton<OnClickFocus>
    {
        private IMouseInput _mouseInput;

        public Transform MainCameraPivot;
        [SerializeField] private Transform canvasUI;

        private void Start()
        {
            _mouseInput = Dependencies.Get<IMouseInput>();
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

                var _building = Dependencies.Get<IBuildingManager>().GetBuilding(mousePosition).FirstOrDefault();
                if (_building != null)
                {
                    ManipulateTransform.Instance.SelectedBuilding(_building, worldPosition);
                    canvasUI.localRotation = MainCameraPivot.rotation;
                    StartCoroutine(Lerp(MainCameraPivot, worldPosition, 1f));

                    //FadeObstructionsManager.Instance.RegisterShouldBeVisible(_building.Root.gameObject);
                    return;
                }
            }

        }


        private IEnumerator Lerp(Transform _target, Vector3 _end, float _duration, System.Action _postAction = null)
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
    }

}
