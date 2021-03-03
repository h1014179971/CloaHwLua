using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class GetGameObjectRoute
{
    [MenuItem("GameObject/GetParentRoute", priority = 0)]
    public static void GetParentRoute()
    {
        Transform transform = Selection.activeTransform;
        string route = transform.GetParentRoute();
        GUIUtility.systemCopyBuffer = route;//将内容复制到系统粘贴板
        Debug.Log(route);
    }
    [MenuItem("GameObject/GetTargetRoute",priority =0)]
    public static void GetTargetRoute()
    {
        Transform transform = Selection.activeTransform;
        string route = transform.GetTargetRoute();
        GUIUtility.systemCopyBuffer = route;//将内容复制到系统粘贴板
        Debug.Log(route);
    }
    [MenuItem("GameObject/GetFollowRoute",priority =0)]
    public static void GetFollowRoute()
    {
        Transform transform = Selection.activeTransform;
        string route = transform.GetFollowRoute();
        GUIUtility.systemCopyBuffer = route;
        Debug.Log(route);
    }

    [MenuItem("GameObject/CopySpriteColor",priority =0)]
    public static void CopySpriteColor()
    {
        Transform transform = Selection.activeTransform;
        SpriteRenderer sprite = transform.GetComponent<SpriteRenderer>();
        if(sprite==null)
        {
            Debug.Log("target dont exist component spriterenderer");
        }
        string color = ColorUtility.ToHtmlStringRGB(sprite.color);
        GUIUtility.systemCopyBuffer = "#" + color;
        Debug.Log($"success to copy color={color}");
    }

    [MenuItem("GameObject/SetSpriteColor",priority =0)]
    public static void SetSpriteColor()
    {
        string copyBuffer = GUIUtility.systemCopyBuffer;
        Color targetColor = Color.white;
        if(!ColorUtility.TryParseHtmlString(copyBuffer, out targetColor))
        {
            Debug.Log("没有选择需要设置的颜色");
        }
        Transform spritesNode = Selection.activeTransform;
        SpriteRenderer[] sprites = spritesNode.GetComponentsInChildren<SpriteRenderer>();
        for (int i=0;i< sprites.Length;i++)
        {
            sprites[i].color = targetColor;
        }
       
    }

  
}
