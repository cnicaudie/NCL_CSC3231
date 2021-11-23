using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private MeshGenerator m_meshGenerator;
    [SerializeField] private GrassGenerator m_grassGenerator;

    private void Start()
    {
        m_meshGenerator.GenerateAndDisplay();

        // TODO : Fetch all object generators and loop through all of them
        m_grassGenerator.Generate(m_meshGenerator);
    }
}
