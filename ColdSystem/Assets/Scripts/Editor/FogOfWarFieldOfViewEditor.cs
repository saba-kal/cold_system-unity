using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfViewMeshGenerator))]
public class FogOfWarFieldOfViewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var fieldOfView = (target as FieldOfViewMeshGenerator);

        var texture = fieldOfView.VisionTexture;

        if (texture != null)
        {
            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(20));
            EditorGUILayout.LabelField("Vision Preview:", EditorStyles.boldLabel);

            GUI.DrawTexture(GUILayoutUtility.GetRect(300, 300), texture, ScaleMode.ScaleToFit);
        }

    }
}