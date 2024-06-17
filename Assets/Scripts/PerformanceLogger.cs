using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PerformanceLogger : MonoBehaviour
{

    private static PerformanceLogger instance;

    public static bool gamePaused;

    private bool lastPaused;
    private string filePath;
    private ulong frameCount;
    private string sceneName;

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        gamePaused = true;
        lastPaused = true;
        filePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "performance.log");
        Debug.Log(filePath);
        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        frameCount = 0;
        sceneName = arg0.name;
    }

    void Start()
    {
        WriteFileHeader();
    }

    void WriteFileHeader()
    {
        string os = SystemInfo.operatingSystem;
        string cpu = SystemInfo.processorType;
        string gpu = SystemInfo.graphicsDeviceName;
        int ram = SystemInfo.systemMemorySize;

        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine("Operating System: " + os);
            writer.WriteLine("CPU: " + cpu);
            writer.WriteLine("GPU: " + gpu);
            writer.WriteLine("Total System RAM (MB): " + ram);
            writer.WriteLine("Registered frame number,FPS,FrameTime (ms)");
        }
    }

    void WriteFrameData(float fps, float frameTime)
    {
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            string data = string.Format("{0},{1},{2},{3}",
                frameCount,
                sceneName,
                fps.ToString("F2"),
                frameTime.ToString("F2"));
            writer.WriteLine(data);
        }
    }

    void Update()
    {
        if (gamePaused)
        {
            return;
        }

        float fps = 1.0f / Time.unscaledDeltaTime;
        float frameTime = Time.unscaledDeltaTime * 1000;


        WriteFrameData(fps, frameTime);

        if (lastPaused != gamePaused)
        {
            Debug.Log("perfomance logger paused " + gamePaused);
        }

        lastPaused = gamePaused;
        frameCount++;
    }
}
