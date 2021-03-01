using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

namespace Delivery
{
    public class DrawLineCtrl : MonoBehaviour
    {
        private float gridWidth = 120f; //格子宽
        private float gridHeight = 130f;//格子高
        private int gridRow = 10;//行数
        private int gridCol = 6;//列数  
        Vector2 leftbot;//网格最左下角坐标
        private Transform lineParent;
        private Camera uiCamera;
        [SerializeField]private ItemCtrl itemCtrl;                                                                         
        void Start()
        {
            uiCamera = RootCanvas.Instance.UICamera;
            lineParent = this.GetComponentByPath<Transform>("bg/line");
            leftbot = new Vector2(-FixScreen.width * 0.5f, -FixScreen.height * 0.5f);
            leftbot.x += (FixScreen.width - gridWidth * gridCol) * 0.5f;
            leftbot.y += 150;
            for(int i = 0; i <= gridCol; i++)
            {
                VectorLine line = new VectorLine("touchCol"+i, new List<Vector2>(), null, 4, LineType.Continuous, Joins.Fill);
                //line.SetCanvas(RootCanvas.Instance.UICanvas, true);
                line.rectTransform.SetParent(lineParent);
                line.rectTransform.anchoredPosition3D = Vector3.zero;
                line.rectTransform.localScale = Vector3.one;
                line.points2.Add(new Vector2(leftbot.x + i * gridWidth, leftbot.y));
                line.points2.Add(new Vector2(leftbot.x + i * gridWidth, leftbot.y + gridRow * gridHeight));
                //line.SetColor(Color.red);
                line.Draw();
            }
            for(int i =0;i<= gridRow; i++)
            {
                VectorLine line = new VectorLine("touchRow" + i, new List<Vector2>(), null, 4, LineType.Continuous, Joins.Fill);
                //line.SetCanvas(RootCanvas.Instance.UICanvas, true);
                line.rectTransform.SetParent(lineParent);
                line.rectTransform.anchoredPosition3D = Vector3.zero;
                line.rectTransform.localScale = Vector3.one;
                line.points2.Add(new Vector2(leftbot.x , leftbot.y + i * gridHeight));
                line.points2.Add(new Vector2(leftbot.x  + gridCol * gridWidth, leftbot.y + i * gridHeight));
                line.Draw();
            }
            itemCtrl.Init(this);
        }
        public void RandomItemPos(RectTransform item)
        {
            Vector2 vecSize = item.sizeDelta;
            int x = Random.Range(0, gridCol+1);
            int y = Random.Range(0, gridRow+1);
            item.anchoredPosition3D = new Vector3(leftbot.x + x * gridWidth, leftbot.y + y * gridHeight, 0);


            Vector3 worldZero = uiCamera.ScreenToWorldPoint(Vector2.zero);  //opengl坐标（左下角为0,0）
            Vector2 center = item.position;
            Vector2 sizeDelta = vecSize * RootCanvas.Instance.AdapterFit();
            Vector3 sizeDeltaWorldPos = uiCamera.ScreenToWorldPoint(sizeDelta);
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


