using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

namespace Delivery.Track
{
    public class TrackDrawGrid : MonoBehaviour
    {
        private float _gridWidth = 120f; //格子宽
        private float _gridHeight = 130f;//格子高
        private int _gridRow = 10;//行数
        private int _gridCol = 6;//列数  
        Vector2 _leftbot;//网格最左下角坐标
        private Transform _lineParent;
        private Camera _uiCamera;  
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
        }
        public void RandomItemPos(RectTransform item)
        {
            Vector2 vecSize = item.sizeDelta;
            int x = Random.Range(0, _gridCol + 1);
            int y = Random.Range(0, _gridRow + 1);
            item.anchoredPosition3D = new Vector3(_leftbot.x + x * _gridWidth, _leftbot.y + y * _gridHeight, 0);


            Vector3 worldZero = _uiCamera.ScreenToWorldPoint(Vector2.zero);  //opengl坐标（左下角为0,0）
            Vector2 center = item.position;
            Vector2 sizeDelta = vecSize * RootCanvas.Instance.AdapterFit();
            Vector3 sizeDeltaWorldPos = _uiCamera.ScreenToWorldPoint(sizeDelta);
            Vector2 boxSize = sizeDeltaWorldPos - worldZero;
            RaycastHit2D[] hit2D = Physics2D.BoxCastAll(center, boxSize, 0, Vector2.zero, 10);
            float halfWidth = boxSize.x / 2f;
            float halfHeight = boxSize.y / 2f;
            Debug.DrawLine(new Vector3(center.x + halfWidth, center.y - halfHeight, 0), new Vector3(center.x + halfWidth, center.y + halfHeight, 0), Color.red, 1000);
            Debug.DrawLine(new Vector3(center.x - halfWidth, center.y - halfHeight, 0), new Vector3(center.x - halfWidth, center.y + halfHeight, 0), Color.red, 1000);
            Debug.DrawLine(new Vector3(center.x - halfWidth, center.y + halfHeight, 0), new Vector3(center.x + halfWidth, center.y + halfHeight, 0), Color.red, 1000);
            Debug.DrawLine(new Vector3(center.x - halfWidth, center.y - halfHeight, 0), new Vector3(center.x + halfWidth, center.y - halfHeight, 0), Color.red, 1000);
            for (int i = 0; i < hit2D.Length; i++)
            {
                //LogUtility.LogInfo($"hit2d=={i}=={hit2D[i].transform.name}");
                if (hit2D[i].transform.tag == Tags.Item)
                {
                    RandomItemPos(item);
                    return;
                }
            }
        } 
    }
}

