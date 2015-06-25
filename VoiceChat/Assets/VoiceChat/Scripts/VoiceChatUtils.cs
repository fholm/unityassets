using System;
using Ionic.Zlib;
using UnityEngine;

namespace VoiceChat
{
    public static class VoiceChatUtils
    {
        static void ToShortArray(this float[] input, short[] output)
        {
            if (output.Length < input.Length)
            {
                throw new System.ArgumentException("in: " + input.Length + ", out: " + output.Length);
            }

            for (int i = 0; i < input.Length; ++i)
            {
                output[i] = (short)Mathf.Clamp((int)(input[i] * 32767.0f), short.MinValue, short.MaxValue);
            }
        }

        static void ToFloatArray(this short[] input, float[] output, int length)
        {
            if (output.Length < length || input.Length < length)
            {
                throw new System.ArgumentException();
            }

            for (int i = 0; i < length; ++i)
            {
                output[i] = input[i] / (float)short.MaxValue;
            }
        }

        static byte[] ZlibCompress(byte[] input, int length)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                using (var compressor = new Ionic.Zlib.ZlibStream(ms, CompressionMode.Compress, CompressionLevel.BestCompression))
                {
                    compressor.Write(input, 0, length);
                }

                return ms.ToArray();
            }
        }

        static byte[] ZlibDecompress(byte[] input, int length)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                using (var compressor = new Ionic.Zlib.ZlibStream(ms, CompressionMode.Decompress, CompressionLevel.BestCompression))
                {
                    compressor.Write(input, 0, length);
                }

                return ms.ToArray();
            }
        }

        static byte[] ALawCompress(float[] input)
        {
            byte[] output = VoiceChatBytePool.Instance.Get();

            for (int i = 0; i < input.Length; ++i)
            {
                int scaled = (int)(input[i] * 32767.0f);
                short clamped = (short)Mathf.Clamp(scaled, short.MinValue, short.MaxValue);
                output[i] = NAudio.Codecs.ALawEncoder.LinearToALawSample(clamped);
            }

            return output;
        }

        static float[] ALawDecompress(byte[] input, int length)
        {
            float[] output = VoiceChatFloatPool.Instance.Get();

            for (int i = 0; i < length; ++i)
            {
                short alaw = NAudio.Codecs.ALawDecoder.ALawToLinearSample(input[i]);
                output[i] = alaw / (float)short.MaxValue;
            }

            return output;
        }

        static NSpeex.SpeexEncoder speexEnc = new NSpeex.SpeexEncoder(NSpeex.BandMode.Narrow);

        static byte[] SpeexCompress(float[] input, out int length)
        {
            short[] shortBuffer = VoiceChatShortPool.Instance.Get();
            byte[] encoded = VoiceChatBytePool.Instance.Get();
            input.ToShortArray(shortBuffer);
            length = speexEnc.Encode(shortBuffer, 0, input.Length, encoded, 0, encoded.Length);
            VoiceChatShortPool.Instance.Return(shortBuffer);
            return encoded;
        }

        static float[] SpeexDecompress(NSpeex.SpeexDecoder speexDec, byte[] data, int dataLength)
        {
            float[] decoded = VoiceChatFloatPool.Instance.Get();
            short[] shortBuffer = VoiceChatShortPool.Instance.Get();
            speexDec.Decode(data, 0, dataLength, shortBuffer, 0, false);
            shortBuffer.ToFloatArray(decoded, shortBuffer.Length);
            VoiceChatShortPool.Instance.Return(shortBuffer);
            return decoded;
        }

        public static VoiceChatPacket Compress(float[] sample)
        {
            VoiceChatPacket packet = new VoiceChatPacket();
            packet.Compression = VoiceChatSettings.Instance.Compression;

            switch (packet.Compression)
            {
                /*
                case VoiceChatCompression.Raw:
                    {
                        short[] buffer = VoiceChatShortPool.Instance.Get();

                        packet.Length = sample.Length * 2;
                        sample.ToShortArray(shortBuffer);
                        Buffer.BlockCopy(shortBuffer, 0, byteBuffer, 0, packet.Length);
                    }
                    break;

                case VoiceChatCompression.RawZlib:
                    {
                        packet.Length = sample.Length * 2;
                        sample.ToShortArray(shortBuffer);
                        Buffer.BlockCopy(shortBuffer, 0, byteBuffer, 0, packet.Length);

                        packet.Data = ZlibCompress(byteBuffer, packet.Length);
                        packet.Length = packet.Data.Length;
                    }
                    break;
                */

                case VoiceChatCompression.Alaw:
                    {
                        packet.Length = sample.Length;
                        packet.Data = ALawCompress(sample);
                    }
                    break;

                case VoiceChatCompression.AlawZlib:
                    {
                        byte[] alaw = ALawCompress(sample);
                        packet.Data = ZlibCompress(alaw, sample.Length);
                        packet.Length = packet.Data.Length;

                        VoiceChatBytePool.Instance.Return(alaw);
                    }
                    break;

                case VoiceChatCompression.Speex:
                    {
                        packet.Data = SpeexCompress(sample, out packet.Length);
                    }
                    break;
            }

            return packet;


        }

        public static int Decompress(VoiceChatPacket packet, out float[] data)
        {
            return Decompress(null, packet, out data);
        }

        public static int Decompress(NSpeex.SpeexDecoder speexDecoder, VoiceChatPacket packet, out float[] data)
        {
            switch (packet.Compression)
            {
                /*
                case VoiceChatCompression.Raw:
                    {
                        short[9 buffer 

                        Buffer.BlockCopy(packet.Data, 0, shortBuffer, 0, packet.Length);
                        shortBuffer.ToFloatArray(data, packet.Length / 2);
                        return packet.Length / 2;
                    }

                case VoiceChatCompression.RawZlib:
                    {
                        byte[] unzipedData = ZlibDecompress(packet.Data, packet.Length);

                        Buffer.BlockCopy(unzipedData, 0, shortBuffer, 0, unzipedData.Length);
                        shortBuffer.ToFloatArray(data, unzipedData.Length / 2);
                        return unzipedData.Length / 2;
                    }
                */

                case VoiceChatCompression.Speex:
                    {
                        data = SpeexDecompress(speexDecoder, packet.Data, packet.Length);
                        return data.Length;
                    }

                case VoiceChatCompression.Alaw:
                    {
                        data = ALawDecompress(packet.Data, packet.Length);
                        return packet.Length;
                    }

                case VoiceChatCompression.AlawZlib:
                    {
                        byte[] alaw = ZlibDecompress(packet.Data, packet.Length);
                        data = ALawDecompress(alaw, alaw.Length);
                        return alaw.Length;
                    }
            }

            data = new float[0];
            return 0;
        }

        public static int ClosestPowerOfTwo(int value)
        {
            int i = 1;

            while (i < value)
            {
                i <<= 1;
            }

            return i;
        }
    } 
}