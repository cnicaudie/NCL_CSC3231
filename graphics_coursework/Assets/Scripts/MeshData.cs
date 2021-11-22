using UnityEngine;

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Color[] colors;

    public int meshWidth;
    public int meshDepth;

    private int triangleIndex;

    // ==================================

    public MeshData(int meshWidth, int meshDepth)
    {
        int nbVertices = meshWidth * meshDepth;
        int nbTriangleIndices = (meshWidth - 1) * (meshDepth - 1) * 6;

        vertices = new Vector3[nbVertices];
        triangles = new int[nbTriangleIndices];
        colors = new Color[nbVertices];

        this.meshWidth = meshWidth;
        this.meshDepth = meshDepth;

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