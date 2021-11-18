using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MeshGenerator meshGenerator = (MeshGenerator)target;

        if ((DrawDefaultInspector() && meshGenerator.liveEditorUpdate) || GUILayout.Button("Generate Terrain"))
        {
            meshGenerator.GenerateAndDisplay();
        }
    }
}
