using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Assets.ImranStuff.Scripts
{
    public class MicrophoneManagerForUnity : ThreadedJob
    {
        static public bool IsInited = false;
        static private string m_RecordingDevice = "";
        private AudioClip m_AudioSample = null;
        private int m_RequestedRecordingSeconds = 1;
        private MemoryStream m_clipMemory = null;

        //--------------------------------------------------------------------------------------------------------------------------------
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

        //--------------------------------------------------------------------------------------------------------------------------------
        protected override void ThreadFunction()
        {
            while (m_clipMemory == null)
            {
                System.Threading.Thread.Sleep(100);
            }
            MemoryStream cloudFormattedStream = createWaveFileStream(m_clipMemory);
            AITransactionHandler AITransact = new AITransactionHandler();
            AITransact.SendDataToCloud(cloudFormattedStream);
        }

        //--------------------------------------------------------------------------------------------------------------------------------
        public void Pulse(float deltaTime)
        {
            int recordingPosition = 0;
            if (Microphone.IsRecording(m_RecordingDevice))
            {
                recordingPosition = Microphone.GetPosition(m_RecordingDevice);
            }
            else
            {
                m_AudioSample = Microphone.Start(m_RecordingDevice, false, m_RequestedRecordingSeconds, 16000);
            }

            if (m_AudioSample.length == m_RequestedRecordingSeconds)
                ConsolidateClips();
        }

        //--------------------------------------------------------------------------------------------------------------------------------
        private void ConsolidateClips()
        {
            if (m_clipMemory == null)
                m_clipMemory = new MemoryStream();
            float[] data = new float[m_AudioSample.samples];
            m_AudioSample.GetData(data, 0);
            for (int i = 0; i < data.Length; i++)
            {
                Int16 thisSampleAsPCM = (Int16)(data[i] * Int16.MaxValue);
                byte[] sampleBytes = BitConverter.GetBytes(thisSampleAsPCM);
                m_clipMemory.Write(sampleBytes, 0, sampleBytes.Length);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------
        public void WriteWavHeader(System.IO.MemoryStream stream, bool isFloatingPoint, ushort channelCount, ushort bitDepth, int sampleRate, int totalSampleCount)
        {
            stream.Position = 0;

            // RIFF header.
            // Chunk ID.
            stream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);

            // Chunk size.
            stream.Write(BitConverter.GetBytes(((bitDepth / 8) * totalSampleCount) + 36), 0, 4);

            // Format.
            stream.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);



            // Sub-chunk 1.
            // Sub-chunk 1 ID.
            stream.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);

            // Sub-chunk 1 size.
            stream.Write(BitConverter.GetBytes(16), 0, 4);

            // Audio format (floating point (3) or PCM (1)). Any other format indicates compression.
            stream.Write(BitConverter.GetBytes((ushort)(isFloatingPoint ? 3 : 1)), 0, 2);

            // Channels.
            stream.Write(BitConverter.GetBytes(channelCount), 0, 2);

            // Sample rate.
            stream.Write(BitConverter.GetBytes(sampleRate), 0, 4);

            // Bytes rate.
            stream.Write(BitConverter.GetBytes(sampleRate * channelCount * (bitDepth / 8)), 0, 4);

            // Block align.
            stream.Write(BitConverter.GetBytes((ushort)channelCount * (bitDepth / 8)), 0, 2);

            // Bits per sample.
            stream.Write(BitConverter.GetBytes(bitDepth), 0, 2);

            // Sub-chunk 2.
            // Sub-chunk 2 ID.
            stream.Write(Encoding.ASCII.GetBytes("data"), 0, 4);

            // Sub-chunk 2 size.
            stream.Write(BitConverter.GetBytes((bitDepth / 8) * totalSampleCount), 0, 4);
        }

        //--------------------------------------------------------------------------------------------------------------------------------
        // Creates a file-headered wave memory stream out of stream
        public MemoryStream createWaveFileStream(System.IO.MemoryStream stream, int samplingRate = 16000)
        {
            MemoryStream outputStream = new MemoryStream();
            WriteWavHeader(outputStream, false, 1, 16, samplingRate, (int)stream.Length / 2);
            outputStream.Write(stream.ToArray(), 0, (int)stream.Length);
            outputStream.Flush();
            outputStream.Position = 0;
            return (outputStream);
        }

    }
}
