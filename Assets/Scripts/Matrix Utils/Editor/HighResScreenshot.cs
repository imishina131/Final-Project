#if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;
using System.IO;
public class SceneViewScreenshot
{
    [MenuItem("Tools/Take Scene View Screenshot %#k")] // Ctrl+Shift+K 
    static void TakeScreenshot()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;

        if (sceneView == null)
        {
            Debug.LogError("No Scene View found.");
            return;
        }

        Camera cam = sceneView.camera;

        RenderTexture rt = new RenderTexture(
            (int)cam.pixelWidth,
            (int)cam.pixelHeight,
            24);

        cam.targetTexture = rt;

        Texture2D screenshot = new Texture2D(
            rt.width,
            rt.height,
            TextureFormat.RGB24,
            false);

        cam.Render();

        RenderTexture.active = rt;

        screenshot.ReadPixels(
            new Rect(0, 0, rt.width, rt.height),
            0,
            0);

        screenshot.Apply();

        cam.targetTexture = null;
        RenderTexture.active = null;

        string fileName = $"SceneScreenshot_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        string path = Path.Combine(Application.dataPath, fileName);

        File.WriteAllBytes(path, screenshot.EncodeToPNG());

        Object.DestroyImmediate(rt);
        Object.DestroyImmediate(screenshot);

        Debug.Log($"Screenshot saved to: {path}");

        AssetDatabase.Refresh();
    }
}
#endif