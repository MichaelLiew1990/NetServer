using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetPlayer : NetworkBehaviour
{
    //[SyncVar]使用注意，它们都必须在内部修改，在外部修改必须循环调用所有NetPlayer然后将其修改，不然会出现调用了的已经修改并且同步到网络，但是没有调用的不会同步到网络。
    //[ClientRpc]标识的函数服务器和客户端上必须都有
    //[Command]标识的函数服务器和客户端上不必都有

    //================客户端用===============服务器更新客户端读取
    //手柄参数（包括按钮）
    [SyncVar]
    public float sync_H;
    [SyncVar]
    public float sync_V;
    [SyncVar]
    public bool sync_Fire1;
    [SyncVar]
    public bool sync_Fire2;
    [SyncVar]
    public bool sync_Fire3;
    [SyncVar]
    public bool sync_Fire4;
    //方向盘参数
    [SyncVar]
    public float sync_Steer;
    [SyncVar]
    public float sync_Throttle;
    [SyncVar]
    public float sync_Brake;
    [SyncVar]
    public int sync_Gear;//-1->R  0->N

    //================服务器用================客户端更新服务端读取
    public Quaternion carPose = Quaternion.identity;
    public Vector3 carPosePos = Vector3.zero;

    public enum TVCommand//TV端用
    {
        StartGame,
        StopGame,
        ResetGame,
        BroadcastYes,
        BroadcastNo
    }

    public enum ContentType//TV端用
    {
        None,
        Movie,
        GameWithPad,
        GameWithSteer,
        Picture
    }

    public enum PlatformCommand//手机平台用
    {
        ContentStarted,
        ContentStoped
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    [Command]
    public void CmdTVServerExec(TVCommand cmd, ContentType type, string arg)
    {
        print("cmd=" + cmd + "type=" + type + "arg" + arg);
        NetController ctrl = FindObjectOfType<NetController>();
        if (ctrl != null)
        {
            if (cmd == TVCommand.StartGame)
            {
                ctrl.GameStart(type, arg);
            }
            else if (cmd == TVCommand.StopGame)
            {
                ctrl.GameStop(type, arg);
            }
            else if (cmd == TVCommand.ResetGame)
            {
                ctrl.GameReset(type, arg);
            }
            else if (cmd == TVCommand.BroadcastYes)
            {
                ctrl.ToggleBroadcast(true);
            }
            else if (cmd == TVCommand.BroadcastNo)
            {
                ctrl.ToggleBroadcast(false);
            }
        }
    }

    [Command]
    public void CmdPlatformServerExec(PlatformCommand cmd, string arg)
    {
        NetController ctrl = FindObjectOfType<NetController>();
        if (cmd == PlatformCommand.ContentStarted)
        {
            RpcGameAlreadyStart();
            if (ctrl) ctrl.GameAlreadyStarted();
        }
        else if (cmd == PlatformCommand.ContentStoped)
        {
            RpcGameAlreadyStop();
            if (ctrl) ctrl.GameAlreadyStoped();
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

    [ClientRpc]
    void RpcGameAlreadyStart()
    {
        //TV端实现
    }

    [ClientRpc]
    void RpcGameAlreadyStop()
    {
        //TV端实现
    }

    [ClientRpc]
    public void RpcStartGame(string sceneName)
    {
        //客户端实现
    }

    [ClientRpc]
    public void RpcStopGame()
    {
        //客户端实现
    }

    [ClientRpc]
    public void RpcUpdateHostIP(string hostIP)
    {
        //客户端实现
    }
}
