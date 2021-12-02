using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    private Transform[] m_children;
    private float m_rotationSpeed = 10f;

    // ==================================

    // ==================================
    // PRIVATE METHODS
    // ==================================

    private void Start()
    {
        m_children = GetComponentsInChildren<Transform>();   
    }

    private void Update()
    {
        foreach (Transform child in m_children)
        {
            child.RotateAround(transform.position, Vector3.up, m_rotationSpeed * Time.deltaTime);
        }
    }
}
