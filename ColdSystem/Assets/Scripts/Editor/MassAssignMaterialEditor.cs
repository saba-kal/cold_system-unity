using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MassAssignMaterial))]
public class MassAssignMaterialEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var massAssignMaterial = (target as MassAssignMaterial);

        if (GUILayout.Button("Assign Material"))
        {
            massAssignMaterial.AssignMaterial();
        }
    }
}
