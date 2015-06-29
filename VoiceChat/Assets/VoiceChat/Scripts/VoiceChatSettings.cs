using System.Linq;
using UnityEngine;

namespace VoiceChat
{
    public class VoiceChatSettings : MonoBehaviour
    {
        #region Instance

        static VoiceChatSettings instance;

        public static VoiceChatSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(VoiceChatSettings)) as VoiceChatSettings;
                }

                return instance;
            }
        }

        #endregion

        [SerializeField]
        int frequency = 16000;

        [SerializeField]
        int sampleSize = 640;

        [SerializeField]
        VoiceChatCompression compression = VoiceChatCompression.Speex;

        [SerializeField]
        VoiceChatPreset preset = VoiceChatPreset.Speex_16K;

        [SerializeField]
        bool localDebug = false;

        public int Frequency
        {
            get { return frequency; }
            private set { frequency = value; }
        }

        public bool LocalDebug
        {
            get { return localDebug; }
            set { localDebug = value; }
        }

        public VoiceChatPreset Preset
        {
            get { return preset; }
            set
            {
                preset = value;

                switch (preset)
                {
                    case VoiceChatPreset.Speex_8K:
                        Frequency = 8000;
                        SampleSize = 320;
                        Compression = VoiceChatCompression.Speex;
                        break;

                    case VoiceChatPreset.Speex_16K:
                        Frequency = 16000;
                        SampleSize = 640;
                        Compression = VoiceChatCompression.Speex;
                        break;

                    case VoiceChatPreset.Alaw_4k:
                        Frequency = 4096;
                        SampleSize = 128;
                        Compression = VoiceChatCompression.Alaw;
                        break;

                    case VoiceChatPreset.Alaw_8k:
                        Frequency = 8192;
                        SampleSize = 256;
                        Compression = VoiceChatCompression.Alaw;
                        break;

                    case VoiceChatPreset.Alaw_16k:
                        Frequency = 16384;
                        SampleSize = 512;
                        Compression = VoiceChatCompression.Alaw;
                        break;

                    case VoiceChatPreset.Alaw_Zlib_4k:
                        Frequency = 4096;
                        SampleSize = 128;
                        Compression = VoiceChatCompression.AlawZlib;
                        break;

                    case VoiceChatPreset.Alaw_Zlib_8k:
                        Frequency = 8192;
                        SampleSize = 256;
                        Compression = VoiceChatCompression.AlawZlib;
                        break;

                    case VoiceChatPreset.Alaw_Zlib_16k:
                        Frequency = 16384;
                        SampleSize = 512;
                        Compression = VoiceChatCompression.AlawZlib;
                        break;

                    /*
                    case VoiceChatPreset.Raw_4k:
                        Frequency = 4096;
                        SampleSize = 128;
                        Compression = VoiceChatCompression.Raw;
                        break;

                    case VoiceChatPreset.Raw_8k:
                        Frequency = 8192;
                        SampleSize = 256;
                        Compression = VoiceChatCompression.Raw;
                        break;

                    case VoiceChatPreset.Raw_16k:
                        Frequency = 16384;
                        SampleSize = 512;
                        Compression = VoiceChatCompression.Raw;
                        break;

                    case VoiceChatPreset.Raw_Zlib_4k:
                        Frequency = 4096;
                        SampleSize = 128;
                        Compression = VoiceChatCompression.RawZlib;
                        break;

                    case VoiceChatPreset.Raw_Zlib_8k:
                        Frequency = 8192;
                        SampleSize = 256;
                        Compression = VoiceChatCompression.RawZlib;
                        break;

                    case VoiceChatPreset.Raw_Zlib_16k:
                        Frequency = 16384;
                        SampleSize = 512;
                        Compression = VoiceChatCompression.RawZlib;
                        break;
                        */
                }
            }
        }

        public VoiceChatCompression Compression
        {
            get { return compression; }
            private set { compression = value; }
        }

        public int SampleSize
        {
            get { return sampleSize; }
            private set { sampleSize = value; }
        }

        public double SampleTime
        {
            get { return (double)SampleSize / (double)Frequency; }
        }
    } 
}