using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 油门是3rd axis， 刹车时4th axis
/// </summary>
public class SteerWheelUI : MonoBehaviour
{
    public NetController net;
    public Slider sliderHorizontal;
    public Slider sliderThrottle;
    public Slider sliderBrake;
    public Text textGear;

    
    void Update()
    {
        float steer = Input.GetAxis("Steer");
        float throttle = Input.GetAxis("Throttle");
        float brake = Input.GetAxis("Brake");

        sliderHorizontal.value = steer;
        sliderThrottle.value = throttle;
        sliderBrake.value = brake;

        net.steer = steer;
        net.throttle = (throttle + 1) / 2f;
        net.brake = (brake + 1) / 2f;

        textGear.text = "N";
        net.gear = 0;
        if (Input.GetKey(KeyCode.JoystickButton12))
        {
            textGear.text = "1";
            net.gear = 1;
        }
        if (Input.GetKey(KeyCode.JoystickButton13))
        {
            textGear.text = "2";
            net.gear = 2;
        }
        if (Input.GetKey(KeyCode.JoystickButton14))
        {
            textGear.text = "3";
            net.gear = 3;
        }
        if (Input.GetKey(KeyCode.JoystickButton15))
        {
            textGear.text = "4";
            net.gear = 4;
        }
        if (Input.GetKey(KeyCode.JoystickButton16))
        {
            textGear.text = "5";
            net.gear = 5;
        }
        if (Input.GetKey(KeyCode.JoystickButton17))
        {
            textGear.text = "6";
            net.gear = 6;
        }
        if (Input.GetKey(KeyCode.JoystickButton18))
        {
            textGear.text = "R";
            net.gear = -1;
        }
    }
}
