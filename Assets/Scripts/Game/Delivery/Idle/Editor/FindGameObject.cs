using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Delivery;

public class FindGameObject 
{
    static GameObject[] SelectionObjs;
    [MenuItem("Tools/Idle/建筑层级改为Mid")]
    static void FindReference()
    {
        Debug.Log($"Selection.gameObjects==={Selection.gameObjects.Length}"); 
        for(int i = 0; i < Selection.gameObjects.Length; i++)
        {
            Transform transform = Selection.gameObjects[i].transform;
            SpriteRenderer[] sprites = transform.GetComponentsInChildren<SpriteRenderer>();
            for(int j =0;j<sprites.Length;j++)
            {
                SpriteRenderer spriteRenderer = sprites[j];
                spriteRenderer.sortingLayerName = SortLayers.mid;
                spriteRenderer.sortingOrder = -(int)(spriteRenderer.transform.position.y * 10);
                EditorUtility.SetDirty(sprites[j].gameObject);
            }
        }
        
    }
}
