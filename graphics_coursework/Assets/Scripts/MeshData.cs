using UnityEngine;

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    // TODO : public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        int nbVertices = meshWidth * meshHeight;
        int nbTriangleIndices = (meshWidth - 1) * (meshHeight - 1) * 6;

        vertices = new Vector3[nbVertices];
        triangles = new int[nbTriangleIndices];
        triangleIndex = 0;
    }

    public void AddTriangle(int t1, int t2, int t3)
    {
        triangles[triangleIndex] = t1;
        triangles[triangleIndex + 1] = t2;
        triangles[triangleIndex + 2] = t3;
        triangleIndex += 3;
    }
}