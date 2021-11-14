using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    [SerializeField]
    private MeshFilter m_meshFilter;
    // TODO : [SerializeField] private MeshRenderer m_meshRenderer;
    private MeshData m_meshData;

    private float m_maxTerrainHeight;
    private float m_minTerrainHeight;

    public int meshWidth;
    public int meshDepth;

    [Header("Noise Settings")]
    public float noiseScale;
    public Vector2 noiseOffset;
    public float heightMultplier; 
    public AnimationCurve heightCurve;
    public int seed;
    public int octaves; // different levels of details of the height map
    public float lacunarity; // controls increase in frequency of octaves
    [Range(0f, 1f)]
    public float persistance; // controls decrease in amplitude of octaves

    [Header("Editor Update")]
    public bool liveEditorUpdate;

    // ==================================

    private void Start()
    {
        if (!m_meshFilter)
        {
            m_meshFilter = GetComponent<MeshFilter>();
        }

        // TODO : m_meshRenderer = GetComponent<MeshRenderer>();

        GenerateAndDisplay();
    }

    public void GenerateAndDisplay()
    {
        if (noiseScale <= 0f)
        {
            noiseScale = 0.0001f;
        }

        m_maxTerrainHeight = float.MinValue;
        m_minTerrainHeight = float.MaxValue;

        GenerateTerrainMesh();
        DrawMesh();
    }

    /// <summary>
    /// Creates the final mesh
    /// </summary>
    private Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

        mesh.SetVertices(m_meshData.vertices);
        mesh.SetTriangles(m_meshData.triangles, 0);
        // TODO : mesh.uv = m_meshData.uvs;

        mesh.RecalculateNormals();

        return mesh;
    }

    /// <summary>
    /// Draws the final mesh
    /// </summary>
    private void DrawMesh()
    {
        m_meshFilter.mesh = CreateMesh();

        // TODO : update m_meshRenderer.material.mainTexture
    }

    /// <summary>
    /// Generates a randomized offset for each octaves
    /// </summary>
    private Vector2[] GetOctaveOffsets()
    {
        const int randomMaxValue = 100000;
        Vector2[] octaveOffsets = new Vector2[octaves];
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-randomMaxValue, randomMaxValue) + noiseOffset.x;
            float offsetY = prng.Next(-randomMaxValue, randomMaxValue) + noiseOffset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        return octaveOffsets;
    }

    /// <summary>
    /// Generates a height value from perlin noise sample method
    /// </summary>
    private float GetNoiseHeightSample(int x, int z, Vector2[] octaveOffsets)
    {
        float noiseHeight = 0f;
        float noiseFrequency = 1f;
        float noiseAmplitude = 1f;

        // Compute the height and fine-tune it through each octaves
        for (int i = 0; i < octaves; i++)
        {
            float sampleX = x / noiseScale * noiseFrequency + octaveOffsets[i].x;
            float sampleZ = z / noiseScale * noiseFrequency + octaveOffsets[i].y;

            float perlinNoiseValue = Mathf.PerlinNoise(sampleX, sampleZ);
            perlinNoiseValue = perlinNoiseValue * 2 - 1; // value between -1 and 1

            noiseHeight += perlinNoiseValue * noiseAmplitude;

            noiseAmplitude *= persistance;
            noiseFrequency *= lacunarity;
        }

        // Compute min/max height of the current terrain
        if (noiseHeight > m_maxTerrainHeight)
        {
            m_maxTerrainHeight = noiseHeight;
        }
        else if (noiseHeight < m_minTerrainHeight)
        {
            m_minTerrainHeight = noiseHeight;
        }

        return Mathf.InverseLerp(m_minTerrainHeight, m_maxTerrainHeight, noiseHeight);
    }

    /// <summary>
    /// Generates the terrain mesh by creating every triangle and assigning a height position
    /// from the sampled height map (using noise)
    /// </summary>
    private void GenerateTerrainMesh()
    {
        m_meshData = new MeshData(meshWidth, meshDepth);
        
        int vertexIndex = 0;

        Vector2[] octaveOffsets = GetOctaveOffsets();

        for (int z = 0; z < meshDepth; z++)
        {
            for (int x = 0; x < meshWidth; x++)
            {
                // Value between 0 and 1
                float height = GetNoiseHeightSample(x, z, octaveOffsets);

                // The evaluate method from the animation curve allows to discard some heights
                // and the multiplier emphasizes the height value
                float finalHeight = heightCurve.Evaluate(height) * heightMultplier;

                m_meshData.vertices[vertexIndex] = new Vector3(x, finalHeight, z);

                if (x < (meshWidth - 1) && z < (meshDepth - 1))
                {
                    // Build a quad (= 2 triangles)
                    m_meshData.AddTriangle(vertexIndex + meshWidth, vertexIndex + meshWidth + 1, vertexIndex);
                    m_meshData.AddTriangle(vertexIndex + 1, vertexIndex, vertexIndex + meshWidth + 1);
                }

                vertexIndex++;
            }
        }
    }
}
