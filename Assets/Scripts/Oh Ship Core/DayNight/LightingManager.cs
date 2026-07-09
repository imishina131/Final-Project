using UnityEngine;
using UnityEngine.VFX;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset preset;
    [SerializeField, Range(0, 96)] private float timeOfDay;
    [SerializeField] private GameObject[] lights;
    [SerializeField] private GameObject[] fireflies;
    [SerializeField] private GameObject[] windowLights;
    private int lightCount;
    private bool lightSetUp = false;
    private bool finishedLightUp = true;
    private bool startedFireflies = false;

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

    private void Start()
    {
        timeOfDay = 18;
        lights = GameObject.FindGameObjectsWithTag("Light");
        fireflies = GameObject.FindGameObjectsWithTag("Firefly");
        windowLights = GameObject.FindGameObjectsWithTag("WindowLight");
    }

    private void Update()
    {

        if(!lightSetUp)
        {
            lightCount = lights.Length;
            lightSetUp = true;
        }       

        if (preset == null)
        {
            return;
        }

        if(Application.isPlaying)
        {
            timeOfDay += Time.deltaTime;
            timeOfDay %= 96;
            UpdateLighting(timeOfDay/144f);
        }
        else
        {
            UpdateLighting(timeOfDay / 144f);
        }

        if(timeOfDay <= 30 || timeOfDay >= 126)
        {
            if(!startedFireflies)
            {

                foreach (var firefly in fireflies)
                {
                    VisualEffect fireflyVFX = firefly.GetComponent<VisualEffect>();
                    fireflyVFX.Play();
                }

                startedFireflies = true;

            }
            if (lightCount > 0 && finishedLightUp)
            {
                finishedLightUp = false;
                Invoke("LightUp", Random.Range(0.1f, 0.6f));
            }
            LightUpBuildings();

        }
        else
        {
            foreach(var light in lights) 
            {
                light.SetActive(false);
                lightSetUp = false;
            }

            if (startedFireflies)
            {
                foreach (var firefly in fireflies)
                {
                    VisualEffect fireflyVFX = firefly.GetComponent<VisualEffect>();
                    fireflyVFX.Stop();
                }

                startedFireflies = false;
            }

            TurnOffBuildings();
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

    private void LightUp()
    {
        GameObject chosenLight = lights[Random.Range(0, lights.Length)];

        if(chosenLight.activeInHierarchy)
        {
            finishedLightUp = true;
            return;
        }
        else
        {
            chosenLight.SetActive(true);
            finishedLightUp = true;
            lightCount--;
        }
    }

    private void LightUpBuildings()
    {
        foreach(GameObject windowLight in windowLights)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetFloat("_Emissive_Intensity", 1.3f);
            Renderer renderer = windowLight.GetComponent<Renderer>();
            renderer.SetPropertyBlock(propertyBlock);
        }
    }

    private void TurnOffBuildings()
    {
        foreach (GameObject windowLight in windowLights)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetFloat("_Emissive_Intensity", 0f);
            Renderer renderer = windowLight.GetComponent<Renderer>();
            renderer.SetPropertyBlock(propertyBlock);
        }
    }
}
