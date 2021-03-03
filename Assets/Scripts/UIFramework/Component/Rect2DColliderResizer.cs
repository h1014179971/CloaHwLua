using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rect2DColliderResizer : MonoBehaviour {
    private void Start()
    {
        ResizeCollider();
    }
    public void ResizeCollider()
    {
        RectTransform rt = this.GetComponent<RectTransform>();
        Vector2 p = new Vector2(0.5f, 0.5f);
        Vector2 pivot = rt.pivot;
        Vector2 tp = p - pivot;
        BoxCollider2D c = this.GetComponent<BoxCollider2D>();
        c.size = rt.rect.size;
        //Debug.Log(rt.sizeDelta);
        c.offset = new Vector2(tp.x * c.size.x, tp.y * c.size.y);
    }

    private void OnRectTransformDimensionsChange()
    {
        ResizeCollider();
    }

}
