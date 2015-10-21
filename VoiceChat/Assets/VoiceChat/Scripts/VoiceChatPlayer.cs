using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace VoiceChat
{
    [RequireComponent(typeof(AudioSource))]
    public class VoiceChatPlayer : MonoBehaviour
    {
        public event Action PlayerStarted;

        float lastTime = 0;
        double played = 0;
        double received = 0;
        int index = 0;
        float[] data;
        float playDelay = 0;
        bool shouldPlay = false;
        float lastRecvTime = 0;
        NSpeex.SpeexDecoder speexDec = new NSpeex.SpeexDecoder(NSpeex.BandMode.Narrow);

        [SerializeField]
        [Range(.1f, 5f)]
        float playbackDelay = .5f;

        [SerializeField]
        [Range(1, 32)]
        int packetBufferSize = 10;

        SortedList<ulong, VoiceChatPacket> packetsToPlay = new SortedList<ulong, VoiceChatPacket>();

        public float LastRecvTime
        {
            get { return lastRecvTime; }
        }

        void Start()
        {
            int size = VoiceChatSettings.Instance.Frequency * 10;

            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().clip = AudioClip.Create("VoiceChat", size, 1, VoiceChatSettings.Instance.Frequency, false);
            data = new float[size];

            if (VoiceChatSettings.Instance.LocalDebug)
            {
                VoiceChatRecorder.Instance.NewSample += OnNewSample;
            }

            if(PlayerStarted != null)
            {
                PlayerStarted();
            }
        }

        void Update()
        {
            if (GetComponent<AudioSource>().isPlaying)
            {
                // Wrapped around
                if (lastTime > GetComponent<AudioSource>().time)
                {
                    played += GetComponent<AudioSource>().clip.length;
                }

                lastTime = GetComponent<AudioSource>().time;

                // Check if we've played to far
                if (played + GetComponent<AudioSource>().time >= received)
                {
                    Stop();
                    shouldPlay = false;
                }
            }
            else
            {
                if (shouldPlay)
                {
                    playDelay -= Time.deltaTime;

                    if (playDelay <= 0)
                    {
                        GetComponent<AudioSource>().Play();
                    }
                }
            }
        }

        void Stop()
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().time = 0;
            index = 0;
            played = 0;
            received = 0;
            lastTime = 0;
        }

        public void OnNewSample(VoiceChatPacket newPacket)
        {
            // Set last time we got something
            lastRecvTime = Time.time;
            
            packetsToPlay.Add(newPacket.PacketId, newPacket);

            if (packetsToPlay.Count < 10)
            {
                return;
            }

            var pair = packetsToPlay.First();
            var packet = pair.Value;
            packetsToPlay.Remove(pair.Key);

            // Decompress
            float[] sample = null;
            int length = VoiceChatUtils.Decompress(speexDec, packet, out sample);

            // Add more time to received
            received += VoiceChatSettings.Instance.SampleTime;

            // Push data to buffer
            Array.Copy(sample, 0, data, index, length);

            // Increase index
            index += length;

            // Handle wrap-around
            if (index >= GetComponent<AudioSource>().clip.samples)
            {
                index = 0;
            }

            // Set data
            GetComponent<AudioSource>().clip.SetData(data, 0);
            
            // If we're not playing
            if (!GetComponent<AudioSource>().isPlaying)
            {
                // Set that we should be playing
                shouldPlay = true;

                // And if we have no delay set, set it.
                if (playDelay <= 0)
                {
                    playDelay = (float)VoiceChatSettings.Instance.SampleTime * playbackDelay;
                }
            }

            VoiceChatFloatPool.Instance.Return(sample);
        }
    } 
}