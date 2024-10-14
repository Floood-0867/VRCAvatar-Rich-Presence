using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Discord;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class AvatarRichPresence
{
    public static Discord.Discord discord { get; private set; }
    public static long lastTimestamp = 0;
    public static string ProjectName { get; private set; }
    public static string SceneName { get; private set; }
    public static bool ShowSceneName;
    public static bool ShowProjectName;


    static void Init()
    {
        if (isDiscordRunning())
        {
            if (discord != null) return;
            discord = new Discord.Discord(834113429333213217, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
            UnityEngine.Debug.Log("Discord Started!");

            lastTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            ProjectName = Application.productName;
            SceneName = SceneManager.GetActiveScene().name;

            if (discord != null) UpdateActivity();

            EditorSceneManager.sceneOpened += SceneOpenedCallback;
        }
    }
    static void OnApplicationQuit()
    {
        if (discord == null) return;
        discord.Dispose();
    }
    static AvatarRichPresence()
    {
        DelayStart();
        EditorApplication.update += Update;
    }

    public static async void DelayStart(int delay = 1000)
    {
        await Task.Delay(delay);
        Init();
    }

    static void Update()
    {
        if (discord != null)
            discord.RunCallbacks();
    }



    private static void UpdateActivity()
    {
        if (discord == null) return;      
        var activityManager = discord.GetActivityManager();

        var activity = new Discord.Activity
        {
            State = ProjectName,
            Details = SceneName,
            Timestamps =
                {
                    Start = lastTimestamp
                },
            Assets =
                {
                    LargeImage = "u_cube_1c_black",
                    LargeText = "Unity " + Application.unityVersion,                },
        };

        activityManager.UpdateActivity(activity, (res) =>
            {
                UnityEngine.Debug.Log(res);
                if (res != Discord.Result.Ok)
                {
                    UnityEngine.Debug.Log("Everything is fine!");
                }
            });
    }


    private static void SceneOpenedCallback(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
    {
        SceneName = EditorSceneManager.GetActiveScene().name;
        UpdateActivity();
    }



        private static bool isDiscordRunning()
    {
        Process[] processes = Process.GetProcessesByName("Discord");

        if (processes.Length == 0)
        {
            processes = Process.GetProcessesByName("DiscordPTB");

            if (processes.Length == 0)
            {
                processes = Process.GetProcessesByName("DiscordCanary");
            }
        }
        if (processes.Length != 0) { return true; } else { return false; }
    }
}