using UnityEngine;

public class MassAssignMaterial : MonoBehaviour
{
    [SerializeField] private Material _material;

    public void AssignMaterial()
    {
        if (_material == null)
        {
            Debug.LogError("Please assign a material in the Material field");
            return;
        }

        foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.material = _material;
        }
    }
}
