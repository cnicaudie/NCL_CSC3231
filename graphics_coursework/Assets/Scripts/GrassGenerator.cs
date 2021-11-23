using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrassGenerator : MonoBehaviour
{
    // TODO : Generalize the generator to work with other object types
    
    [SerializeField] private GameObject m_grassObject;

    [Range(1f, 10f)]
    public float radius = 5f;
    public int numSamplesBeforeRejection = 300;

    [SerializeField] private float m_minHeight = 13f;
    [Range(0f, 180f)]
    public float maxAngle = 45f;

    // TODO Remove debug tools later
    public bool displayGizmos = false;
    private List<Vector2> m_grassChunksPositions;
    private bool m_positionGenerated = false;

    public void Generate(MeshGenerator meshGenerator)
    {
        MeshData meshData = meshGenerator.GetMeshData();

        List<Vector2> positions = GeneratePositionList(new Vector2(meshData.meshWidth * 10f, meshData.meshDepth * 10f));

        int layerMask = ~LayerMask.NameToLayer("Terrain");

        foreach (Vector2 position in positions)
        {
            // TODO : Apply a randomized offset

            Vector3 origin = new Vector3(position.x, 200f, position.y);
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
                    // TODO : Apply a randomized Y rotation

                    // TODO : Apply a randomized scale
                    
                    // Create GameObject and apply everything
                    GameObject grassChunk = Instantiate(m_grassObject, transform);
                    grassChunk.transform.position = newPosition;
                    grassChunk.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitNormal);
                }
            }
        }
    }

    public List<Vector2> GeneratePositionList(Vector2 sampleRegionSize)
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
            bool candidateAccepted = false;

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
                    candidateAccepted = true;
                    break;
                }
            }

            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }

        }

        return points;
    }

    private bool IsPositionValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, List<Vector2> points, int[,] grid)
    {
        if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y)
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

            m_grassChunksPositions = GeneratePositionList(new Vector2(meshData.meshWidth * 10f, meshData.meshDepth * 10f));
        }

        m_positionGenerated = true;

        Debug.Log("Generated Positions !");
    }

    private void OnValidate()
    {
        if (displayGizmos)
        {
            GeneratePositions();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (displayGizmos)
        {
            if (!m_positionGenerated)
            {
                GeneratePositions();
            }

            foreach (Vector2 position in m_grassChunksPositions)
            {
                Vector3 origin = new Vector3(position.x, 200f, position.y);
                Ray ray = new Ray(origin, Vector3.down);
                RaycastHit hitInfo;
                int layerMask = ~LayerMask.NameToLayer("Terrain");

                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask))
                {
                    Vector3 newPosition = hitInfo.point;
                    Vector3 hitNormal = hitInfo.normal;

                    float slopeAngle = Vector3.Angle(Vector3.up, hitNormal);

                    if (newPosition.y > m_minHeight && Mathf.Abs(slopeAngle) <= maxAngle)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(newPosition, 1f);
                    }
                }
            }
        }

    }
}
