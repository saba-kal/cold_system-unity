using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(FogOfWarMeshGenerator))]
public class FogOfWarMeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var meshGenerator = (target as FogOfWarMeshGenerator);

        if (GUILayout.Button("Generate fog of war mesh."))
        {
            meshGenerator.InitializeHeightAdjustedFog();
        }
    }
}