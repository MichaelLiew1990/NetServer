using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetPlayer : NetworkBehaviour
{
    //[SyncVar]使用注意，它们都必须在内部修改，在外部修改必须循环调用所有NetPlayer然后将其修改，不然会出现调用了的已经修改并且同步到网络，但是没有调用的不会同步到网络。

    [SyncVar]
    //[HideInInspector]
    public float sync_H;//服务器更新客户端读取
    [SyncVar]
    //[HideInInspector]
    public float sync_V;//服务器更新客户端读取

    //[HideInInspector]
    public Quaternion carPose = Quaternion.identity;//客户端更新服务端读取
    //[HideInInspector]
    public Vector3 carPosePos = Vector3.zero;//客户端更新服务端读取

    public enum NetCommand
    {
        StartGame,
        StopGame,
        ResetGame,
        BroadcastYes,
        BroadcastNo
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    [Command]
    public void CmdServerExec(NetCommand cmd)
    {
        VedioPlayUI vUI = GameObject.FindObjectOfType<VedioPlayUI>();
        if (vUI != null)
        {
            if (cmd == NetCommand.StartGame)
            {
                vUI.ButtonPlay();
            }
            else if (cmd == NetCommand.StopGame)
            {
                vUI.ButtonStop();
            }
            else if (cmd == NetCommand.ResetGame)
            {
                vUI.ButtonReset();
            }
            else if (cmd == NetCommand.BroadcastYes)
            {
                vUI.ToggleBroadcast(true);
            }
            else if (cmd == NetCommand.BroadcastNo)
            {
                vUI.ToggleBroadcast(false);
            }
        }
    }

    [Command]
    public void CmdUpdateCarPose(Quaternion rotate, Vector3 pos, string clientIP)
    {
        carPose = rotate;
        carPosePos = pos;
        if (clientIP == "0.0")
        {
            Debug.LogError("Host IP is 0.0!");
        }
    }

    [Command]
    public void CmdMovieEnd()
    {
        VedioPlayUI vUI = GameObject.FindObjectOfType<VedioPlayUI>();
        if (vUI != null)
        {
            vUI.MovieEnd();
        }
    }

    [ClientRpc]
    public void RpcStartGame(string sceneName)
    {
        //服务器不需要实现
    }

    [ClientRpc]
    public void RpcStopGame()
    {
        //服务器不需要实现
    }

    [ClientRpc]
    public void RpcPlayMovie(string mp4Name)
    {
        //服务器不需要实现
    }

    [ClientRpc]
    public void RpcUpdateHostIP(string hostIP)
    {
        //服务器不需要实现
    }
}
