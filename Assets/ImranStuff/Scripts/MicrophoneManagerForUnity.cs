using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Assets.ImranStuff.Scripts
{
    public class MicrophoneManagerForUnity
    {
        static public bool IsInited = false;
        static private string m_RecordingDevice = "";

        public MicrophoneManagerForUnity()
        {
            // check initialization and initialize if needed.
            if (!IsInited)
            {
                // let's figure out how to record sound in Unity.
                foreach (string micDevice in Microphone.devices)
                {
                    m_RecordingDevice = micDevice;
                }
            }
            IsInited = true;
        }

        public void Pulse(float deltaTime)
        {
            AudioClip sample = null;
            int recordingPosition = 0;
            if (Microphone.IsRecording(m_RecordingDevice))
            {
                recordingPosition = Microphone.GetPosition(m_RecordingDevice);
                Debug.logger.Log(recordingPosition + " cat " + deltaTime);
            }
            else
            {
                sample = Microphone.Start(m_RecordingDevice, false, 1, 16000);
                Debug.logger.Log(sample.length + " dog ");
            }
        }
    }
}
