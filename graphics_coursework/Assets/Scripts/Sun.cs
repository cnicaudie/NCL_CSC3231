using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Light))]
public class Sun : MonoBehaviour
{
    public Gradient ambientColor;
    public Gradient lightColor;
    public Gradient fogColor;
    public AnimationCurve intensityCurve;

    private Light m_directionalLight;

    private const float k_maxIntensity = 0.7f;
    private const float k_maxHoursInDay = 24f;

    [SerializeField, Range(0, k_maxHoursInDay)] private float m_timeOfDay;

    // ==================================

    private void Start()
    {
        m_directionalLight = GetComponent<Light>();
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            m_timeOfDay += Time.deltaTime;
            m_timeOfDay %= k_maxHoursInDay;
        }

        UpdateLighting(m_timeOfDay / k_maxHoursInDay);
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = ambientColor.Evaluate(timePercent);
        RenderSettings.fogColor = fogColor.Evaluate(timePercent);

        m_directionalLight.color = lightColor.Evaluate(timePercent);
        m_directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        m_directionalLight.intensity = intensityCurve.Evaluate(timePercent) * k_maxIntensity;
    }

    private void OnValidate()
    {
        if (m_directionalLight != null)
            return;

        if (RenderSettings.sun != null)
        {
            m_directionalLight = RenderSettings.sun;
        }
        else
        {
            Debug.LogWarning("You need to setup the sun in the render settings !");
        }
    }
}
