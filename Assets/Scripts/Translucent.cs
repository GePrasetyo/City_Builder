using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Translucent
{
    List<Material> currentMaterial;
    private GameObject _target;
    private Material _material;

    public Translucent(GameObject _gameObject, Material _mat)
    {
        _target = _gameObject;
        _material = _mat;
    }

    public void ChangeMaterials()
    {
        MeshRenderer meshRenderer = _target.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            currentMaterial = new List<Material>();
            List<Material> subs = new List<Material>();
            foreach (var data in meshRenderer.materials.Select((value, index) => new { index, value }))
            {
                currentMaterial.Add(data.value);
                subs.Add(_material);
            }
            meshRenderer.materials = subs.ToArray();
        }
    }

    public void Reset()
    {
        if (_target)
        {
            MeshRenderer meshRenderer = _target.GetComponent<MeshRenderer>();
            if (meshRenderer != null && meshRenderer.materials.Length.Equals(currentMaterial.Count))
            {
                meshRenderer.materials = currentMaterial.ToArray(); ;
            }
        }
    }
}
