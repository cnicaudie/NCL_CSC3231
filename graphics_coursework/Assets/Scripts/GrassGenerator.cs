using UnityEngine;

public class GrassGenerator : MonoBehaviour
{
    // TODO : Do the same for trees

    [SerializeField] private GameObject m_grassObject;
    
    private float m_minHeight = 10f;

    private const int k_minSpacing = 1;
    private const int k_maxSpacing = 3;
    private int nextSpacing;

    public void Generate(MeshGenerator meshGenerator)
    {
        nextSpacing = k_minSpacing;
        
        MeshData meshData = meshGenerator.GetMeshData();
        Vector3[] vertices = meshData.vertices;
        
        for (int i = 0; i < vertices.Length; i += nextSpacing)
        {
            // Determine position/rotation/scale
            Vector3 vertexPosition = vertices[i];
            Vector3 worldVertexPosition = vertexPosition * 10f;
                
            if (worldVertexPosition.y < m_minHeight)
            {
                continue;
            }

            // Instantiate grass object
            GameObject grassChunk = Instantiate(m_grassObject, transform);
            grassChunk.transform.position = worldVertexPosition;

            nextSpacing = Random.Range(k_minSpacing, k_maxSpacing + 1);
        }
    }
}
