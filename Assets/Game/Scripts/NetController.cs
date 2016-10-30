using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NetController : MonoBehaviour
{
    public Button btnStart;
    public Button btnStop;
    public Button btnReset;//重置座椅归零
    public Dropdown dropType;
    public GameObject serverCar;
    public bool needSyncPose = false;
    public NetPlayer.ContentType contentType;
    private string contentName;

    [HideInInspector]
    public ServerNetworkMgr netMgr;

    //以下的参数都在外部适配部件中进行赋值，这里只需要同步到网络上去就行
    //手柄参数（包括按钮）
    public float h = 0f;
    public float v = 0f;
    public bool fire1 = false;
    public bool fire2 = false;
    public bool fire3 = false;
    public bool fire4 = false;

    //方向盘参数
    public float steer = 0f;
    public float throttle = 0f;
    public float brake = 0f;
    public int gear = 0;//-1->R  0->N

    [HideInInspector]
    public VedioPlayUI uiMovie;
    [HideInInspector]
    public GamePadUI uiGamePad;
    [HideInInspector]
    public SteerWheelUI uiSteerWheel;

    private NetPlayer[] allPlayers;//需要反复获取，反复使用
    private bool isStarted = false;


    /// <summary>
    /// 点击事件放在Update里
    /// </summary>
    void Update()
    {
        //更新外设事件数据
        if (contentType == NetPlayer.ContentType.GameWithPad)
        {
            foreach (NetPlayer p in allPlayers)
            {
                p.sync_Fire1 = fire1;
                p.sync_Fire2 = fire2;
                p.sync_Fire3 = fire3;
                p.sync_Fire4 = fire4;
            }
        }
        else if (contentType == NetPlayer.ContentType.GameWithSteer)
        {
            foreach (NetPlayer p in allPlayers)
            {
                p.sync_Gear = gear;
            }
        }


        if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.A))
        {
            ButtonStart();
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.B))
        {
            ButtonReset();
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton2) || Input.GetKeyDown(KeyCode.X))
        {
            ButtonStop();
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.F4))
        {
            ToggleBroadcast(true);
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton8) || Input.GetKeyDown(KeyCode.F5))
        {
            ToggleBroadcast(false);
        }
    }

    /// <summary>
    /// 实时更新事件放在FixedUpdate里
    /// </summary>
    void FixedUpdate()
    {
        allPlayers = FindObjectsOfType<NetPlayer>();//只在这里反复获取

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

            //更新外设实时数据，服务器上的每个Player都更新
            if (contentType == NetPlayer.ContentType.GameWithPad)
            {
                p.sync_H = h;
                p.sync_V = v;
            }
            else if (contentType == NetPlayer.ContentType.GameWithSteer)
            {
                p.sync_Steer = steer;
                p.sync_Throttle = throttle;
                p.sync_Brake = brake;
            }
        }
    }

    void Start()
    {
        netMgr = gameObject.GetComponent<ServerNetworkMgr>();

        uiMovie = FindObjectOfType<VedioPlayUI>();
        uiGamePad = FindObjectOfType<GamePadUI>();
        uiSteerWheel = FindObjectOfType<SteerWheelUI>();

        btnStart.onClick.AddListener(ButtonStart);
        btnStop.onClick.AddListener(ButtonStop);
        btnReset.onClick.AddListener(ButtonReset);
        btnStart.gameObject.SetActive(true);
        btnStop.gameObject.SetActive(false);
        dropType.onValueChanged.AddListener(DropContentTypeChanged);

        uiMovie.gameObject.SetActive(false);
        uiGamePad.gameObject.SetActive(false);
        uiSteerWheel.gameObject.SetActive(false);
    }

    public void GameStart(NetPlayer.ContentType type, string arg)
    {
        contentName = arg;
        ButtonStart();
        UpdateContentType(type);
    }

    void DropContentTypeChanged(int v)
    {
        if (v == 0)
        {
            UpdateContentType(NetPlayer.ContentType.Movie, false);
        }
        else if (v == 1)
        {
            UpdateContentType(NetPlayer.ContentType.GameWithPad, false);
        }
        else if (v == 2)
        {
            UpdateContentType(NetPlayer.ContentType.GameWithSteer, false);
        }
    }

    void UpdateContentType(NetPlayer.ContentType type, bool changeDrop = true)
    {
        contentType = type;
        if (type == NetPlayer.ContentType.Movie)
        {
            uiMovie.gameObject.SetActive(true);
            uiGamePad.gameObject.SetActive(false);
            uiSteerWheel.gameObject.SetActive(false);
            if (changeDrop) dropType.value = 0;
        }
        else if (type == NetPlayer.ContentType.GameWithPad)
        {
            uiMovie.gameObject.SetActive(false);
            uiGamePad.gameObject.SetActive(true);
            uiSteerWheel.gameObject.SetActive(false);
            if (changeDrop) dropType.value = 1;
        }
        else if (type == NetPlayer.ContentType.GameWithSteer)
        {
            uiMovie.gameObject.SetActive(false);
            uiGamePad.gameObject.SetActive(false);
            uiSteerWheel.gameObject.SetActive(true);
            if (changeDrop) dropType.value = 2;
        }
    }

    public void GameStop(NetPlayer.ContentType type, string arg)
    {
        ButtonStop();
    }

    public void GameReset(NetPlayer.ContentType type, string arg)
    {
        ButtonReset();
    }

    void ButtonStart()
    {
        if (GetOneNetPlayer() != null && !isStarted)
        {
            string movieName = contentName == "" ? uiMovie.dropPlayList.options[uiMovie.dropPlayList.value].text : contentName;
            GetOneNetPlayer().RpcStartGame(movieName);
            btnStart.gameObject.SetActive(false);
            btnStop.gameObject.SetActive(false);
            isStarted = true;
            needSyncPose = true;
        }
    }

    void ButtonReset()
    {
        serverCar.GetComponent<ChairPose>().isReseting = true;
    }

    void ButtonStop()
    {
        if (GetOneNetPlayer() != null && isStarted)
        {
            GetOneNetPlayer().RpcStopGame();
        }

        btnStart.gameObject.SetActive(true);
        btnStop.gameObject.SetActive(false);
        isStarted = false;
        needSyncPose = false;
    }

    public void GameAlreadyStoped()
    {
        btnStart.gameObject.SetActive(true);
        btnStop.gameObject.SetActive(false);
        isStarted = false;
        needSyncPose = false;
    }

    public void GameAlreadyStarted()
    {
        btnStop.gameObject.SetActive(true);
        btnStart.gameObject.SetActive(false);
    }

    public void ToggleBroadcast(bool isOn)
    {
        netMgr.toggleBroadCast.isOn = isOn;
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
