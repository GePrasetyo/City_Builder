using UnityEngine;

public class CameraSwitchManager : MonoBehaviour
{
    public GameObject mainCam, fpsCam;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.C)) {
            mainCam.SetActive(!mainCam.activeInHierarchy);
            Cursor.lockState = CursorLockMode.None;
            fpsCam.SetActive(!fpsCam.activeInHierarchy);

        }
    }
}
