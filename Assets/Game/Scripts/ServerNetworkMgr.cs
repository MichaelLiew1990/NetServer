using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ServerNetworkMgr : NetworkManager
{
    public Button stopBroadCast;
    public Toggle toggleBroadCast;

    // 在服务器上当有客户端断开时调用
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnServerDisconnect");

        if (conn.playerControllers.Count > 1) Debug.LogError("conn.playerControllers.Count > 1");
        if (conn.playerControllers.Count > 0 && !conn.playerControllers[0].unetView.isLocalPlayer)
        {
            RemovePlayer(conn.connectionId);
        }

        base.OnServerDisconnect(conn);
    }

    // 当生成对象到客户端时调用  
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("OnServerAddPlayer:" + conn.connectionId);
        base.OnServerAddPlayer(conn, playerControllerId);

        if (conn.playerControllers.Count > 1) Debug.LogError("conn.playerControllers.Count > 1");
        if (conn.playerControllers.Count > 0 && !conn.playerControllers[0].unetView.isLocalPlayer)
        {
            AddPlayer(conn.connectionId, conn.address.Substring(conn.address.Length - 3));
        }
    }

    // 当服务器发生错误时调用  
    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("OnServerError");
        base.OnServerError(conn, errorCode);
    }



    // 网络稳定性处理==============================
    void Start()
    {
        StartCoroutine(CoAutoReConnectNetwork());
        players = new Dictionary<int, string>();

        toggleBroadCast.onValueChanged.AddListener(ToggleBroadCast);
    }

    IEnumerator CoAutoReConnectNetwork()
    {
        //第一次启动延缓检查时间
        yield return new WaitForSeconds(5f);

        bool isConnected = false;
        while (true)
        {
            Debug.Log("Check!");
            yield return new WaitForSeconds(1f);
            if (!isNetworkActive)
            {
                isConnected = false;
                Debug.Log("Restart!");
                StopServer();
                yield return new WaitForSeconds(1f);
                StartServer();
                ServerGameMgr.Instance.netState = NetState.Connecting;
                yield return new WaitForSeconds(5f);
            }
            else//连接成功 或 正在尝试连接
            {
                if (!isConnected)
                {
                    isConnected = IsClientConnected();
                    if (isConnected) ServerGameMgr.Instance.netState = NetState.Connected;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!isNetworkActive)
        {
            ServerGameMgr.Instance.netState = NetState.Failed;
            ClearPlayer();
        }
    }

    void ToggleBroadCast(bool b)
    {
        ServerBroadCast s = GetComponent<ServerBroadCast>();
        if (b)
        {
            if (s == null)
            {
                gameObject.AddComponent<ServerBroadCast>();
            }
        }
        else
        {
            if (s != null)
            {
                s.DestroyBroadCast();
            }
        }
    }

    //========================================
    //========================================
    //服务器端使用============================
    public Text text;

    Dictionary<int, string> players;//IP字符后三位
    void ClearPlayer()
    {
        if (players.Count <= 0) return;
        players.Clear();
        UpdatePlayers();
    }

    void AddPlayer(int playerID, string playerAddressNum)
    {
        players.Add(playerID, playerAddressNum);
        UpdatePlayers();
    }

    void RemovePlayer(int playerID)
    {
        players.Remove(playerID);
        UpdatePlayers();
    }

    void UpdatePlayers()
    {
        text.text = "No Player";
        if (players.Count <= 0) return;
        int n = 0;
        foreach (int key in players.Keys)
        {
            if (n == 0)
            {
                text.text = "Player: " + players[key];
            }
            else
            {
                text.text += "-" + players[key];
            }
            n++;
        }
        Invoke("InvokeUpdateHostIP", 1f);
    }

    void InvokeUpdateHostIP()
    {
        NetController net = GameObject.FindObjectOfType<NetController>();
        if (net != null && net.GetOneNetPlayer() != null)
        {
            foreach (int key in players.Keys)
            {
                net.GetOneNetPlayer().RpcUpdateHostIP(players[key]);
                break;
            }
        }
    }
}
