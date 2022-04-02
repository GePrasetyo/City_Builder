using UnityEngine;
using CityBuilderCore;

public class AdjusterFoundation : MonoBehaviour
{
    [SerializeField] private Transform foundation, pivot;
    [SerializeField] private Building buildingComp;

    private void Start() {
        var size = buildingComp.RawSize;

        foundation.localScale = new Vector3(size.x-0.2f, 1, size.y - 0.2f);
        foundation.localPosition = new Vector3(size.x/2, 0, size.y/2);

        var objChild = ModelFileLoader.instance._loadedGameObject;
        objChild.transform.SetParent(pivot);
        objChild.transform.localPosition = Vector3.zero;

    }
}
