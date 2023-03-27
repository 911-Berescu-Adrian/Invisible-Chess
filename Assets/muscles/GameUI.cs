using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum CameraAngle
{
    menu=0,
    team_white=1,
    team_black=2
}

public class GameUI : MonoBehaviour
{
    public static GameUI instance { set; get; }

    public server server;
    public client client;

    public Action<bool> SetLocalGame;

    [SerializeField] private Animator menuAnimator;
    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] private GameObject[] cameraAngles;


    public void ChangeCamera(CameraAngle index)
    {
        for (int i = 0; i < cameraAngles.Length; i++)
            cameraAngles[i].SetActive(false);
        cameraAngles[(int)index].SetActive(true);
    }
    private void Awake()
    {
        instance = this;
        RegisterEvents();
    }
    private void RegisterEvents()
    {
        NetUtility.C_START_GAME += OnStartGameClient;
    }

    private void OnStartGameClient(NetMessage obj)
    {
        menuAnimator.SetTrigger("InGameMenu");
    }

    private void UnregisterEvents()
    {
        NetUtility.C_START_GAME -= OnStartGameClient;
    }


    public void LocalGameButton()
    {
        menuAnimator.SetTrigger("InGameMenu");
        SetLocalGame?.Invoke(true);
        server.Init(8007);
        client.Init("127.0.0.1", 8007);
    }

    public void OnlineButton()
    {
        menuAnimator.SetTrigger("OnlineMenu");
    }

    public void HostButton()
    {
        server.Init(8007);
        SetLocalGame?.Invoke(false);
        client.Init("127.0.0.1", 8007);
        menuAnimator.SetTrigger("HostMenu");
    }

    public void ConnectButton()
    {
        SetLocalGame?.Invoke(false);
        client.Init(addressInput.text, 8007);
    }

    public void OnlineBackButton()
    {
        menuAnimator.SetTrigger("StartMenu");
    }

    public void HostBackButton()
    {
        server.Shutdown();
        client.Shutdown();
        menuAnimator.SetTrigger("OnlineMenu");
    }
}
