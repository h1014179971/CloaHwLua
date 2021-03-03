using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResizePanel : MonoBehaviour, IPointerDownHandler, IDragHandler {
	
	public Vector2 minSize = new Vector2 (100, 100);
	public Vector2 maxSize = new Vector2 (400, 400);

    private RectTransform parentRectTransform;
	private RectTransform panelRectTransform;
	private Vector2 originalLocalPointerPosition;
	private Vector2 originalSizeDelta;
	
    void Awake () {
        panelRectTransform = transform.parent.GetComponent<RectTransform> ();
        parentRectTransform = panelRectTransform.parent as RectTransform;
	}
	
	public void OnPointerDown (PointerEventData data) {
		originalSizeDelta = panelRectTransform.sizeDelta;
		RectTransformUtility.ScreenPointToLocalPointInRectangle (panelRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);
	}
	
	public void OnDrag (PointerEventData data) {
		if (panelRectTransform == null)
			return;
		
		Vector2 localPointerPosition;
		RectTransformUtility.ScreenPointToLocalPointInRectangle (panelRectTransform, data.position, data.pressEventCamera, out localPointerPosition);
		Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
		
		Vector2 sizeDelta = originalSizeDelta + new Vector2 (offsetToOriginal.x, -offsetToOriginal.y);
        float maxX = Mathf.Min(maxSize.x, parentRectTransform.rect.width - panelRectTransform.anchoredPosition.x);
        float maxY = Mathf.Min(maxSize.y, parentRectTransform.rect.height + panelRectTransform.anchoredPosition.y);
		sizeDelta = new Vector2 (
                              Mathf.Clamp (sizeDelta.x, minSize.x, maxX),
                              Mathf.Clamp (sizeDelta.y, minSize.y, maxY)
		);
		
		panelRectTransform.sizeDelta = sizeDelta;
    }
}