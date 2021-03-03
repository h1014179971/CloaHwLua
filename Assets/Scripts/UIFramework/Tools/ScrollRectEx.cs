using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UIFramework
{
    public class ScrollRectEx : MonoBehaviour
    {
        public enum Arrangement
        {
            Horizontal,
            Vertical,
        }

        public Arrangement _movement = Arrangement.Horizontal;
        public bool isInitStar = false;
        //Item之间的距离
        [Range(0, 100)]
        public int cellPadiding = 5;
        //Item的宽高
        public int cellWidth = 500;
        public int cellHeight = 100;
        //默认加载的Item个数，一般比可显示个数大2~3个
        [Range(0, 20)]
        public int viewCount = 6;
        public RectTransform _content;
        public ScrollRect _scrollRect;

        private int _index = -1;
        public List<ScrollRectItem> _itemList;
        private int _dataCount;
        private int _maxCount = 10;
        public Action<int> GetIndex;

        private void Start()
        {
            if (isInitStar)
            {
                MaxCount = 10;
                //DataCount = viewCount;
                Init();
            }
        }

        public void Init()
        {
            _index = -1;
            _content.anchoredPosition = Vector2.zero;
            _scrollRect.onValueChanged.RemoveAllListeners();
            _scrollRect.onValueChanged.AddListener(OnValueChange);
            InitData();
        }

        private void InitData()
        {
            const int index = 0;
            if (_index != index && index > -1)
            {
                _index = index;
                for (int i = _index; i < _itemList.Count; i++)
                {
                    CreateItem(i);
                }
            }
        }


        public void OnValueChange(Vector2 pos)
        {

            //Debug.Log("pos======" + pos);
            int index = GetPosIndex();
            if (index + viewCount > MaxCount) return;
            //Debug.Log("index======" + index);
            if (_index != index && index > -1)
            {
                if (index > _index)
                {
                    if (DataCount >= MaxCount)
                    {
                        return;
                    }
                    DataCount += 1;
                    _index += 1;
                }
                else
                {
                    DataCount -= 1;
                    _index -= 1;
                }
                //_index = index;

                for (int i = _itemList.Count; i > 0; i--)
                {
                    ScrollRectItem item = _itemList[i - 1];
                    if (item.Index < index)
                    {
                        item.Index += viewCount;
                    }
                    else if (item.Index >= index + viewCount)
                    {
                        item.Index -= viewCount;
                    }
                }

            }
            //GetIndex(_index);
        }

        private void CreateItem(int index)
        {
            ScrollRectItem go = _itemList[index];
            if (go != null)
            {
                go.Init();
                go.Scroller = this;
                go.Index = index;

            }
        }
        public void UpdateItem()
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                ScrollRectItem go = _itemList[i];
                go.CreateItem();
            }
        }

        private int GetPosIndex()
        {
            switch (_movement)
            {
                case Arrangement.Horizontal:
                    return Mathf.FloorToInt(_content.anchoredPosition.x / -(cellWidth + cellPadiding));
                case Arrangement.Vertical:
                    return Mathf.FloorToInt(_content.anchoredPosition.y / (cellHeight + cellPadiding));
            }
            return 0;
        }

        public Vector3 GetPosition(int i)
        {
            switch (_movement)
            {
                case Arrangement.Horizontal:
                    return new Vector3(i * (cellWidth + cellPadiding), 0f, 0f);
                case Arrangement.Vertical:
                    return new Vector3(0f, i * -(cellHeight + cellPadiding), 0f);
            }
            return Vector3.zero;
        }

        public int DataCount
        {
            get { return _dataCount; }
            set
            {
                _dataCount = value;
                UpdateTotalWidth();
            }
        }

        public int MaxCount
        {
            get { return _maxCount; }
            set
            {
                _maxCount = value;
                int min = Mathf.Min(viewCount, _maxCount);
                //AddItem(_maxCount);//item没有复用
                AddItem(min); //item复用
                DataCount = viewCount;
                Init();
            }
        }
        public void AddMaxCount(int value = 1)
        {
            _maxCount += value;
        }
        public void AddItem(int min)
        {
            Transform Content = transform.Find("Viewport/Content");
            GameObject obj = Content.GetChild(0).gameObject;

            for (int i = 0; i < min; i++)
            {
                if (i == 0)
                {
                    _itemList.Add(obj.GetComponent<ScrollRectItem>());
                    continue;
                }
                GameObject clone = GameObject.Instantiate(obj) as GameObject;
                clone.transform.SetParent(Content, true);
                clone.GetComponent<RectTransform>().localScale = Vector3.one;
                _itemList.Add(clone.GetComponent<ScrollRectItem>());
            }

        }
        public void AddItem(bool repeat = true/*是否重复使用*/)
        {
            if (repeat && _itemList.Count >= viewCount)
            {
                for (int i = _index; i < _itemList.Count; i++)
                {
                    CreateItem(i);
                }
                return;
            }
            Transform Content = transform.Find("Viewport/Content");
            GameObject obj = Content.GetChild(0).gameObject;
            GameObject clone = GameObject.Instantiate(obj) as GameObject;
            clone.transform.SetParent(Content, true);
            clone.GetComponent<RectTransform>().localScale = Vector3.one;
            _itemList.Add(clone.GetComponent<ScrollRectItem>());
            for (int i = _index; i < _itemList.Count; i++)
            {
                CreateItem(i);
            }
            if (!repeat)
            {
                _content.GetComponent<ContentSizeFitter>().enabled = false;
                Invoke("DelayContentShow", 0.05f);
                //_content.GetComponent<ContentSizeFitter>().enabled = true;
                //_content.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                //_content.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }
        void DelayContentShow()
        {
            _content.GetComponent<ContentSizeFitter>().enabled = true;
        }
        private void UpdateTotalWidth()
        {
            switch (_movement)
            {
                case Arrangement.Horizontal:
                    _content.sizeDelta = new Vector2(cellWidth * _dataCount + cellPadiding * (_dataCount - 1), _content.sizeDelta.y);
                    break;
                case Arrangement.Vertical:
                    _content.sizeDelta = new Vector2(_content.sizeDelta.x, cellHeight * _dataCount + cellPadiding * (_dataCount - 1));
                    break;
            }
        }
        public void UpdateContne(int count)
        {
            for (int i = 1; i < _itemList.Count; i++)
            {
                Destroy(_itemList[i].gameObject);
            }
            _itemList.Clear();
            MaxCount = count;
        }
    }
}

