using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VedioPlayUI : MonoBehaviour
{
    public Button btnPlay;
    public Button btnStop;
    public Button btnReset;//重置座椅归零
    public NetController net;
    public Dropdown dropPlayList;
    public ChairPose car;

    private bool isStarted = false;

    void Start()
    {
        btnPlay.onClick.AddListener(ButtonPlay);
        btnStop.onClick.AddListener(ButtonStop);
        btnReset.onClick.AddListener(ButtonReset);
        btnPlay.gameObject.SetActive(true);
        btnStop.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.A))
        {
            ButtonPlay();
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.B))
        {
            ButtonReset();
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton2) || Input.GetKeyDown(KeyCode.X))
        {
            ButtonStop();
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetKeyDown(KeyCode.F1))
        {
            dropPlayList.value = 0;
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetKeyDown(KeyCode.F2))
        {
            dropPlayList.value = 1;
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton6) || Input.GetKeyDown(KeyCode.F3))
        {
            dropPlayList.value = 2;
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.F4))
        {
            net.netMgr.toggleBroadCast.isOn = true;
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton8) || Input.GetKeyDown(KeyCode.F5))
        {
            net.netMgr.toggleBroadCast.isOn = false;
        }
    }

    public void MovieEnd()
    {
        btnPlay.gameObject.SetActive(true);
        btnStop.gameObject.SetActive(false);
        isStarted = false;
        net.needSyncPose = false;
    }

    void ButtonPlay()
    {
        if (net.GetOneNetPlayer() != null && !isStarted)
        {
            string movieName = dropPlayList.options[dropPlayList.value].text;
            net.GetOneNetPlayer().RpcStartGame(movieName);
            Invoke("InvokePlayMovie", 3f);

            btnPlay.gameObject.SetActive(false);
            btnStop.gameObject.SetActive(false);
            isStarted = true;
            net.needSyncPose = true;
        }
    }

    void ButtonReset()
    {
        car.GetComponent<ChairPose>().isReseting = true;
    }

    //     IEnumerator ResetRotation()
    //     {
    //         float t = 0f;
    //         float o = car.offsetE + car.gameObject.transform.rotation.y;
    //         while (true)
    //         {
    //             car.gameObject.transform.rotation = Quaternion.Slerp(
    //     car.gameObject.transform.rotation, Quaternion.identity, t);
    //             yield return new WaitForSeconds(0.02f);
    //             t += 0.01f;
    //             Debug.Log("=====" + t + "====" + car.gameObject.transform.rotation);
    //             if (car.gameObject.transform.rotation.x == 0 && car.gameObject.transform.rotation.y == 0 &&
    //                 car.gameObject.transform.rotation.z == 0)
    //             {
    //                 break;
    //             }
    //         }
    //         car.gameObject.transform.rotation = Quaternion.identity;
    //         car.lastRotY = 0f;
    //         car.offsetE = 0f;
    //         yield return null;
    //     }

    void ButtonStop()
    {
        if (net.GetOneNetPlayer() != null && isStarted)
        {
            net.GetOneNetPlayer().RpcStopGame();
        }

        btnPlay.gameObject.SetActive(true);
        btnStop.gameObject.SetActive(false);
        isStarted = false;
        net.needSyncPose = false;
    }

    void InvokePlayMovie()
    {
        string movieName = dropPlayList.options[dropPlayList.value].text;
        net.GetOneNetPlayer().RpcPlayMovie(movieName);
        btnStop.gameObject.SetActive(true);
        btnPlay.gameObject.SetActive(false);
    }
}
