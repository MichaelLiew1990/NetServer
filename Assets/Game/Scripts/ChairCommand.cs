using UnityEngine;
using System.Collections;
using System.Net;     
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;

[SerializeField]
public enum ChairType
{
    SixDof,
    SixDofWithRotate
}

public class ChairCommand : MonoBehaviour
{
    private static ChairCommand instance = null;
    public static ChairCommand Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject o = new GameObject();
                o.name = "ChairCommand";
                instance = o.AddComponent<ChairCommand>();
            }
            return instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// UDP端口号  专门针对座椅的
    /// </summary>
    public int port = 11000;
    /// <summary>
    /// 服务端IP
    /// </summary>
    public string ip = "";
    /// <summary>
    /// 平台旋转放大比例
    /// </summary>
    public float rotationScale = 1;
    /// <summary>
    /// 刹车灯是否亮
    /// </summary>
    public bool stop = false;
    /// <summary>
    /// 平台旋转数据
    /// </summary>
    public Vector3 rotate = Vector3.zero;
    /// <summary>
    /// 平台移动数据（增量，为加速度）
    /// </summary>
    public Vector3 move = Vector3.zero;
    /// <summary>
    /// 当前平台移动数据
    /// </summary>
    public Vector3 currentMove = Vector3.zero;
    public float maxDrag = 0.11f;
    public bool isSweeping = false;//是否扫腿
    public bool isWinding = false;//是否吹气
    public ChairType chairType = ChairType.SixDof;
    /// <summary>
    /// UDP Client
    /// </summary>
    UdpClient client;
    IPEndPoint groupEP;
    bool run = true;

    public Slider[] SliderList;

    /// <summary>
    /// 加载配置文件，启动数据发送线程，仅服务端程序同步状态数据
    /// </summary>
    public void StartSyncCarState()
    {
        StartCoroutine(LoadIpConfig(() =>
        {
            client = new UdpClient();
            groupEP = new IPEndPoint(IPAddress.Parse(ip), port);
            Thread t = new Thread(() =>
            {
                while (run)
                {
                    //创建位移和旋转数据
                    byte[] bytes = CreateSendData();
                    //创建刹车灯是否亮数据
                    //byte[] bytesStop = CreateStopData();
                    //发送位移和旋转数据
                    client.Send(bytes, bytes.Length, groupEP);
                    Thread.Sleep(10);
                    //发送刹车灯数据
                    //client.Send(bytesStop, bytesStop.Length, groupEP);
                    //Thread.Sleep(10);
                }
            });
            t.Start();
        }));
    }


    /// <summary>
    /// 加载配置文件
    /// </summary>
    /// <param name="onFinished">加载完成后回调</param>
    /// <returns></returns>
    IEnumerator LoadIpConfig(System.Action onFinished)
    {
        //WWW www = new WWW("file://" + Application.dataPath + "/IPConfig.txt");
        yield return null;
        rotationScale = 1f;
        maxDrag = 0.11f;
        //if (onFinished != null)
        {
            onFinished();
        }
    }

    /// <summary>
    /// 创建刹车灯是否亮数据
    /// </summary>
    /// <returns></returns>
    byte[] CreateStopData()
    {
        string temp = stop ? "stop" : "run";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(temp);
        return bytes;
    }

    /// <summary>
    /// 创建位移和旋转数据
    /// </summary>
    /// <returns></returns>
    byte[] CreateSendData()
    {
        rotate *= rotationScale;
        //平台以固定速度向原点移动
//         currentMove.x = Mathf.Lerp(currentMove.x, 0, 0.01f);
//         currentMove.y = Mathf.Lerp(currentMove.y, 0, 0.01f);
//         currentMove.z = Mathf.Lerp(currentMove.z, 0, 0.01f);
        //限制平台位移在-1024到1024间
        currentMove.x = Mathf.Clamp(currentMove.x, -1023, 1023);
        currentMove.y = Mathf.Clamp(currentMove.y, -1023, 1023);
        currentMove.z = Mathf.Clamp(currentMove.z, -1023, 1023);
        //平台以固定速度回正
//         rotate.x = Mathf.Lerp(rotate.x, 0, 0.01f);
//         rotate.y = Mathf.Lerp(rotate.y, 0, 0.01f);
        //限制平台旋转在-1024到1024间
        rotate.x = Mathf.Clamp(rotate.x, -1023, 1023);
        rotate.y = Mathf.Clamp(rotate.y, -1023, 1023);
        string data = "";
        data += "x";
        data += FormatData((int)currentMove.x);
        data += "y";
        data += FormatData((int)currentMove.y);
        data += "z";
        data += FormatData((int)currentMove.z);
        data += "u";
        data += FormatData((int)rotate.x);
        data += "v";
        data += FormatData((int)rotate.y);
        data += "w";
        data += FormatData(0);
        if (chairType == ChairType.SixDofWithRotate)
        {
            data += "c";
            int c = (int)((rotate.z / 360f) * 36000f);
            data += FormatData(c, 7);
            data += "t";
            data += "0000000000" + (isWinding ? "1" : "0") + (isSweeping ? "1" : "0");
        }

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
        return bytes;
    }

    /// <summary>
    /// 格式化数据
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    string FormatData(int d, int n = 4)
    {
        string ds = "";
        if (d < 0)
        {
            ds += "-";
        }
        else
        {
            ds += "0";
        }
        d = Mathf.Abs(d);

        if (n == 4) ds += string.Format("{0:D4}", d);
        else if (n == 7) ds += string.Format("{0:D7}", d);
        else Debug.LogError("n need 4 or 7");

        return ds;
    }

    void OnDestroy()
    {
        run = false;
        if (client != null)
        {
            client.Close();
        }
    }

}

