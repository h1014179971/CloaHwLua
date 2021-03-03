using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vectrosity;

namespace Delivery.TrackEditor
{
    public class TrackEditorDrawGrid : MonoBehaviour
    {
        private float _gridWidth = 120f; //格子宽
        private float _gridHeight = 130f;//格子高
        private int _gridRow = 10;//行数
        private int _gridCol = 6;//列数  
        Vector2 _leftbot;//网格最左下角坐标
        private Transform _lineParent;
        private Camera _uiCamera;
        private List<Vector2> _points;
        private List<Vector2> _usedPoints = new List<Vector2>();//已经使用了的位置
        void Start()
        {
            _uiCamera = RootCanvas.Instance.UICamera;
            _lineParent = this.GetComponentByPath<Transform>("bg/line");
            _leftbot = new Vector2(-FixScreen.width * 0.5f, -FixScreen.height * 0.5f);
            _leftbot.x += (FixScreen.width - _gridWidth * _gridCol) * 0.5f;
            _leftbot.y += 150;
            for (int i = 0; i <= _gridCol; i++)
            {
                VectorLine line = new VectorLine("touchCol" + i, new List<Vector2>(), null, 4, LineType.Continuous, Joins.Fill);
                //line.SetCanvas(RootCanvas.Instance.UICanvas, true);
                line.rectTransform.SetParent(_lineParent);
                line.rectTransform.anchoredPosition3D = Vector3.zero;
                line.rectTransform.localScale = Vector3.one;
                line.points2.Add(new Vector2(_leftbot.x + i * _gridWidth, _leftbot.y));
                line.points2.Add(new Vector2(_leftbot.x + i * _gridWidth, _leftbot.y + _gridRow * _gridHeight));
                //line.SetColor(Color.red);
                line.Draw();
            }
            for (int i = 0; i <= _gridRow; i++)
            {
                VectorLine line = new VectorLine("touchRow" + i, new List<Vector2>(), null, 4, LineType.Continuous, Joins.Fill);
                //line.SetCanvas(RootCanvas.Instance.UICanvas, true);
                line.rectTransform.SetParent(_lineParent);
                line.rectTransform.anchoredPosition3D = Vector3.zero;
                line.rectTransform.localScale = Vector3.one;
                line.points2.Add(new Vector2(_leftbot.x, _leftbot.y + i * _gridHeight));
                line.points2.Add(new Vector2(_leftbot.x + _gridCol * _gridWidth, _leftbot.y + i * _gridHeight));
                line.Draw();
            }
            CreatePos();
        }
        private void CreatePos()
        {
            _points = new List<Vector2>();
            Vector2 pos = Vector2.zero;
            for (int i =0;i<= _gridCol; i++)
            {
                for(int j =0;j<= _gridRow; j++)
                {
                    pos = new Vector2(_leftbot.x + i * _gridWidth, _leftbot.y + j * _gridHeight);
                    _points.Add(pos);
                    //GameObject img = new GameObject();
                    //img.name = "img" + _points.Count;
                    //img.AddComponent<Image>();
                    //img.transform.SetParent(_lineParent);
                    //RectTransform rectTrans = img.GetComponent<RectTransform>();
                    //rectTrans.anchoredPosition = pos;
                    //rectTrans.localScale = Vector3.one;
                    //GameObject prefab = Resources.Load<GameObject>("Prefabs/MyText");
                    //GameObject txt = Instantiate<GameObject>(prefab);
                    //MyText t = txt.GetOrAddComponent<MyText>();
                    //t.color = Color.red;
                    //t.fontSize = 20;
                    //t.text = _points.Count.ToString();
                    //t.alignment = TextAnchor.MiddleCenter;
                    //txt.transform.SetParent(rectTrans);
                    //RectTransform tRectTrans = txt.GetComponent<RectTransform>();
                    //tRectTrans.anchoredPosition = Vector2.zero;
                    //tRectTrans.localScale = Vector3.one;
                }
            }
        }
        public Vector2 GetTrackEditorModelPos(RectTransform item)
        {
            Vector2 pos = new Vector2(-10000, -10000);
            float dis = 300;
            for(int i = 0; i < _points.Count; i++)
            {
                if (_usedPoints.Contains(_points[i])) continue;
                float mag = (item.anchoredPosition - _points[i]).magnitude;
                if(mag <dis)
                {
                    dis = mag;
                    pos = _points[i];
                     
                }   
            }
            if(pos != new Vector2(-10000, -10000))
                _usedPoints.Add(pos);
            return pos;
        }
        public void RemoveUsedPos(RectTransform item)
        {
            if (_usedPoints.Contains(item.anchoredPosition))
                _usedPoints.Remove(item.anchoredPosition);
        }
    }
}

