using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    [SerializeField] private GameObject m_object;
    
    [Range(1f, 10f)]
    public float radius = 5f;
    
    [SerializeField] private float m_minHeight = 13f;
    [Range(0f, 180f)]
    public float maxAngle = 45f;

    // TODO Remove debug tools later
    public bool displayGizmos = false;
    public Color debugColor = Color.green;
    private List<Vector2> m_generatedDebugPositions;
    private bool m_debugPositionGenerated = false;

    // ==================================

    // ==================================
    // PUBLIC METHODS
    // ==================================

    /// <summary>
    /// Spawns objects on the mesh based on various parameters
    /// and some randomized variables
    /// </summary>
    public void Generate(MeshGenerator meshGenerator)
    {
        if (m_object == null)
        {
            Debug.LogWarning("Object to spawn hasn't been defined - " + gameObject.name);
            return;
        }

        MeshData meshData = meshGenerator.GetMeshData();

        List<Vector2> positions = GenerateSpawnPositionsList(new Vector2(meshData.meshWidth * 10f, meshData.meshDepth * 10f));

        int layerMask = ~LayerMask.NameToLayer("Terrain");

        foreach (Vector2 position in positions)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-1.75f, 1.75f), 0f, Random.Range(-1.75f, 1.75f));
            Vector3 origin = new Vector3(position.x, 200f, position.y) + randomOffset;

            Ray ray = new Ray(origin, Vector3.down);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask))
            {
                // Get height corresponding to the generated x / z position
                Vector3 newPosition = hitInfo.point;
                Vector3 hitNormal = hitInfo.normal;

                float slopeAngle = Vector3.Angle(Vector3.up, hitNormal);
                
                // Filter out position below minimum height / slope angle
                if (newPosition.y > m_minHeight && Mathf.Abs(slopeAngle) <= maxAngle)
                {
                    // Instantiate object and position it
                    GameObject go = Instantiate(m_object, transform);
                    go.transform.position = newPosition;

                    // Rotate the element so that it lies correctly on the surface
                    go.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitNormal);

                    // Apply random upward rotation and scale
                    Vector3 randomRotation = new Vector3(0f, Random.Range(0f, 360f), 0f);
                    float randomScaleMultiplier = Random.Range(0.75f, 1.25f);
                    
                    go.transform.up += randomRotation;
                    go.transform.localScale *= randomScaleMultiplier;
                }
            }
        }
    }

    // ==================================
    // PRIVATE METHODS
    // ==================================


    /// <summary>
    /// Generates a list of XZ positions whithin a region size
    /// using the Poisson Disc algorithm
    /// </summary>
    private List<Vector2> GenerateSpawnPositionsList(Vector2 sampleRegionSize)
    {
        float cellSize = radius / Mathf.Sqrt(2);
        int numSamplesBeforeRejection = 20;

        int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];

        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();

        spawnPoints.Add(sampleRegionSize / 2);

        while (spawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCentre = spawnPoints[spawnIndex];
            bool wasCandidateAccepted = false;

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCentre + dir * Random.Range(radius, 2 * radius);

                if (IsPositionValid(candidate, sampleRegionSize, cellSize, points, grid))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    wasCandidateAccepted = true;
                    break;
                }
            }

            if (!wasCandidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return points;
    }


    /// <summary>
    /// Checks if candidate point is whithin the bounds of the sample region
    /// and check that it isn't inside of another point's radius
    /// </summary>
    private bool IsPositionValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, List<Vector2> points, int[,] grid)
    {
        if (candidate.x >= 0 && candidate.x < sampleRegionSize.x
            && candidate.y >= 0 && candidate.y < sampleRegionSize.y)
        {
            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);

            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y <= searchEndY; y++)
                {
                    int pointIndex = grid[x, y] - 1;

                    if (pointIndex != -1)
                    {
                        float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;

                        if (sqrDst < Mathf.Pow(radius, 2))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        return false;
    }

    // TODO : Remove debug tools later
    private void GeneratePositions()
    {
        MeshGenerator meshGenerator = FindObjectOfType<MeshGenerator>();

        if (meshGenerator != null)
        {
            MeshData meshData = meshGenerator.GetMeshData();

            if (meshData == null)
            {
                meshGenerator.GenerateAndDisplay();
                meshData = meshGenerator.GetMeshData();
            }

            m_generatedDebugPositions = GenerateSpawnPositionsList(new Vector2(meshData.meshWidth * 10f, meshData.meshDepth * 10f));
        }

        m_debugPositionGenerated = true;
    }

    private void OnValidate()
    {
        if (displayGizmos)
        {
            GeneratePositions();
        }
    }

    private void DrawPointGizmos(Vector3 position)
    {
        Gizmos.color = debugColor;
        Gizmos.DrawSphere(position, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        if (displayGizmos)
        {
            if (!m_debugPositionGenerated)
            {
                GeneratePositions();
            }

            foreach (Vector2 position in m_generatedDebugPositions)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-1.75f, 1.75f), 0f, Random.Range(-1.75f, 1.75f));
                Vector3 origin = new Vector3(position.x, 200f, position.y) + randomOffset;

                Ray ray = new Ray(origin, Vector3.down);
                RaycastHit hitInfo;
                
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, ~LayerMask.NameToLayer("Terrain")))
                {
                    // Get height corresponding to the generated x / z position
                    Vector3 newPosition = hitInfo.point;
                    Vector3 hitNormal = hitInfo.normal;

                    float slopeAngle = Vector3.Angle(Vector3.up, hitNormal);

                    // Filter out position below minimum height / slope angle
                    if (newPosition.y > m_minHeight && Mathf.Abs(slopeAngle) <= maxAngle)
                    {
                        DrawPointGizmos(newPosition);
                    }
                }
            }
        }

    }
}
