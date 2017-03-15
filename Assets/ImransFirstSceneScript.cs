using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.ImranStuff.Scripts;

public class ImransFirstSceneScript : MonoBehaviour
{
    private int m_RequestedRecordingSeconds = 20;
    static private string m_RecordingDevice = "";
    private AudioClip m_AudioSample = null;

    private float m_forwardSpeed = 3.0f;
    private bool m_IsRunningTask;
    static private MicrophoneManagerForUnity m_MicManager = null;

    // Use this for initialization
    void Start()
    {
        foreach (string micDevice in Microphone.devices)
        {
            m_RecordingDevice = micDevice;
        }

        if (m_MicManager == null)
            m_MicManager = new MicrophoneManagerForUnity();
        m_IsRunningTask = false;
    }

    // Update is called once per frame
    void Update()
    {
        //float deadZone = 0.15f; // used for the Gvr input touchpad.
        if ((GvrController.AppButtonDown) || (Input.GetKeyDown(KeyCode.Space)))
        {
            m_MicManager.Clear();
            m_AudioSample = Microphone.Start(m_RecordingDevice, false, m_RequestedRecordingSeconds, 16000);
            m_MicManager.ClipStart();
        }
        if ((GvrController.AppButton) || (Input.GetKey(KeyCode.Space)))
        {
            if (!Microphone.IsRecording(m_RecordingDevice))
            {
                m_MicManager.ConsolidateClips(m_AudioSample, m_AudioSample.samples);
                m_AudioSample = Microphone.Start(m_RecordingDevice, false, m_RequestedRecordingSeconds, 16000);
                m_MicManager.ClipStart();
            }

        }
        if ((GvrController.AppButtonUp) || (Input.GetKeyUp(KeyCode.Space)))
        {
            int recordingPosition = Microphone.GetPosition(m_RecordingDevice); // have to know this before we end, or the API will zero out.
            Microphone.End(m_RecordingDevice);
            m_MicManager.ConsolidateClips(m_AudioSample, recordingPosition);
            m_MicManager.Start();
            m_IsRunningTask = true; // job management on a heavyweight thread.
        }

        if (m_IsRunningTask)
        {
            if (m_MicManager.Update())
            {
                // we have information back from the MicManager.
                Debug.logger.Log("All done!");
                m_IsRunningTask = false;
            }
        }

        // put some debug output into the world.
        var theTextGameObject = GameObject.Find("txtMainData");
        UnityEngine.UI.Text theTextComponent = theTextGameObject.GetComponent<UnityEngine.UI.Text>();
        var player = GameObject.Find("Player");
        var camera = GameObject.Find("Main Camera");

        /*  Should probaby remove this and use the main camera.orientation?
        // let's rotate the camera.
        if (GvrController.IsTouching)
        {
            if (GvrController.TouchPos.x < .5 - deadZone)
            {
                // Should be rotating left
                player.transform.Rotate(0, -1 * Time.deltaTime * m_rotationSpeed, 0);
            }
            else if (GvrController.TouchPos.x > .5 + deadZone)
            {
                //Should be rotating right
                player.transform.Rotate(0, 1 * Time.deltaTime * m_rotationSpeed, 0);
            }
        }
        */

        // let's move the camera when the user pushes the controller "click" button.
        if (GvrController.ClickButton)
        {
            // let's translate the camera forward
            player.transform.position += (camera.transform.rotation * Vector3.forward * Time.deltaTime) * m_forwardSpeed; // using deltatime means I'm moving at 1 meter/s

            // and keep the world canvas in scene.
            var mainTextCanvas = GameObject.Find("mainTextCanvas");
            var controllerPointer = GameObject.Find("GvrControllerPointer");

            mainTextCanvas.transform.position = controllerPointer.transform.position;
            mainTextCanvas.transform.position += (GvrController.Orientation * Vector3.forward) * 3;
            mainTextCanvas.transform.rotation = GvrController.Orientation;
        }
    }


    // FixedUpdate is called once per physics engine call, usually before update
    private void FixedUpdate()
    {
        // find the bouncing sphere from inside this central game object.
        var mySphere = GameObject.Find("Sphere"); // Can skip getting if this script component is attached to the GameObject that will be the target.
        var sphereRigidBody = mySphere.GetComponent<Rigidbody>();
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
            if (GvrController.AppButton)
            {
                Vector3 forceToAdd = GvrController.Orientation * Vector3.forward * 100;
                sphereRigidBody.AddForceAtPosition(forceToAdd, pointingAtWhat.point);
            }
        }

        // update all the forces we want to...
        if (mySphere.transform.position.y < 0.51)
            sphereRigidBody.AddForce(0, 300, 0, ForceMode.Acceleration);
    }
}
