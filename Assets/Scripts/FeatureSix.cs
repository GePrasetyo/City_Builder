using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 6. Auto save while u work 
// But they don't have saving management, cannot save multiple file. 1 PC can only save 1 scene. If you need that as well, it could be 4 days

namespace CityBuilderCore
{
    public class FeatureSix : MonoBehaviour
    {

        [SerializeField] private Button buttonSave;
        [SerializeField] private TMP_InputField inputFieldSave;
        [SerializeField] private Button buttonLoad;
        [SerializeField] private Button buttonLoadCloseList;
        [SerializeField] private GameObject gameObjectLoadFileMenu;
        [SerializeField] private Transform transformLoadFileContainer;
        [SerializeField] private GameObject gameObjectLoadFileModel;
        [SerializeField] private Toggle toggleAutoSave;


        public float autoSavePeriode = 1800;
        private float _currentTimeToSave = 0;
        private string _saveFileName = "default";
        private string _loadFileName;
        private bool _isAutoSave = false;
        List<string> savedFile = new List<string>();
        List<GameObject> savedGameobject = new List<GameObject>();

        private bool _isLoadList = false;


        // Start is called before the first frame update
        void Start()
        {
            LoadDataList();


            gameObjectLoadFileMenu.SetActive(false);
            toggleAutoSave.onValueChanged.AddListener(delegate { IsAutoSave(toggleAutoSave.isOn); });
            buttonSave.onClick.AddListener(() => OnButtonSaveClicked());
            buttonLoad.onClick.AddListener(() => OnButtonLoadClicked());
            buttonLoadCloseList.onClick.AddListener(() => OnCloseLoadMenu());
        }

        // Update is called once per frame
        void Update()
        {
            if (_isAutoSave)
            {
                _currentTimeToSave -= Time.deltaTime;
                if (_currentTimeToSave <= 0)
                {
                    OnButtonSaveClicked();
                    _currentTimeToSave = autoSavePeriode;
                }
            }
        }

        private void IsAutoSave(bool _status)
        {
            if (_status)
            {
                _currentTimeToSave = autoSavePeriode;
                _isAutoSave = true;
            }
            else
            {
                _isAutoSave = false;
            }
        }

        private void OnButtonSaveClicked()
        {
            if (string.IsNullOrWhiteSpace(inputFieldSave.text)) _saveFileName = "default";
            else _saveFileName = inputFieldSave.text;

            FindObjectOfType<DefaultGameManager>().Save(_saveFileName);

            bool isNew = true;
            foreach (var name in savedFile)
            {
                if (name.Equals(_saveFileName))
                {
                    isNew = false;
                    break;
                }
            }
            if (isNew)
                savedFile.Add(_saveFileName);


            inputFieldSave.text = "";
            SaveDataList();
        }

        private void OnButtonLoadClicked()
        {
            if (!_isLoadList)
            {
                foreach (var s in savedFile)
                {
                    GameObject ls = Instantiate(gameObjectLoadFileModel, Vector3.zero, Quaternion.identity);
                    ls.transform.parent = transformLoadFileContainer;

                    TMP_Text _text = ls.transform.GetComponentInChildren<TMP_Text>();
                    _text.text = s;
                    ls.transform.localScale = new Vector3(1, 1, 1);
                    ls.GetComponent<Button>().onClick.AddListener(() => OnSelectedSavedFile(s));
                    ls.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => OnDeleteData(s));

                    ls.SetActive(true);
                    savedGameobject.Add(ls);

                }
                gameObjectLoadFileMenu.SetActive(true);
                if (savedFile.Count < 1)
                    StartCoroutine(SetObjectMode(gameObjectLoadFileMenu, 2, false));
                else
                    buttonLoad.interactable = false;
            }
            else
            {
                FindObjectOfType<DefaultGameManager>().Load(_loadFileName);
                buttonLoad.GetComponentInChildren<TMP_Text>().text = $"Load";
                _isLoadList = false;
            }
        }

        private void OnDeleteData(string _data)
        {
            foreach (var d in savedFile)
            {
                if (_data.Equals(d)) savedFile.Remove(d);
            }
            SaveDataList();
            OnCloseLoadMenu();
        }

        private void OnCloseLoadMenu()
        {
            gameObjectLoadFileMenu.SetActive(false);
            buttonLoad.interactable = true;

            foreach (var go in savedGameobject)
            {
                Destroy(go);
            }
            savedGameobject.Clear();
        }

        IEnumerator SetObjectMode(GameObject _object, float _time, bool _status)
        {
            yield return new WaitForSeconds(_time);
            _object.SetActive(_status);
        }

        private void OnSelectedSavedFile(string _fileName)
        {
            _isLoadList = true;
            _loadFileName = _fileName;
            foreach (var go in savedGameobject)
            {
                Destroy(go);
            }
            savedGameobject.Clear();
            buttonLoad.GetComponentInChildren<TMP_Text>().text = $"Load {_loadFileName}";
            gameObjectLoadFileMenu.SetActive(false);
            buttonLoad.interactable = true;
        }

        private void SaveDataList()
        {
            string data = null;
            foreach (var s in savedFile)
            {
                if (string.IsNullOrWhiteSpace(data)) data += s;
                else data += $",{s}";
            }
            PlayerPrefs.SetString($"SAVE_TEMP_LIST", data);
            PlayerPrefs.Save();
        }

        private void LoadDataList()
        {
            savedFile = new List<string>();
            savedFile.Clear();
            if (!string.IsNullOrWhiteSpace(PlayerPrefs.GetString($"SAVE_TEMP_LIST")))
            {
                string[] data = PlayerPrefs.GetString($"SAVE_TEMP_LIST").Split(',');
                savedFile = data.ToList<string>();
            }
        }

    }

}
