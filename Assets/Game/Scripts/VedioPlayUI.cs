using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VedioPlayUI : MonoBehaviour
{
    public NetController net;
    public Dropdown dropPlayList;

    void Start()
    {

    }

    void Update()
    {
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
    }
}
