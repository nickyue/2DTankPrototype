using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBlowable : MonoBehaviour
{
    public GameObject wind;
    private Wind wind2access;
    private float windForceX2Set;
    private float windForceY2Set;
    public Rigidbody2D rb;
    // Use this for initialization
    void Start()
    {
        wind = GameObject.Find("Wind");


        wind2access = wind.GetComponent<Wind>();

        windForceX2Set = wind2access.windForceX;
        windForceY2Set = wind2access.windForceY;
    }

    // Update is called once per frame
    void Update()
    {

        
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(windForceX2Set, windForceY2Set), ForceMode2D.Force);


    }
}
