using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Light))]
public class LightManager : MonoBehaviour
{
    public Gradient ambientColor;
    public Gradient lightColor;
    public Gradient fogColor;
    public AnimationCurve intensityCurve;

    public enum TimeBase { HOURS, MINUTES, SECONDS }
    public TimeBase timeBase;

    private Light m_sunLight;
    private const float k_maxIntensity = 0.7f;
    private const float k_maxHoursInDay = 24f;

    [SerializeField, Range(0, k_maxHoursInDay)] private float m_timeOfDay;
    [SerializeField] private float m_timeSpeed;

    // ==================================

    private void Start()
    {
        m_sunLight = GetComponent<Light>();

        switch (timeBase)
        {
            case TimeBase.HOURS:
                m_timeSpeed = 3600;
                break;
            case TimeBase.MINUTES:
                m_timeSpeed = 60;
                break;
            case TimeBase.SECONDS:
                m_timeSpeed = 1;
                break;
        }
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            m_timeOfDay += 1 / m_timeSpeed * Time.deltaTime;
            m_timeOfDay %= k_maxHoursInDay;
        }

        UpdateLighting(m_timeOfDay / k_maxHoursInDay);
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = ambientColor.Evaluate(timePercent);
        RenderSettings.fogColor = fogColor.Evaluate(timePercent);

        m_sunLight.color = lightColor.Evaluate(timePercent);
        m_sunLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        m_sunLight.intensity = intensityCurve.Evaluate(timePercent) * k_maxIntensity;
    }

    private void OnValidate()
    {
        if (m_sunLight != null)
            return;

        if (RenderSettings.sun != null)
        {
            m_sunLight = RenderSettings.sun;
        }
        else
        {
            Debug.LogWarning("You need to setup the sun in the render settings !");
        }
    }
}
