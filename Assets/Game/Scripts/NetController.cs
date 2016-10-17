using UnityEngine;
using System.Collections;

public class NetController : MonoBehaviour
{
    public GameObject serverCar;
    public bool needSyncPose = false;
    [HideInInspector]
    public ServerNetworkMgr netMgr;

    //以下的参数都在外部适配部件中进行赋值，这里只需要同步到网络上去就行
    //手柄参数（包括按钮）
    [HideInInspector]
    public float h = 0f;
    [HideInInspector]
    public float v = 0f;
    [HideInInspector]
    public bool fire1 = false;
    [HideInInspector]
    public bool fire2 = false;
    [HideInInspector]
    public bool fire3 = false;
    [HideInInspector]
    public bool fire4 = false;

    //方向盘参数
    [HideInInspector]
    public float steer = 0f;
    [HideInInspector]
    public float throttle = 0f;
    [HideInInspector]
    public float brake = 0f;
    [HideInInspector]
    public int gear = 0;//-1->R  0->N

    private NetPlayer[] allPlayers;//需要反复获取，反复使用


    void Start()
    {
        netMgr = gameObject.GetComponent<ServerNetworkMgr>();
    }

    /// <summary>
    /// 点击事件放在Update里
    /// </summary>
    void Update()
    {
        //foreach (NetPlayer p in allPlayers)
        //{
        //    p.sync_Fire1 = fire1;
        //}
    }

    /// <summary>
    /// 实时更新事件放在FixedUpdate里
    /// </summary>
    void FixedUpdate()
    {
        //只在这里反复获取
        allPlayers = GameObject.FindObjectsOfType<NetPlayer>();
        foreach (NetPlayer p in allPlayers)
        {
            if (needSyncPose)
            {
                //获取某个HostPlayer发过来的最新姿态
                if (p.carPose != Quaternion.identity)
                {
                    serverCar.transform.rotation = p.carPose;
                }
                if (p.carPosePos != Vector3.zero)
                {
                    serverCar.transform.position = p.carPosePos;
                }
            }
            else
            {
                if (p.carPose != Quaternion.identity)
                {
                    p.carPose = Quaternion.identity;
                }
                if (p.carPosePos != Vector3.zero)
                {
                    p.carPosePos = Vector3.zero;
                }
            }

            //服务器上的每个Player都更新
            p.sync_H = h;
            p.sync_V = v;
            //p.sync_Steer = steer;
            //p.sync_Throttle = throttle;
            //p.sync_Brake = brake;
        }
    }

    //获取Player中的任意一个，用于RPC调用
    public NetPlayer GetOneNetPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            return players[0].GetComponent<NetPlayer>();
        }
        return null;
    }
}
