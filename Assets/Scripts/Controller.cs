using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Destructible2D
{
    public class Controller : MonoBehaviour
    {
        enum Parts
        {
            Body,
            FWheel,
            BWheel,
            Barrel,
            ReferenceTrans,
            Weapon
        };
        private GameObject FWheel;
        private GameObject BWheel;
        private GameObject locBarrel;



        private JointMotor2D locMoter;

        private HingeJoint2D Barrel_joint;
        private float Barrel_tolerance = 3;
        public float torqueMax = 1000f;
        public float speed = 700;

        public float barrel_speed = 35;
        Transform a;

        [Tooltip("The prefab that gets thrown")]
        public GameObject ProjectilePrefab;

        // Use this for initialization
        void Start()
        {
            FWheel = this.gameObject.transform.GetChild((int)Parts.FWheel).gameObject;
            BWheel = this.gameObject.transform.GetChild((int)Parts.BWheel).gameObject;
            locBarrel = this.gameObject.transform.GetChild((int)Parts.Barrel).gameObject;



        }

        // Update is called once per frame
        void Update()
        {
            Controller__UpdateMovement();
            Controller__UpdateBarrel();
   



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

        private void Controller__UpdateBarrel()
        {
            Vector2 BarrelPos, MousePos, mouseVec2;
            Ray ray;
            float barrel_angle;
            float Mouse_angle;
            JointMotor2D jointMotor = new JointMotor2D { motorSpeed = barrel_speed, maxMotorTorque = 10000 };

            BarrelPos = locBarrel.transform.GetChild(0).position;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            MousePos = (Vector2)ray.origin;
            // get mousVec
            mouseVec2 = MousePos - BarrelPos;
            // get Joint angle
            Barrel_joint = locBarrel.GetComponent<HingeJoint2D>();
            //Get Mouse angle
            barrel_angle = Barrel_joint.jointAngle + 90;
            Mouse_angle = Vector2.SignedAngle(mouseVec2, new Vector2(0f, 1f));

            //Debug.Log("mouse vec" + Mouse_angle + "angle" + barrel_angle, gameObject);


            if (Mouse_angle - barrel_angle + Barrel_tolerance < 0)
            {// -speed
                jointMotor.motorSpeed = -barrel_speed;
                Barrel_joint.motor = jointMotor;
            }
            else if (Mouse_angle - barrel_angle - Barrel_tolerance > 0)
            {// +speed
                jointMotor.motorSpeed = barrel_speed;
                Barrel_joint.motor = jointMotor;
            }
            else
            {
                jointMotor.motorSpeed = 0;
                Barrel_joint.motor = jointMotor;
            }

            if (Input.GetMouseButtonDown(0))
            {
                var projectile = Instantiate(ProjectilePrefab);
                var startPos = BarrelPos;
                float angle = -Barrel_joint.jointAngle ;
                var angleV2 = new Vector2(Mathf.Cos(angle/180 * Mathf.PI), Mathf.Sin(angle / 180 * Mathf.PI));
                var rigidbody2d = projectile.GetComponent<Rigidbody2D>();
                Debug.Log(" vec" + angleV2 + "angle" + angle, gameObject);
                if (rigidbody2d != null)
                {
                    rigidbody2d.velocity = angleV2 *10;
                }
                projectile.transform.position = startPos + angleV2 *2;
            }
        }





    }

}