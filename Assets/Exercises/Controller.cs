using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameObject Prefab;
    public Material MyMaterial;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject instancePrefab = Instantiate(Prefab);
            instancePrefab.transform.position = new Vector3(2 * i, 0, 0);
            instancePrefab.transform.parent = this.transform;
            instancePrefab.AddComponent<Rotate3DObject>();
            if (IsVisibleFrom(instancePrefab.transform.GetComponent<Renderer>().bounds, Camera.main))
            {
                Debug.Log("INSTANCE[" + i + "] IS VISIBLE");
            }
            else
            {
                Debug.Log("INSTANCE[" + i + "] IS NOT VISIBLE");
            }
        }
        ApplyMaterialOnObjects(this.gameObject.transform.root, MyMaterial);
    }

    private void ApplyMaterialOnObjects(Transform _item, Material _material)
    {
        foreach (Transform child in _item)
        {
            ApplyMaterialOnObjects(child.gameObject.transform, _material);
        }
        if (_item.GetComponent<Renderer>() != null)
        {
            _item.GetComponent<Renderer>().material = _material;
        }
    }

    private bool IsVisibleFrom(Bounds _bounds, Camera _camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_camera);
        return GeometryUtility.TestPlanesAABB(planes, _bounds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
