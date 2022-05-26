using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColaFramework.Foundation
{
    public class DoubleLinkedList<T> : IEnumerable<T>,IEnumerator<T>
    {
        public class DoubleLinkedNode
        {
            public T Value;
            public DoubleLinkedNode Prior;
            public DoubleLinkedNode Next;

            public DoubleLinkedNode()
            {

            }
            public DoubleLinkedNode(T t)
            {
                Value = t;
            }
            public DoubleLinkedNode(DoubleLinkedNode another)
            {
                this.Value = another.Value;
                this.Prior = another.Prior;
                this.Next = another.Next;
            }

            public void AddPrior(DoubleLinkedNode another)
            {
                this.Prior = another;
                another.Next = this;
            }

            public void AddNext(DoubleLinkedNode another)
            {
                this.Next = another;
                another.Prior = this;
            }

            public void Clear()
            {
                Prior = null;
                Next = null;
            }
        }

        public DoubleLinkedNode Head;
        public DoubleLinkedNode End;

        public int Size
        {
            get { return _size; }
        }

        private int _size;
        private int _cursor = -1;
        private DoubleLinkedNode _cursorNode = null;
        object IEnumerator.Current => Current;

        public T Current
        {
            get { return _cursorNode.Value; }
        }


        public bool MoveNext()
        {
            _cursor++;
            if (_cursor == 0)
                _cursorNode = Head;
            else
                _cursorNode = _cursorNode.Next;
            return (_cursor < _size);
        }

        public void Reset()
        {
            _cursor = -1;
            _cursorNode = null;
        }


        public void Dispose()
        {
            ;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            Reset();
            return this;
        }


        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            Reset();
            return this;
        }

        public DoubleLinkedList()
        {
            _size = 0;
        }

        public DoubleLinkedList(DoubleLinkedList<T> another)
        {
            _size = 0;
            foreach (var t in another)
            {
                PushBack(t);
            }
        }

        public T this[int index]
        {
            get
            {
                return Index(index).Value;
            }
            set
            {
                Index(index).Value = value;
            }
        }


        public static DoubleLinkedList<T> operator +(DoubleLinkedList<T> b, DoubleLinkedList<T> c)
        {
            DoubleLinkedList<T> a = new DoubleLinkedList<T>(b);
            foreach (var t in c)
            {
                a.PushBack(t);
            }

            a._size = b._size + c._size;
            return a;
        }

        public bool Contain(T find)
        {
            int index = Find(find);
            if (index == -1)
                return false;
            return true;
        }

        public int Find(T find)
        {
            foreach (var t in this)
            {
                if (t.Equals(find))
                    return _cursor;
            }
            return -1;
        }

        private DoubleLinkedNode Index(int p)
        {
            if (p >= _size)
                return null;
            DoubleLinkedNode node = Head;
            while (p > 0)
            {
                node = node.Next;
                p--;
            }
            return node;
        }

        public void PushHead(T value)
        {
            DoubleLinkedNode node = new DoubleLinkedNode(value);
            if (Head != null)
                Head.AddPrior(node);
            else
                End = node;

            Head = node;
            _size++;
        }

        public void PushBack(T value)
        {
            DoubleLinkedNode node = new DoubleLinkedNode(value);
            if (End != null)
                End.AddNext(node);
            else
                Head = node;

            End = node;
            _size++;
        }

        public void PushAt(T value, int index)
        {
            if (index > _size || index < 0)
                return;
            if (index == 0)
                PushHead(value);
            else if (index == _size)
                PushBack(value);
            else
            {
                DoubleLinkedNode node = new DoubleLinkedNode(value);
                DoubleLinkedNode next = Index(index);
                DoubleLinkedNode prior = Index(index - 1);
                next.AddPrior(node);
                prior.AddNext(node);
                _size++;
            }
        }

        public T PopBack()
        {
            if (_size <= 0)
                return default;
            DoubleLinkedNode node = End;
            if (node.Prior != null)
            {
                End = node.Prior;
                End.Next = null;
            }
            else
            {
                Head = null;
                End = null;
            }
            _size--;
            node.Clear();
            return node.Value;
        }

        public T PopHead()
        {
            if (_size <= 0)
                return default;
            DoubleLinkedNode node = Head;
            if (node.Next != null)
            {
                Head = node.Next;
                Head.Prior = null;
            }
            else
            {
                Head = null;
                End = null;
            }
            _size--;
            node.Clear();
            return node.Value;
        }

        public T PopAt(int index)
        {
            if (index > _size - 1 || index < 0)
                return default;
            if (index == 0)
                return PopHead();
            else if (index == _size - 1)
                return PopBack();
            else
            {
                DoubleLinkedNode node = Index(index);
                node.Prior.Next = node.Next;
                node.Next.Prior = node.Prior;
                node.Clear();
                _size--;
                return node.Value;
            }
        }

        public void Pop(T value)
        {
            int i = Find(value);
            if (i <= -1)
                return;
            PopAt(i);
        }

        public void MoveToBack(int index)
        {
            if (index < 0 || index >= _size)
                return;
            PushBack(PopAt(index)); ;
        }

        public void MoveToHead(int index)
        {
            if (index < 0 || index >= _size)
                return;
            PushHead(PopAt(index));
        }


        public void Switch(int a, int b)
        {
            if (a == b)
                return;
            if (a > b)
            {
                int temp = a;
                a = b;
                b = temp;
            }
            PushAt(PopAt(a), b - 1);
            PushAt(PopAt(b), a);
        }

        public bool IsHead<T>(T value)
        {
            if (Head == null)
                return false;
            if (Head.Value.Equals(value))
                return true;
            return false;
        }

        public bool IsEnd<T>(T value)
        {
            if (End == null)
                return false;
            if (End.Value.Equals(value))
                return true;
            return false;
        }

    }
}

