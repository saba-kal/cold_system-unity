using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FogOfWar))]
public class FogOfWarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var fogOfWar = (target as FogOfWar);

        if (GUILayout.Button("Initialize fog."))
        {
            fogOfWar.InitializeHeightAdjustedFog();
        }
    }
}
