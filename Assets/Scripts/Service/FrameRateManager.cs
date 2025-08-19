using System.Collections;
using System.Text;
using UnityEditor;
// using UnityEditor.Build;
using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
    [Header("Frame Settings")]
    // int MaxRate = 9999;
    float currentFrameTime;
    private float fps;

    public float targetFrameRate = 60.0f;
    public TMPro.TextMeshProUGUI FPSCounterText;
    private StringBuilder fpsStringBuilder = new StringBuilder(10);


    // public float timer, refresh, avgFramerate;

    void Awake()
    {
        // QualitySettings.vSyncCount = 0;
        // Application.targetFrameRate = 60;

        // StartCoroutine("WaitForNextFrame");
    }

    // [MenuItem("Build/Android Frame Pacing Example")]
    // public static void FramePacing()
    // {
    //     PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
    //     PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

    //     PlayerSettings.Android.optimizedFramePacing = true;

    //     BuildPlayerOptions options = new BuildPlayerOptions();
    //     options.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
    //     options.locationPathName = "Builds/AndroidBuild.apk";
    //     options.target = BuildTarget.Android;
    //     options.targetGroup = BuildTargetGroup.Android;

    //     BuildPipeline.BuildPlayer(options);
    // }
    void Start()
    {
        // FramePacing();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        // Screen.sleepTimeout = SleepTimeout.NeverSleep;
        // QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1, true);
        if(Application.isEditor) 
        {
            FPSCounterText.gameObject.SetActive(true);
            InvokeRepeating("GetFps", 0.5f, 0.5f);
        }
    }

    // Update is called once per frame
    void GetFps()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        fpsStringBuilder.Clear().Append("FPS: ").Append(fps);
        FPSCounterText.text = "FPS: " + fps.ToString();
    }

    IEnumerator WaitForNextFrame()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            currentFrameTime += 1.0f / targetFrameRate;
            var t = Time.realtimeSinceStartup;
            var sleepTime = currentFrameTime - t - 0.01f;
            if (sleepTime > 0)
            // Thread.Sleep((int)(sleepTime * 1000));
            yield return new WaitForSeconds(sleepTime);
            while (t < currentFrameTime) t = Time.realtimeSinceStartup;
        }
    }
}
