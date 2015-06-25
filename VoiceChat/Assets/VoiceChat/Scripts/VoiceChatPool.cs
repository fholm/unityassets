using System.Collections.Generic;

namespace VoiceChat
{
    public abstract class VoiceChatPool<T>
where T : class
    {
        Queue<T> queue = new Queue<T>();

        public T Get()
        {
            if (queue.Count > 0)
            {
                return queue.Dequeue();
            }

            return Create();
        }

        public void Return(T obj)
        {
            if (obj != null)
            {
                queue.Enqueue(obj);
            }
        }

        protected abstract T Create();
    }

    public class VoiceChatBytePool : VoiceChatPool<byte[]>
    {
        public static readonly VoiceChatBytePool Instance = new VoiceChatBytePool();

        VoiceChatBytePool()
        {

        }

        protected override byte[] Create()
        {
            return new byte[VoiceChatSettings.Instance.SampleSize];
        }
    }

    public class VoiceChatShortPool : VoiceChatPool<short[]>
    {
        public static readonly VoiceChatShortPool Instance = new VoiceChatShortPool();

        VoiceChatShortPool()
        {

        }

        protected override short[] Create()
        {
            return new short[VoiceChatSettings.Instance.SampleSize];
        }
    }

    public class VoiceChatFloatPool : VoiceChatPool<float[]>
    {
        public static readonly VoiceChatFloatPool Instance = new VoiceChatFloatPool();

        VoiceChatFloatPool()
        {

        }

        protected override float[] Create()
        {
            return new float[VoiceChatSettings.Instance.SampleSize];
        }
    } 
}