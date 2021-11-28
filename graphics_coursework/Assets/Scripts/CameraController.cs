using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 100f;
    public float sensitivity = 0.25f;
    private Vector3 m_mousePosition;

    private float m_horizontalInput;
    private float m_verticalInput;

    // ==================================

    // ==================================
    // PRIVATE METHODS
    // ==================================

    private void Start()
    {

        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2);
        m_mousePosition = screenCenter;
    }

    private void Update()
    {
        GetInputs();
        RotateCamera();
        TranslateCamera();
    }

    private void GetInputs()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");
    }

    private void RotateCamera()
    {
        if (IsMouseInScreen())
        {
            Vector3 mousePositionOffset = Input.mousePosition - m_mousePosition;
            Vector3 nextCameraAngle = new Vector3(-mousePositionOffset.y, mousePositionOffset.x, 0f);

            nextCameraAngle.x += transform.eulerAngles.x;
            nextCameraAngle.y += transform.eulerAngles.y;

            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, nextCameraAngle, sensitivity);

            m_mousePosition = Input.mousePosition;
        }
    }

    private void TranslateCamera()
    {
        Vector3 cameraDirection = new Vector3(m_horizontalInput, 0f, m_verticalInput);

        transform.Translate(speed * cameraDirection * Time.deltaTime);
    }

    private bool IsMouseInScreen()
    {
        Vector3 mousePosition = Input.mousePosition;

        if (mousePosition.x <= 0 || mousePosition.y <= 0)
        {
            return false;
        }
#if UNITY_EDITOR
        else if (mousePosition.x >= Handles.GetMainGameViewSize().x - 1
                || mousePosition.y >= Handles.GetMainGameViewSize().y - 1)
        {
            return false;
        }
#else
        else if (mousePosition.x >= Screen.width - 1
                || mousePosition.y >= Screen.height - 1)
        {
            return false;
        }
#endif
        else
        {
            return true;
        }
    }
}
