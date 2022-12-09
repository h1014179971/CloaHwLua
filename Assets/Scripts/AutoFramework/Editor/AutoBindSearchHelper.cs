using System.Linq;
using System.Text;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

namespace AutoCode
{
    public class AutoBindSearchHelper
    {
        private static List<Type> ExportComponentTypes = new List<Type>() {typeof(Button),typeof(InputField),typeof(Dropdown),typeof(Toggle),typeof(Slider),
        typeof(ScrollRect),typeof(Scrollbar)};
        private static List<Type> ExportPropertyTypes = new List<Type>() { typeof(Image), typeof(RawImage), typeof(Text), typeof(RectTransform), typeof(Transform) };
        public static void Search(AutoCodeGenTask task)
        {
            ProcessUIPrefab(task.GameObject, task);
        }
        private static void ProcessUIPrefab(GameObject gameObject, AutoCodeGenTask task)
        {
            if (null == gameObject) return;
            foreach (Transform transform in gameObject.transform)
            {
                if (transform.CompareTag("UIIgnore"))
                {
                    continue;
                }
                ProcessUIPrefab(transform.gameObject,task);


                bool isHandled = false;
                foreach (var type in ExportComponentTypes)
                {
                    var UIComp = transform.GetComponent(type);
                    if (null != UIComp)
                    {
                        task.ComponentTypes.Add(new UIComponentInfo()
                        {
                            TypeName = GetDefaultComponentName(UIComp.gameObject),
                            MemberName = UIComp.gameObject.name,
                            obj = UIComp.gameObject
                        }); 
                        isHandled = true;
                        break;
                    }
                }
                if (isHandled) continue;
                foreach (var type in ExportPropertyTypes)
                {
                    var UIComp = transform.GetComponent(type);
                    if (null != UIComp && transform.CompareTag("UIProperty"))
                    {
                        task.ComponentTypes.Add(new UIComponentInfo()
                        {
                            TypeName = GetDefaultComponentName(UIComp.gameObject),
                            MemberName = UIComp.gameObject.name,
                            obj = UIComp.gameObject,
                            PathToRoot = PathToParent(UIComp.transform, task.GameObject.name)
                        });
                        isHandled = true;
                        break;
                    }
                }
            }
        }
        private static string GetDefaultComponentName(GameObject obj)
        {
            if (obj.GetComponent("SkeletonAnimation")) return "SkeletonAnimation";
            if (obj.GetComponent<ScrollRect>()) return "UnityEngine.UI.ScrollRect";
            if (obj.GetComponent<InputField>()) return "UnityEngine.UI.InputField";

            // text mesh pro supported
            if (obj.GetComponent("TMP.TextMeshProUGUI")) return "TMP.TextMeshProUGUI";
            if (obj.GetComponent("TMPro.TextMeshProUGUI")) return "TMPro.TextMeshProUGUI";
            if (obj.GetComponent("TMPro.TextMeshPro")) return "TMPro.TextMeshPro";
            if (obj.GetComponent("TMPro.TMP_InputField")) return "TMPro.TMP_InputField";

            // ugui bind
            if (obj.GetComponent<Dropdown>()) return "UnityEngine.UI.Dropdown";
            if (obj.GetComponent<Button>()) return "UnityEngine.UI.Button";
            if (obj.GetComponent<Text>()) return "UnityEngine.UI.Text";
            if (obj.GetComponent<RawImage>()) return "UnityEngine.UI.RawImage";
            if (obj.GetComponent<Toggle>()) return "UnityEngine.UI.Toggle";
            if (obj.GetComponent<Slider>()) return "UnityEngine.UI.Slider";
            if (obj.GetComponent<Scrollbar>()) return "UnityEngine.UI.Scrollbar";
            if (obj.GetComponent<Image>()) return "UnityEngine.UI.Image";
            if (obj.GetComponent<ToggleGroup>()) return "UnityEngine.UI.ToggleGroup";

            // other
            if (obj.GetComponent<Rigidbody>()) return "Rigidbody";
            if (obj.GetComponent<Rigidbody2D>()) return "Rigidbody2D";

            if (obj.GetComponent<BoxCollider2D>()) return "BoxCollider2D";
            if (obj.GetComponent<BoxCollider>()) return "BoxCollider";
            if (obj.GetComponent<CircleCollider2D>()) return "CircleCollider2D";
            if (obj.GetComponent<SphereCollider>()) return "SphereCollider";
            if (obj.GetComponent<MeshCollider>()) return "MeshCollider";

            if (obj.GetComponent<Collider>()) return "Collider";
            if (obj.GetComponent<Collider2D>()) return "Collider2D";

            if (obj.GetComponent<Animator>()) return "Animator";
            if (obj.GetComponent<Canvas>()) return "Canvas";
            if (obj.GetComponent<Camera>()) return "Camera";
            if (obj.GetComponent("Empty4Raycast")) return "QFramework.Empty4Raycast";
            if (obj.GetComponent<RectTransform>()) return "RectTransform";
            if (obj.GetComponent<MeshRenderer>()) return "MeshRenderer";

            if (obj.GetComponent<SpriteRenderer>()) return "SpriteRenderer";
            return "Transform";
        }
        public static string PathToParent(Transform trans, string parentName)
        {
            var retValue = new StringBuilder(trans.name);

            while (trans.parent != null)
            {
                if (trans.parent.name.Equals(parentName))
                {
                    break;
                }

                retValue.AddPrefix("/").AddPrefix(trans.parent.name);

                trans = trans.parent;
            }

            return retValue.ToString();
        }
    }
}

