using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;


//现在是服务端与客户端公用，以后可以考虑分开
public class GamePadUI : MonoBehaviour
{
    public NetController net;
    public RawImage imgJoystickCenter;
    public RawImage imgA;
    public RawImage imgB;
    public RawImage imgX;
    public RawImage imgY;

    void Update()
    {
        ResetAllImage();
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        net.h = h;
        net.v = v;

        imgJoystickCenter.rectTransform.localPosition = new Vector3(h * 80, v * 80, 0);

        if (Input.GetKey(KeyCode.Joystick1Button0))
        {
            SetButtonDownColor(imgA);
            net.fire1 = true;
        }
        else
        {
            net.fire1 = false;
        }

        if (Input.GetKey(KeyCode.Joystick1Button1))
        {
            SetButtonDownColor(imgB);
            net.fire2 = true;
        }
        else
        {
            net.fire2 = false;
        }

        if (Input.GetKey(KeyCode.Joystick1Button2))
        {
            SetButtonDownColor(imgX);
            net.fire3 = true;
        }
        else
        {
            net.fire3 = false;
        }

        if (Input.GetKey(KeyCode.Joystick1Button3))
        {
            SetButtonDownColor(imgY);
            net.fire4 = true;
        }
        else
        {
            net.fire4 = false;
        }
    }

    void SetButtonDownColor(RawImage img)
    {
        img.color = Color.red;
    }

    void ResetAllImage()
    {
        imgJoystickCenter.rectTransform.localPosition = new Vector3(0, 0, 0);
        imgA.color = Color.white;
        imgB.color = Color.white;
        imgX.color = Color.white;
        imgY.color = Color.white;
    }
}
