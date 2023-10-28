using UnityEditor;

[CustomEditor(typeof(FogOfWarShaderManager))]
public class FogOfWarManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var fogOfWarShaderManager = (target as FogOfWarShaderManager);
        fogOfWarShaderManager.UpdateFogOfWarGlobals();
    }
}
