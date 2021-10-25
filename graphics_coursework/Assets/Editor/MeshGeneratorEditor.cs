using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MeshGenerator meshGenerator = (MeshGenerator)target;

        if (DrawDefaultInspector())
        {
            if (meshGenerator.liveEditorUpdate)
            {
                meshGenerator.GenerateAndDisplay();
            }
        }

        if (GUILayout.Button("Generate Terrain"))
        {
            meshGenerator.GenerateAndDisplay();
        }
    }
}
