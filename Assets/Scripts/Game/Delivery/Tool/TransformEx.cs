using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;

/// <summary>
/// Transform扩展方法，获取新手引导所需的路径配置
/// </summary>
public static class TransformEx 
{
   public static string GetParentRoute(this Transform transform)
    {
        string route = "";
        Transform parent = transform.parent;
        while (parent!=null)
        {
            if (parent.GetComponent<Canvas>() != null) return route;//父节点是画布时退出
            if (parent.GetComponent<ScrollRectItem>()!=null)
            {
                route = "";
                parent = parent.parent;
                continue;
            }
            if(string.IsNullOrEmpty(route))
                route = parent.name;
            else
            {
                route = $"{parent.name}/{route}";
            }
            parent = parent.parent;
        }
        return route;
    }

    public static string GetTargetRoute(this Transform transform)
    {
        string route = transform.name;
        Transform parent = transform.parent;
        string parentRoute = route;
        while(parent!=null)
        {
            if (parent.GetComponent<Canvas>() != null) return route;//父节点是画布时退出
            if (parent.GetComponent<ScrollRectItem>() != null)
            {
                return parentRoute;
            }
            parentRoute = $"{parent.name}/{parentRoute}";
            parent = parent.parent;
        }
        return route;
    }

    public static string GetFollowRoute(this Transform transform)
    {
        string route = transform.name;
        Transform parent = transform.parent;
        while(parent!=null)
        {
            route = $"{parent.name}/{route}";
            parent = parent.parent;
        }
        return route;
    }
}
