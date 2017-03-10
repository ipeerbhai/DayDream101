using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImransFirstSceneScript : MonoBehaviour {
    static private int m_callCount = 0; // how often has this class been called?
	// Use this for initialization
	void Start () {
    }

    // Update is called once per frame
    void Update ()
    {
        // find the bouncing sphere from inside this central game object.
        var MySphere = GameObject.Find("Sphere"); // Can skip getting if this script component is attached to the GameObject that will be the target.
        var MySphereTransform = MySphere.transform;

        // find the controller and get its position.
        var controllerPointer = GameObject.Find("GvrControllerPointer");
        var controllerTransform = controllerPointer.transform;

        // use the controller orientation quaternion and get a forward pointing vector from it, then raycast.
        Vector3 fwd = GvrController.Orientation * Vector3.forward;
        RaycastHit pointingAtWhat;
        if (Physics.Raycast(controllerTransform.position, fwd, out pointingAtWhat) )
        {
            var theTextGameObject = GameObject.Find("txtMainData");
            UnityEngine.UI.Text theTextComponent = theTextGameObject.GetComponent<UnityEngine.UI.Text>();
            theTextComponent.text = "hit " + pointingAtWhat.collider.name;
        }
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
