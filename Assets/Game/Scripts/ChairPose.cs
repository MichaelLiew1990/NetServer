using UnityEngine;
using System.Collections;

public class ChairPose : MonoBehaviour
{
    public AnimationCurve fixCurve;
    public float lastRotY = 0f;//上一帧Y向的转动，用来计算是否满360而跳值（回位归零时需要归零）
    public float offsetE = 0f;//360度跳到了1度时加360，1度跳到了360时减360（回位归零时需要归零）
    public ChairType chairType;

    void Start()
    {
        ChairCommand.Instance.chairType = chairType;
    }

    void FixedUpdate()
    {
        if (CheckReset()) return;

        float RX = 0.0f, RY = 0.0f, RZ = 0.0f;
        float MX = 0.0f, MY = 0.0f, MZ = 0.0f;

        MX = this.gameObject.transform.position.x;
        MY = this.gameObject.transform.position.y;
        MZ = this.gameObject.transform.position.z;

        RX = this.gameObject.transform.rotation.eulerAngles.x;
        RY = this.gameObject.transform.rotation.eulerAngles.y;//底座旋转值
        RZ = this.gameObject.transform.rotation.eulerAngles.z;
        if (lastRotY - RY > 200f) offsetE += 360f;
        if (lastRotY - RY < -200f) offsetE -= 360f;

        lastRotY = RY;

        RY += offsetE;
        FixAngleTo90(ref RX);
        FixAngleTo90(ref RZ);

        RX = fixCurve.Evaluate(RX);
        RZ = fixCurve.Evaluate(RZ);

        Vector3 rot = ChairCommand.Instance.rotate;
        Vector3 mov = ChairCommand.Instance.currentMove;

        ChairCommand.Instance.rotate = new Vector3(-RX / 90f * 1023f, -RZ / 90f * 1023f, -RY);

        MY -= 2f;//从2分开，大于部分为正值，小于部分为负值
        ChairCommand.Instance.currentMove = new Vector3(0f, 0f, MY / 2f * 1023f);

        //         ChairCommand.Instance.rotate = new Vector3(
        //             Mathf.Lerp(rot.x, -RX / 90f * 1023f, 0.1f),
        //             Mathf.Lerp(rot.y, -RZ / 90f * 1023f, 0.1f),
        //             Mathf.Lerp(rot.z, -RY, 0.1f));

        //         MY -= 2f;//从2分开，大于部分为正值，小于部分为负值
        //         ChairCommand.Instance.currentMove = new Vector3(0f, 0f,
        //             Mathf.Lerp(mov.z, MY / 2f * 1023f, 0.1f));
    }


    public bool isReseting = false;//是否正在复位
    public float resetSpeed = 20f;//复位所需时间
    private float startTime = 0f;
    private float journeyLength = 0f;
    bool CheckReset()
    {
        if (isReseting)
        {
            if (startTime == 0 && journeyLength == 0)
            {
                startTime = Time.time;
                journeyLength = Vector3.Magnitude(ChairCommand.Instance.rotate);
            }

            float distCovered = (Time.time - startTime) * resetSpeed;
            float fracJourney = distCovered / journeyLength;
            ChairCommand.Instance.rotate = Vector3.Lerp(ChairCommand.Instance.rotate, Vector3.zero, fracJourney);

            if (Vector3.Distance(ChairCommand.Instance.rotate, Vector3.zero) < 0.001f)
            {
                startTime = 0f;
                journeyLength = 0f;
                isReseting = false;
                lastRotY = 0f;
                offsetE = 0f;
                ChairCommand.Instance.rotate = Vector3.zero;
                this.gameObject.transform.rotation = Quaternion.identity;
                //TODO:position需要复位
            }
        }
        return isReseting;
    }

    void FixAngleTo90(ref float angle)
    {
        while (Mathf.Abs(angle) > 90)
        {
            if (angle > 0)
            {
                angle -= 180f;
            }
            else
            {
                angle += 180f;
            }
        }
    }
}
