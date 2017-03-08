using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImransFirstSceneScript : MonoBehaviour {
    static private int m_callCount = 0; // how often has this class been called?
	// Use this for initialization
	void Start () {
    }

    // Update is called once per frame
    void Update () {
        var MySphere = GameObject.Find("Sphere");
        var MySphereTransform = MySphere.transform;

        var myGVRPointer = GameObject.Find("Main Camera").GetComponent<GvrPointerPhysicsRaycaster>();
        Debug.logger.Log(myGVRPointer);


    }

    // Update is called once per physics engine call, usually before update
    private void FixedUpdate()
    {
        var mySphere = GameObject.Find("Sphere");
        var theRigidBody = mySphere.GetComponent<Rigidbody>();

        // see if the controller is looking at us and has the button pushed.
        var contOrientation = GvrController.Orientation;
        
        //Debug.logger.Log(contOrientation);


        // update all the forces we want to...
        if (mySphere.transform.position.y < 0.51)
            theRigidBody.AddForce(0, 300, 0, ForceMode.Acceleration);
    }
}
