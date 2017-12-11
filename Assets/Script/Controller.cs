using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    enum Parts{
        Body,
        FWheel,
        BWheel
    };
    GameObject FWheel;
    GameObject BWheel;
    
    JointMotor2D  locMoter;
    public float torqueMax = 100f;
    public float speed = 40;

    

	// Use this for initialization
	void Start () {
        FWheel = this.gameObject.transform.GetChild((int)Parts.FWheel).gameObject;
        BWheel = this.gameObject.transform.GetChild((int)Parts.BWheel).gameObject;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Controller__UpdateMovement();

    }

    private void Controller__UpdateMovement()
    {
        float horiTor = (Input.GetAxis("Horizontal")) * torqueMax;
        if (horiTor > 0)
        {
            locMoter.motorSpeed = speed;
            locMoter.maxMotorTorque = horiTor;
        }
        else if (horiTor < 0)
        {
            locMoter.motorSpeed = -speed;
            locMoter.maxMotorTorque = -horiTor;
        }
        else
        {
            locMoter.motorSpeed = 0;
            locMoter.maxMotorTorque = 5;
        }
        FWheel.GetComponent<WheelJoint2D>().motor = locMoter;
        BWheel.GetComponent<WheelJoint2D>().motor = locMoter;
    }

    private void COntroller__UpdateBarrel()
    {

    }

 
}
