using System;
using UnityEngine;

namespace Framework.Tools
{
    public interface IUserDefEnumerator<out T>
    {
        T Current { get; }
        bool MoveNext();
    }
    public class SmartQueue<T>
    {
        private T[] array;
        private int size, caption;
        private int head, tail;

        public int Count { get { return size; } }
        
        public SmartQueue(int InCaption)
        {
            caption = InCaption;
            array = new T[caption];
            size = head = tail = 0;
        }

        public void Clear()
        {
            if (size > 0)
            {
                for (int i = head; i < tail; ++i)
                {
                    array[i] = default(T);
                }

                size = head = tail = 0;
            }
        }

        public void Enqueue(T InValue)
        {
            if (size < caption)
            {
                array[tail] = InValue;
                tail = (tail + 1) % caption;
                ++size;
            }
            else Debug.LogError("Out of range.");

            //Debug.LogError("->" + tail + ", " + size + ", " + InValue );
        }

        public void Push(T InValue)
        {
            if (size < caption)
            {
                head = (head + caption - 1) % caption;
                array[head] = InValue;
                ++size;
            }
            else throw new IndexOutOfRangeException();
        }

        public T Pop()
        {
            if (size > 0)
            {
                tail = (tail + caption - 1) % caption;
                T result = array[tail];
                array[tail] = default(T);
                --size;

                return result;
            }
            else throw new IndexOutOfRangeException();
        }

        public T Dequeue()
        {
            if (size > 0)
            {
                T result = array[head];
                array[head] = default(T);
                head = (head + 1) % caption;
                --size;
                //Debug.LogError("->" + head + ", " + size + ", " + result);
                return result;
            }
            else throw new Exception("Not value in smart queue.");
        }

        public void InsertBehindHead(int InIndex, T InValue)
        {
            if (size < caption)
            {
                if (size < caption)
                {
                    if (InIndex < size)
                    {
                        int index = (head + InIndex) % caption;
                        T cur = array[index], next;
                        array[index] = InValue;
                        while (index != tail)
                        {
                            index = (index + 1) % caption;
                            next = array[index];
                            array[index] = cur;
                            cur = next;
                        }

                        tail = (tail + 1) % caption;
                        ++size;
                    }
                    else Enqueue(InValue);
                }
                else Debug.LogError("Insert value behind the tail.");
            }
        }

        public T Peek()
        {
            if (size > 0) return array[head];
            else throw new Exception("Not value in smart queue.");
        }

        /// <summary>
        /// 正序遍历， 从 head -> tail
        /// </summary>
        public class SmartQueueAsEnumerator : IUserDefEnumerator<T>
        {
            public T[] array;
            public int index, head, size, caption;

            public T Current
            {
                get { return array[(head + index) % caption]; }
            }

            public SmartQueueAsEnumerator(ref T[] InArray, int InHead, int InSize, int InCaption)
            {
                array = InArray;
                head = InHead;
                size = InSize;
                caption = InCaption;
                index = -1;
            }

            public bool MoveNext()
            {
                ++index;
                return index < size;
            }
        }

        /// <summary>
        /// 倒序遍历，tail -> head
        /// </summary>
        public class SmartQueueDeEnumerator : IUserDefEnumerator<T>
        {
            public T[] array;
            public int index, tail, size, caption;

            public T Current
            {
                get
                {
                    //Debug.LogError("-->" + tail + "," + size + "," + index);
                    //Debug.LogError((tail - index) % caption);
                    return array[(tail - index) % caption];
                }
            }

            public SmartQueueDeEnumerator(ref T[] InArray, int InTail, int InSize, int InCaption)
            {
                array = InArray;
                tail = InCaption + InTail - 1;
                size = InSize;
                index = -1;
                caption = InCaption;
            }
            
            public bool MoveNext()
            {
                ++index;
                return index < size;
            }
        }

        /// <summary>
        /// 正序遍历， 从 head -> tail
        /// </summary>
        public IUserDefEnumerator<T> GetAscendingEnumerator()
        {
            return new SmartQueueAsEnumerator(ref array, head, size, caption);
        }

        /// <summary>
        /// 倒序遍历，tail -> head
        /// </summary>
        public IUserDefEnumerator<T> GetDecendingEnumerator()
        {
            return new SmartQueueDeEnumerator(ref array, tail, size, caption);
        }
    }
}