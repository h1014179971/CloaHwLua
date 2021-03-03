using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Vectrosity;
using UnityEngine.UI;

namespace Delivery
{
    public class DrawLinesMouse : MonoBehaviour
    {
        public Texture2D lineTex;
        public int maxPoints = 5000;
        public float lineWidth = 4.0f;
        public int minPixelMove = 5;
        private VectorLine line;
        private Vector3 previousPosition;
        private int sqrMinPixelMove;

        PointerEventData eventData;
        List<ItemModel> itemModules;
        private LayerMask layerGround;

        public Image image1;
        public Image image2;
        void Start()
        {
            line = new VectorLine("DrawnLine", new List<Vector2>(), lineTex ?? null, lineWidth, LineType.Continuous, Joins.Fill);
            line.SetCanvas(this.GetComponent<Canvas>(), true);
            line.endPointsUpdate = 2;   //只更新最后几个点的优化，其余的不重新计算
            sqrMinPixelMove = minPixelMove * minPixelMove;

            eventData = new PointerEventData(EventSystem.current);
            itemModules = new List<ItemModel>();
            layerGround = 1 << LayerMask.NameToLayer("UI_Pass");

            Vector2 v1 = image1.GetComponent<RectTransform>().anchoredPosition;
            Vector2 v2 = image2.GetComponent<RectTransform>().anchoredPosition;
            Debug.Log($"v1:{v1}=>v2:{v2}");
            float angle1 = Vector2.Angle(Vector2.right, v2 - v1);
            Debug.Log($"v1 to v2:{angle1}");
            float angle1_1 = Utility.Angle_360(v1, v2);
            Debug.Log($"v1 to v2 angle1_1:{angle1_1}");
            float angle2 = Vector2.Angle(Vector2.right, v1 - v2);
            Debug.Log($"v2 to v1:{angle2}");
            if (Utility.IsAngle_45(v1, v2))
            {
                line.points2.Add(v1);
                line.points2.Add(v2);
                line.Draw();
            }
            else
            {
                object data1 = Utility.LineAB(v1, v2, true);
                Debug.Log($"data1=========={data1}");
                object data2 = Utility.LineAB(v2, v1, false);
                Debug.Log($"data2============{data2}");
                if (data1 is Vector2 && data2 is Vector2)
                {
                    Vector2 vec1 = (Vector2)data1;
                    Vector2 vec2 = (Vector2)data2;
                    float x = (vec2.y - vec1.y) / (vec1.x - vec2.x);
                    float y = vec1.x * x + vec1.y;
                    Vector2 mid = new Vector2(x, y);
                    Debug.Log($"x==={x}:y==={y}");
                    line.points2.Add(v1);
                    line.points2.Add(mid);
                    line.points2.Add(v2);
                    line.Draw();
                }
                else if (data1 is float && data2 is Vector2)
                {
                    Vector2 vec2 = (Vector2)data2;
                    float x = v1.x;
                    float y = vec2.x * x + vec2.y;
                    Vector2 mid = new Vector2(x, y);
                    Debug.Log($"x==={x}:y==={y}");
                    line.points2.Add(v1);
                    line.points2.Add(mid);
                    line.points2.Add(v2);
                    line.Draw();
                }
                else if (data1 is Vector2 && data2 is float)
                {
                    Vector2 vec1 = (Vector2)data1;
                    float x = v2.x;
                    float y = vec1.x * x + vec1.y;
                    Vector2 mid = new Vector2(x, y);
                    Debug.Log($"x==={x}:y==={y}");
                    line.points2.Add(v1);
                    line.points2.Add(mid);
                    line.points2.Add(v2);
                    line.Draw();
                }
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.touchCount > 1) return;
            if (Input.GetMouseButtonDown(0))
            {
                //line.Draw();
                //previousPosition = Input.mousePosition;
                //line.points2.Add(previousPosition);  
                bool isPointer = IsPointerOverPass(Input.mousePosition);
                Debug.Log($"isPointer=={isPointer}");


            }
            else if (Input.GetMouseButton(0) && (Input.mousePosition - previousPosition).sqrMagnitude > sqrMinPixelMove)
            {
                //previousPosition = Input.mousePosition;
                //line.points2.Add(previousPosition);
                //line.Draw();
            }
        }

        /// <summary>
        /// 检测是否点击UI
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        private bool IsPointerOverPass(Vector2 mousePosition)
        {
            eventData.position = mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            //向点击位置发射一条射线，检测是否点击UI
            EventSystem.current.RaycastAll(eventData, results);
            if (results.Count > 0 && itemModules.Count < 2)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    GameObject resultObj = results[i].gameObject;
                    if (!resultObj.GetComponent<ItemModel>()) continue;
                    ItemModel resultModule = resultObj.GetComponent<ItemModel>();
                    if (resultObj.layer == layerGround && !itemModules.Contains(resultModule))
                    {
                        itemModules.Add(resultModule);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

    }
}


