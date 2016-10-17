using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum NetState
{
    None,
    Connecting,
    Connected,
    Failed
}

public class ServerGameMgr : MonoBehaviour
{
    [HideInInspector]
    public NetState netState = NetState.None;

    [HideInInspector]
    public string netErrorInfo = "";

    private static ServerGameMgr inst = null;
    public static ServerGameMgr Instance
    {
        get
        {
            if (GameObject.Find("GameManager"))
            {
                inst = GameObject.Find("GameManager").GetComponent<ServerGameMgr>();
            }
            else
            {
                GameObject o = new GameObject();
                o.name = "GameManager";
                inst = o.AddComponent<ServerGameMgr>();
            }
            return inst;
        }
    }

    void OnDestroy()
    {
        NetworkManager.singleton.StopServer();
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()//这个函数理论上进进入一次，多次进入那就错了
    {
        netState = NetState.Connecting;
        StartNetworking();
    }

    void StartNetworking()
    {
        //启动RPC服务
        NetworkManager.singleton.maxConnections = 100;
        NetworkManager.singleton.networkAddress = "192.168.15.245";
        NetworkManager.singleton.StartServer();

        //启动Command服务
        ChairCommand.Instance.ip = "192.168.15.245";
        ChairCommand.Instance.StartSyncCarState();
    }
}
