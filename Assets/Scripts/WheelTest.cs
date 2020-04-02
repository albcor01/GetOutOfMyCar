using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTest : MonoBehaviour
{

    public enum TireType { Front, Back }

    public TireType tireType = TireType.Front;

    WheelCollider WheelCollider;
    public float torque = 200;
    public float MaxTurnAngle = 30;
    public GameObject wheel;


    // Start is called before the first frame update
    void Start()
    {
        WheelCollider = GetComponent<WheelCollider>();
        if (WheelCollider == null) throw new System.Exception("collider de la rueda no encontrado");
        if (wheel == null) throw new System.Exception("error al encontrar el gameobject de la rueda");
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        setWheelPosition();
    }

    void HandleInput()
    {
        if(Input.GetAxis("Forward") != 0)
        {
            Move(Input.GetAxis("Forward"));
        }

        if (Input.GetAxis("Turn") != 0)
        {
            Turn(Input.GetAxis("Turn"));
        }
    }

    void Move(float mov)
    {
        float thurstTorque = mov * torque;
        if (tireType == TireType.Back){        
            WheelCollider.motorTorque = thurstTorque;
        }   
    }
    void Turn(float turnValue)
    {
        if (tireType == TireType.Front)
        {
            float turn = turnValue * MaxTurnAngle;
            WheelCollider.steerAngle = turn;
        }
    }

    void setWheelPosition() {
        Quaternion quat;
        Vector3 position;
        WheelCollider.GetWorldPose(out position, out quat);
        wheel.transform.position = position;
        wheel.transform.rotation = quat;
    }
}
