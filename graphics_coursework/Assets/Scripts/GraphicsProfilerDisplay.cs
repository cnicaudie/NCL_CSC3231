using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;

public class GraphicsProfilerDisplay : MonoBehaviour
{
    public GameObject canvas;
    public Text fpsText;
    public Text memoryText;

    public bool displayStats = true;

    private float m_deltaTime;

    // ==================================

    // ==================================
    // PRIVATE METHODS
    // ==================================

    private void Start()
    {
        ToggleCanvas();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D)) // Debug
        {
            ToggleCanvas();
        }

        UpdateFPS();
        UpdateMemoryUsage();
    }

    private void ToggleCanvas()
    {
        canvas.SetActive(displayStats && !canvas.activeSelf);
    }

    // Note : Around 20/25 FPS is ok
    private void UpdateFPS()
    {
        m_deltaTime += (Time.deltaTime - m_deltaTime) * 0.1f;
        float fps = 1.0f / m_deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString() + " FPS";
    }

    private void UpdateMemoryUsage()
    {
        const long megaByte = 1048576;
        long memoryUsage = Profiler.GetTotalAllocatedMemoryLong() / megaByte;
        memoryText.text = Mathf.Ceil(memoryUsage).ToString() + " MB";
    }
}
