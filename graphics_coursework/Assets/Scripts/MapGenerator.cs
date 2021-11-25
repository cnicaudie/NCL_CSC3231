using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private MeshGenerator m_meshGenerator;
    [SerializeField] private ObjectGenerator[] m_objectGenerators;

    // ==================================

    // ==================================
    // PRIVATE METHODS
    // ==================================

    private void Start()
    {
        m_meshGenerator.GenerateAndDisplay();

        m_objectGenerators = FindObjectsOfType<ObjectGenerator>();

        foreach (ObjectGenerator objectGenerator in m_objectGenerators)
        {
            objectGenerator.Generate(m_meshGenerator);
        }
    }
}
