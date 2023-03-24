using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimeSet : MonoBehaviour
{
    public GameObject Sun;
    public GameObject Moon;
    private GameObject CenterPoint;
    [HideInInspector] public float _Time = 6.0f;
    public float _TimeSetSpeed = 1.0f;
    public float SunLightIntensity = 1.5f;
    public float MoonLightIntensity = 0.5f;
    [Range(-1f, 1f)] public float RotationAxisBias = 0;

    public Color SunRiseColor = new Color(1.0f, 0.75f, 0.5f);
    public Color DayColor = new Color(1.0f, 0.95f, 0.9f);
    public Color MoonRiseColor = new Color(0.5f, 0.5f, 0.5f);
    public Color NightColor = new Color(0.6f, 0.8f, 1.0f);
    [Range(0f, 10f)]public float AmbientIntensity = 1f;

    private float LightingColorBrightness = 1f;
    private float LightingColorSaturate = 1f;
    private Vector3 RotationAxis;
    private float SunAngleH1 = 0.0f;
    private float SunAngleH2 = 0.0f;
    private Light SunLightComponent = null;
    private Light MoonLightComponent = null;
    private Color LightColor;
    private Color AmbientColor;
    private float LightColorGrayScale;
    private float _TimeMapping;

    void Start()
    {
        CenterPoint = new GameObject("Center");
        CenterPoint.transform.localEulerAngles = new Vector3(360, 0, 0);
        Sun.transform.position = CenterPoint.transform.position + new Vector3(0, 0, 1);
        Sun.transform.LookAt(CenterPoint.transform);
        Sun.transform.SetParent(CenterPoint.transform);
        Moon.transform.position = CenterPoint.transform.position + new Vector3(0, 0, -1);
        Moon.transform.LookAt(CenterPoint.transform);
        Moon.transform.SetParent(CenterPoint.transform);
        SunLightComponent = Sun.GetComponent<Light>();
        MoonLightComponent = Moon.GetComponent<Light>();

    }
    void FixedUpdate()
    {
        set_Time();
        doRotation();
        doLightSwitch();
    }

    float smoothstep(float t1, float t2, float x)
    {
        x = Mathf.Clamp((x - t1) / (t2 - t1), 0.0f, 1.0f);
        return x * x * (3 - 2 * x);
    }
    void set_Time()
    {
        if (_Time > 24f)
        {
            _Time = 0f;
        }
        if (_Time < 0f)
        {
            _Time = 24f;
        }
        SunAngleH1 = (_Time - 6.0f) * 15.0f;
        _Time += 0.002f * _TimeSetSpeed;
        if (_Time >= 0f && _Time < 12f)
        {
            _TimeMapping = (_Time - 6f) / 6f;
        }
        else
        {
            _TimeMapping = -(_Time - 18f) / 6f;
        }
    }
    void doRotation()
    {
        // RotationAxis = new Vector3(Mathf.Abs(RotationAxisBias) - 1, -RotationAxisBias, 0);
        // CenterPoint.transform.rotation *= Quaternion.AngleAxis(SunAngleH1 - SunAngleH2, RotationAxis);
        SunAngleH2 = SunAngleH1;
        
        CenterPoint.transform.Rotate(Vector3.right * _TimeSetSpeed /24f *360f * Time.deltaTime); //(0,24) ~ (0,360)
    }
    void doLightSwitch()
    {
        if (_Time >= 6.0f && _Time < 18.0f)
        {
            MoonLightComponent.intensity = 0;

            LightColor = Color.Lerp(SunRiseColor, DayColor, smoothstep(0.0f, 0.6f, (float)System.Math.Pow(Vector3.Dot(Vector3.Normalize(Sun.transform.forward), new Vector3(0.0f, -1.0f, 0.0f)), 1.5f)));
            LightColor.r = Mathf.Max(LightColor.r, 0);
            LightColor.g = Mathf.Max(LightColor.g, 0);
            LightColor.b = Mathf.Max(LightColor.b, 0);
            LightColorGrayScale = (LightColor.r + LightColor.g + LightColor.b) / 3f;
            SunLightComponent.color = Color.Lerp(new Color(LightColorGrayScale, LightColorGrayScale, LightColorGrayScale), LightColor, LightingColorSaturate);
            SunLightComponent.intensity = Mathf.Pow(LightingColorBrightness, 2f) * SunLightIntensity * Mathf.Pow(smoothstep(0.0f, 0.4f, System.Math.Max(Vector3.Dot(Vector3.Normalize(Sun.transform.forward), new Vector3(0.0f, -1.0f, 0.0f)), 0.0f)), 0.75f);
            AmbientColor = LightingColorBrightness * Color.Lerp(new Color(0.05f, 0.1f, 0.1f), new Color(0.25f, 0.3f, 0.3f), Mathf.Min(Mathf.Pow(_TimeMapping * 4f, 3f), 1f)) + new Color(Mathf.Pow(LightColor.b, 0.25f), Mathf.Pow(LightColor.g, 0.25f), Mathf.Pow(LightColor.r, 0.25f)) * SunLightComponent.intensity / 10 * AmbientIntensity;
            AmbientColor = Color.Lerp(new Color(AmbientColor.grayscale, AmbientColor.grayscale, AmbientColor.grayscale), AmbientColor, LightingColorSaturate);
            RenderSettings.ambientLight = AmbientColor;
            //SunLightComponent.shadowStrength = Mathf.Min(LightingColorBrightness * Mathf.Pow((float)System.Math.Pow(Vector3.Dot(Vector3.Normalize(Sun.transform.forward), new Vector3(0.0f, -1.0f, 0.0f)), 1.5f), 0.5f), 1) * 0.9f;
        }
        else
        {
            SunLightComponent.intensity = 0;

            LightColor = Color.Lerp(MoonRiseColor, NightColor, smoothstep(0.0f, 0.6f, System.Math.Max(Vector3.Dot(Vector3.Normalize(Moon.transform.forward), new Vector3(0.0f, -1.0f, 0.0f)), 0.0f)));
            LightColor.r = Mathf.Max(LightColor.r, 0);
            LightColor.g = Mathf.Max(LightColor.g, 0);
            LightColor.b = Mathf.Max(LightColor.b, 0);
            LightColorGrayScale = (LightColor.r + LightColor.g + LightColor.b) / 3f;
            MoonLightComponent.color = Color.Lerp(new Color(LightColorGrayScale, LightColorGrayScale, LightColorGrayScale), LightColor, LightingColorSaturate) * LightingColorBrightness;
            MoonLightComponent.intensity = Mathf.Pow(LightingColorBrightness, 0.5f) * MoonLightIntensity * smoothstep(0.0f, 0.3f, System.Math.Max(Vector3.Dot(Vector3.Normalize(Moon.transform.forward), new Vector3(0.0f, -1.0f, 0.0f)), 0.0f));
            AmbientColor = LightingColorBrightness * Color.Lerp(new Color(0.05f, 0.1f, 0.1f), new Color(0.1f, 0.15f, 0.15f), Mathf.Min(-Mathf.Pow(_TimeMapping * 4f, 3f), 1f)) + LightColor * MoonLightComponent.intensity / 4 * AmbientIntensity;
            AmbientColor = Color.Lerp(new Color(AmbientColor.grayscale, AmbientColor.grayscale, AmbientColor.grayscale), AmbientColor, LightingColorSaturate);
            RenderSettings.ambientLight = AmbientColor;
            //MoonLightComponent.shadowStrength = Mathf.Min(LightingColorBrightness * Mathf.Pow(System.Math.Max(Vector3.Dot(Vector3.Normalize(Moon.transform.forward), new Vector3(0.0f, -1.0f, 0.0f)), 0.0f), 0.75f), 1);
        }
    }
}