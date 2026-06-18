using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset preset;
    [SerializeField, Range(0, 96)] private float timeOfDay;
    [SerializeField] private GameObject lights;

    private void UpdateLighting(float timePercentage)
    {
        RenderSettings.ambientLight = preset.ambientColor.Evaluate(timePercentage);
        RenderSettings.fogColor = preset.fogColor.Evaluate(timePercentage);

        if(directionalLight != null)
        {
            directionalLight.color = preset.directionColor.Evaluate(timePercentage);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercentage * 360f) - 90f, 170f, 0));
        }
    }

    private void Update()
    {
        if(preset == null)
        {
            return;
        }

        if(Application.isPlaying)
        {
            timeOfDay += Time.deltaTime;
            timeOfDay %= 96;
            UpdateLighting(timeOfDay/96f);
        }
        else
        {
            UpdateLighting(timeOfDay / 96f);
        }

        if(timeOfDay <= 20 || timeOfDay >= 84)
        {
            lights.SetActive(true);
        }
        else
        {
            lights.SetActive(false);
        }
    }
    private void OnValidate()
    {
        if(directionalLight != null)
        {
            return;
        }

        if(RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if(light.type == LightType.Directional)
                {
                    directionalLight = light;
                    return;
                }
            }
        }
    }
}
