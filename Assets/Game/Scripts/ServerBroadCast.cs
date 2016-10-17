using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.Events;

public class ServerBroadCast : MonoBehaviour
{
    public int port = 6677;//用于局域网搜索的特定端口

    [HideInInspector]
    public UnityEvent onSucceed;
    [HideInInspector]
    public UnityEvent onFailed;

    [HideInInspector]
    public bool isServer = true;

    private bool isRunning = false;
    private string specialText = "$$Strom-Mojing-DCC$$";

    private Thread serverThread = null;
    private UdpClient UdpSend = null;

    private int isSucceed = 0;//>0 true
    private int isFailed = 0;//>0 true

    //停止广播服务器IP并且销毁自身
    public void DestroyBroadCast()
    {
        StopBroadCast();
        Destroy(this, 2f);
    }

    void StartBroadCast()//服务器一直发消息
    {
        if (serverThread != null && serverThread.IsAlive) return;

        serverThread = new Thread(() =>
        {
            UdpSend = new UdpClient();

            while (isRunning)
            {
                Thread.Sleep(2000);
                byte[] buf = Encoding.Unicode.GetBytes(specialText);
                int ret = UdpSend.Send(buf, buf.Length, new IPEndPoint(IPAddress.Broadcast, port));
                if (isSucceed == 0)
                {
                    if (ret > 0)
                    {
                        isSucceed = 1;
                    }
                    else
                    {
                        isFailed = 1;
                    }
                }
            }

            UdpSend.Close();
        });

        serverThread.IsBackground = true;
        InitBroadCast();
        serverThread.Start();
    }


    void Awake()
    {
        onSucceed = new UnityEvent();
        onFailed = new UnityEvent();
    }

    void Update()
    {
        if (isSucceed==1)
        {
            onSucceed.Invoke();
            isSucceed = 2;
        }
        if (isFailed==1)
        {
            onFailed.Invoke();
            isFailed = 2;
        }
    }

    void Start()
    {
        StartBroadCast();
    }

    void OnDestroy()
    {
        //一定要调用呀
        StopBroadCast();
    }

    void InitBroadCast()
    {
        isRunning = true;
        if (UdpSend != null) UdpSend.Close();
    }

    void StopBroadCast()
    {
        isRunning = false;
        if (UdpSend != null) UdpSend.Close();
        if (serverThread != null && serverThread.IsAlive) serverThread.Abort();
    }
}
