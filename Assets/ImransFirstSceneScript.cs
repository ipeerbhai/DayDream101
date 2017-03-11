using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImransFirstSceneScript : MonoBehaviour {
    private const float m_forwardSpeed = 3.0f;
	// Use this for initialization
	void Start () {
    }

    // Update is called once per frame
    void Update ()
    {
        // let's move the camera when the user pushes the controller "click" button.
        if (GvrController.ClickButton)
        {
            var camera = GameObject.Find("Main Camera");
            camera.transform.position += (Vector3.forward * Time.deltaTime) * m_forwardSpeed; // using deltatime means I'm moving at 1 meter/s
        }
    }

    // Update is called once per physics engine call, usually before update
    private void FixedUpdate()
    {
        // find the bouncing sphere from inside this central game object.
        var mySphere = GameObject.Find("Sphere"); // Can skip getting if this script component is attached to the GameObject that will be the target.
        var MySphereTransform = mySphere.transform;

        // find the controller and get its position.
        var controllerPointer = GameObject.Find("GvrControllerPointer");
        var controllerTransform = controllerPointer.transform;

        // use the controller orientation quaternion and get a forward pointing vector from it, then raycast.
        Vector3 fwd = GvrController.Orientation * Vector3.forward;
        RaycastHit pointingAtWhat;
        if (Physics.Raycast(controllerTransform.position, fwd, out pointingAtWhat))
        {
            var theTextGameObject = GameObject.Find("txtMainData");
            UnityEngine.UI.Text theTextComponent = theTextGameObject.GetComponent<UnityEngine.UI.Text>();
            theTextComponent.text = "hit " + pointingAtWhat.collider.name;
        }

        // update all the forces we want to...
        var theRigidBody = mySphere.GetComponent<Rigidbody>();
        if (mySphere.transform.position.y < 0.51)
            theRigidBody.AddForce(0, 300, 0, ForceMode.Acceleration);
    }
}
