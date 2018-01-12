using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HardnessProperty : MonoBehaviour
{

    // Use this for initialization

    // Must be attached to the root game object as a component for it to work
    // ColliderlessImpact will only try to find this component at root

    [Tooltip("Assign a zero or negative value will set the hardness to infinity, thus making the landscape unable to destroy")]
    [Range(0.0f, 1000.0f)]
    public float Hardness = 1.0f;

    [Tooltip("If hardness is disabled, default hardness 1.0f will be used and uses default explosion stamp")]
    public bool UseHardness = true;



    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
