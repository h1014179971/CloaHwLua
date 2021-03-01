using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputWrapper : Singleton<InputWrapper>
{
    public bool IsTouchScreen()
    {
#if UNITY_EDITOR
        return Input.GetMouseButton(0);
#else
        return (Input.touchCount == 1);
#endif
    }

    public Vector3 GetCursorPosition()
    {
#if UNITY_EDITOR
        return Input.mousePosition;
#else
        Vector3 retPos = Vector3.zero;
        if (Input.touchCount > 0)
        {
            Vector2 pos = Input.GetTouch(0).position;
            retPos = new Vector3(pos.x, pos.y, 0f);
        }
        return retPos;
#endif
    }


    // 中心点的屏幕坐标，overlay模式就是position,否则是世界坐标,需要worldtoscreen进行转换
    private Vector3 GetSpacePos(RectTransform rect, Canvas canvas, Camera camera)
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return rect.position;
        }
        return camera.WorldToScreenPoint(rect.position);
    }

    // 返回四个角的世界坐标,对应的屏幕坐标依然和渲染模式有关
    private void GetSpaceCorners(RectTransform rect, Canvas canvas, Vector3[] corners, Camera camera)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        rect.GetWorldCorners(corners);
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
        }
        else
        {
            for (var i = 0; i < corners.Length; i++)
            {
                corners[i] = camera.WorldToScreenPoint(corners[i]);
            }
        }
    }

    // 获取鼠标点下图片的像素
    public Rect GetSpaceRect(Canvas canvas, RectTransform rect, Camera camera)
    {
        Rect spaceRect = rect.rect;
        Vector3 spacePos = GetSpacePos(rect, canvas, camera);
        //lossyScale
        spaceRect.x = spaceRect.x * rect.lossyScale.x + spacePos.x;
        spaceRect.y = spaceRect.y * rect.lossyScale.y + spacePos.y;
        spaceRect.width = spaceRect.width * rect.lossyScale.x;
        spaceRect.height = spaceRect.height * rect.lossyScale.y;
        return spaceRect;
    }

    public bool RectContainsScreenPoint(Vector3 point, Canvas canvas, RectTransform rect, Camera camera)
    {
        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(rect, point, camera);
        }
        return GetSpaceRect(canvas, rect, camera).Contains(point);
    }
}
