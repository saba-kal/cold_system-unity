using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FogOfWarVisuals))]
public class FogOfWarVisualsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var fogVisuals = (target as FogOfWarVisuals);

        if (fogVisuals.Texture != null)
        {
            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(20));
            EditorGUILayout.LabelField("Height Map Preview:", EditorStyles.boldLabel);

            GUI.DrawTexture(GUILayoutUtility.GetRect(300, 300), fogVisuals.Texture, ScaleMode.ScaleToFit);
        }

        if (GUILayout.Button("Initialize Fog"))
        {
            fogVisuals.InitializeFog();
        }
    }
}
