using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectLinearBuoyancy : MonoBehaviour
{
    public Transform[] floaters;
    public Transform water;

    public float underwaterDrag = 4f;
    public float underwaterAngularDrag = 1f;
    public float defaultDrag = 0f;
    public float defaultAngularDrag = 0.05f;

    public float floatingForce = 50f;

    private Rigidbody m_rigidbody;
    private float m_waterHeight;
    private bool m_isUnderwater;
    private int m_floatersUnderwaterCount;

    // ==================================

    // ==================================
    // PRIVATE METHODS
    // ==================================

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_waterHeight = water.position.y;
    }

    private void FixedUpdate()
    {
        m_floatersUnderwaterCount = 0;

        foreach (Transform floater in floaters)
        {

            float heightDifference = floater.position.y - m_waterHeight;

            if (heightDifference < 0f)
            {
                m_rigidbody.AddForceAtPosition(Vector3.up * floatingForce * Mathf.Abs(heightDifference), floater.position);
                m_floatersUnderwaterCount += 1;

                if (!m_isUnderwater)
                {
                    ChangeState(true);
                }
            }
        }

        if (m_isUnderwater && m_floatersUnderwaterCount == 0)
        {
            ChangeState(false);
        }
    }

    private void ChangeState(bool isUnderwater)
    {
        m_isUnderwater = isUnderwater;

        m_rigidbody.drag = isUnderwater ? underwaterDrag : defaultDrag;
        m_rigidbody.angularDrag = isUnderwater ? underwaterAngularDrag : defaultAngularDrag;
    }
}
