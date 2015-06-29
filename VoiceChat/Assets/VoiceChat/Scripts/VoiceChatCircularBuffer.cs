using System;
using System.Collections.Generic;

namespace VoiceChat
{
    public class VoiceChatCircularBuffer<T>
    {
        int capacity;
        int count;
        int head;
        int tail;
        T[] buffer;

        public bool HasItems
        {
            get { return count > 0; }
        }

        public int TailIndex
        {
            get { return tail; }
        }

        public T[] Data
        {
            get { return buffer; }
        }

        public int Count
        {
            get { return count; }
        }

        public int Capacity
        {
            get { return capacity; }
        }

        public VoiceChatCircularBuffer(int maxCapacity)
        {
            capacity = maxCapacity;
            buffer = new T[capacity];
        }

        public void Add(T item)
        {
            if (count == capacity && head == tail)
            {
                if (++tail == capacity)
                {
                    tail = 0;
                }
            }

            buffer[head] = item;

            if (++head == capacity)
            {
                head = 0;
            }

            count = Math.Min(capacity, count + 1);
        }

        public T Remove()
        {
            if (count == 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            T item = buffer[tail];

            if (++tail == capacity)
            {
                tail = 0;
            }

            --count;

            return item;
        }

        public bool NextIndex(ref int value)
        {
            if (++value == capacity)
            {
                value = 0;
            }

            return value != head;
        }
    }

}