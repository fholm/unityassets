using System;
using System.Linq;
using UnityEngine;

namespace VoiceChat
{
    public class VoiceChatRecorder : MonoBehaviour
    {
        #region Instance
        static VoiceChatRecorder instance;

        public static VoiceChatRecorder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(VoiceChatRecorder)) as VoiceChatRecorder;
                }

                return instance;
            }
        }

        public AudioClip audioClip { get { return clip; } }

        #endregion

        public event Action StartedRecording;
            
        [SerializeField]
        KeyCode toggleToTalkKey = KeyCode.O;

        [SerializeField]
        KeyCode pushToTalkKey = KeyCode.P;

        [SerializeField]
        bool autoDetectSpeaking = false;

        [SerializeField]
        int autoDetectIndex = 4;

        [SerializeField]
        float forceTransmitTime = 2f;

        ulong packetId;
        int previousPosition = 0;
        int sampleIndex = 0;
        string device = null;
        AudioClip clip = null;
        bool transmitToggled = false;
        bool recording = false;
        float forceTransmit = 0f;
        int recordFrequency = 0;
        int recordSampleSize = 0;
        int targetFrequency = 0;
        int targetSampleSize = 0;
        float[] fftBuffer = null;
        float[] sampleBuffer = null;
        VoiceChatCircularBuffer<float[]> previousSampleBuffer = new VoiceChatCircularBuffer<float[]>(5);

        public KeyCode PushToTalkKey
        {
            get { return pushToTalkKey; }
            set { pushToTalkKey = value; }
        }

        public KeyCode ToggleToTalkKey
        {
            get { return toggleToTalkKey; }
            set { toggleToTalkKey = value; }
        }

        public bool AutoDetectSpeech
        {
            get { return autoDetectSpeaking; }
            set { autoDetectSpeaking = value; }
        }

        public int NetworkId
        {
            get;
            set;
        }

        public string Device
        {
            get { return device; }
            set
            {
                if (value != null && !Microphone.devices.Contains(value))
                {
                    Debug.LogError(value + " is not a valid microphone device");
                    return;
                }

                device = value;
            }
        }

        public bool HasDefaultDevice
        {
            get { return device == null; }
        }

        public bool HasSpecificDevice
        {
            get { return device != null; }
        }

        public bool IsTransmitting
        {
            get { return transmitToggled || forceTransmit > 0 || Input.GetKey(pushToTalkKey); }
        }

        public bool IsRecording
        {
            get { return recording; }
        }

        public string[] AvailableDevices
        {
            get { return Microphone.devices; }
        }

        public event System.Action<VoiceChatPacket> NewSample;

        void Start()
        {
            if (instance != null && instance != this)
            {
                MonoBehaviour.Destroy(this);
                Debug.LogError("Only one instance of VoiceChatRecorder can exist");
                return;
            }

            Application.RequestUserAuthorization(UserAuthorization.Microphone);
            instance = this;
        }

        void OnEnable()
        {
            if (instance != null && instance != this)
            {
                MonoBehaviour.Destroy(this);
                Debug.LogError("Only one instance of VoiceChatRecorder can exist");
                return;
            }

            Application.RequestUserAuthorization(UserAuthorization.Microphone);
            instance = this;
        }

        void OnDisable()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        void Update()
        {
            if (!recording)
            {
                return;
            }

            forceTransmit -= Time.deltaTime;

            if (Input.GetKeyUp(toggleToTalkKey))
            {
                transmitToggled = !transmitToggled;
            }

            bool transmit = transmitToggled || Input.GetKey(pushToTalkKey);
            int currentPosition = Microphone.GetPosition(Device);

            // This means we wrapped around
            if (currentPosition < previousPosition)
            {
                while (sampleIndex < recordFrequency)
                {
                    ReadSample(transmit);
                }

                sampleIndex = 0;
            }

            // Read non-wrapped samples
            previousPosition = currentPosition;

            while (sampleIndex + recordSampleSize <= currentPosition)
            {
                ReadSample(transmit);
            }
        }

        void Resample(float[] src, float[] dst)
        {
            if (src.Length == dst.Length)
            {
                Array.Copy(src, 0, dst, 0, src.Length);
            }
            else
            {
                //TODO: Low-pass filter 
                float rec = 1.0f / (float)dst.Length;

                for (int i = 0; i < dst.Length; ++i)
                {
                    float interp = rec * (float)i * (float)src.Length;
                    dst[i] = src[(int)interp];
                }
            }
        }

        void ReadSample(bool transmit)
        {
            // Extract data
            clip.GetData(sampleBuffer, sampleIndex);

            // Grab a new sample buffer
            float[] targetSampleBuffer = VoiceChatFloatPool.Instance.Get();

            // Resample our real sample into the buffer
            Resample(sampleBuffer, targetSampleBuffer);

            // Forward index
            sampleIndex += recordSampleSize;

            // Highest auto-detected frequency
            float freq = float.MinValue;
            int index = -1;

            // Auto detect speech, but no need to do if we're pushing a key to transmit
            if (autoDetectSpeaking && !transmit)
            {
                // Clear FFT buffer
                for (int i = 0; i < fftBuffer.Length; ++i)
                {
                    fftBuffer[i] = 0;
                }

                // Copy to FFT buffer
                Array.Copy(targetSampleBuffer, 0, fftBuffer, 0, targetSampleBuffer.Length);

                // Apply FFT
                Exocortex.DSP.Fourier.FFT(fftBuffer, fftBuffer.Length / 2, Exocortex.DSP.FourierDirection.Forward);

                // Get highest frequency
                for (int i = 0; i < fftBuffer.Length; ++i)
                {
                    if (fftBuffer[i] > freq)
                    {
                        freq = fftBuffer[i];
                        index = i;
                    }
                }
            }

            // If we have an event, and 
            if (NewSample != null && (transmit || forceTransmit > 0 || index >= autoDetectIndex))
            {
                // If we auto-detected a voice, force recording for a while
                if (index >= autoDetectIndex)
                {
                    if (forceTransmit <= 0)
                    {
                        while (previousSampleBuffer.Count > 0)
                        {
                            TransmitBuffer(previousSampleBuffer.Remove());
                        }
                    }

                    forceTransmit = forceTransmitTime;
                }

                TransmitBuffer(targetSampleBuffer);
            }
            else
            {
                if (previousSampleBuffer.Count == previousSampleBuffer.Capacity)
                {
                    VoiceChatFloatPool.Instance.Return(previousSampleBuffer.Remove());
                }

                previousSampleBuffer.Add(targetSampleBuffer);
            }

        }

        void TransmitBuffer(float[] buffer)
        {
            // Compress into packet
            VoiceChatPacket packet = VoiceChatUtils.Compress(buffer);

            // Set networkid of packet
            packet.NetworkId = NetworkId;
            packet.PacketId = ++packetId;
            // Raise event
            NewSample(packet);
        }

        public bool StartRecording()
        {
            if (NetworkId == 0 && !VoiceChatSettings.Instance.LocalDebug)
            {
                Debug.LogError("NetworkId is not set");
                return false;
            }

            if (recording)
            {
                Debug.LogError("Already recording");
                return false;
            }

            targetFrequency = VoiceChatSettings.Instance.Frequency;
            targetSampleSize = VoiceChatSettings.Instance.SampleSize;

            int minFreq;
            int maxFreq;
            Microphone.GetDeviceCaps(Device, out minFreq, out maxFreq);

            recordFrequency = minFreq == 0 && maxFreq == 0 ? 44100 : maxFreq;
            recordSampleSize = recordFrequency / (targetFrequency / targetSampleSize);

            clip = Microphone.Start(Device, true, 1, recordFrequency);
            sampleBuffer = new float[recordSampleSize];
            fftBuffer = new float[VoiceChatUtils.ClosestPowerOfTwo(targetSampleSize)];
            recording = true;

            if (StartedRecording != null)
            {
                StartedRecording();
            }

            return recording;
        }

        public void StopRecording()
        {
            clip = null;
            recording = false;
        }

        public void ToggleTransmit()
        {
            transmitToggled = !transmitToggled;
        }

        public void StartTransmit()
        {
            transmitToggled = true;
        }
        public void StopTransmit()
        {
            transmitToggled = false;
        }
    }
}