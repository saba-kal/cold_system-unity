using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FogOfWarVolumeGenerator))]
public class FogOfWarVolumeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var volumeGenerator = (target as FogOfWarVolumeGenerator);

        var texture = volumeGenerator.GetHeightMap();

        if (texture != null)
        {
            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(20));
            EditorGUILayout.LabelField("Height Map Preview:", EditorStyles.boldLabel);

            GUI.DrawTexture(GUILayoutUtility.GetRect(300, 300), texture, ScaleMode.ScaleToFit);
        }

        if (GUILayout.Button("Wrap Fog to Terrain"))
        {
            volumeGenerator.WrapFogToTerrain();
        }
    }
}
