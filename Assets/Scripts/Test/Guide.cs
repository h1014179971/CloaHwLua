using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 新手引导动画
/// </summary>
public class Guide : MonoBehaviour
{


	public Image target;

	public bool isCircle;
	private Vector4 center;
	private Material material;
	private float diameter; // 直径
	private float current = 0f;

	Vector3[] corners = new Vector3[4];

	//矩形
	//UI Rect宽度的一半
	private float offsetX = 0f;
	//UI Rect高度的一半
	private float offsetY = 0f;
	//当前遮挡宽度
	private float currentX = 0f;
	//当前遮挡高度
	private float currentY = 0f;
	private float offset = 10;//上下偏移量
	void Awake()
	{

		Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		target.rectTransform.GetWorldCorners(corners);
		

		float x = corners[0].x + ((corners[3].x - corners[0].x) / 2f);
		float y = corners[0].y + ((corners[1].y - corners[0].y) / 2f);

		Vector3 center = new Vector3(x, y, 0f);
		Vector2 position = Vector2.zero;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, center, canvas.GetComponent<Camera>(), out position);




		center = new Vector4(position.x, position.y, 0f, 0f);
		material = GetComponent<Image>().material;
		material.SetVector("_Center", center);
		
		
		if (isCircle)
        {
			material.SetFloat("_IsCircle", 1);
			diameter = Vector2.Distance(WordToCanvasPos(canvas, corners[0]), WordToCanvasPos(canvas, corners[2])) / 2f;
			(canvas.transform as RectTransform).GetWorldCorners(corners);
			for (int i = 0; i < corners.Length; i++)
			{
				current = Mathf.Max(Vector3.Distance(WordToCanvasPos(canvas, corners[i]), center), current);
			}
			material.SetFloat("_Silder", current);
		}
        else
        {
			material.SetFloat("_IsCircle", 0);
			offsetX = Vector2.Distance(WordToCanvasPos(canvas, corners[0]), WordToCanvasPos(canvas, corners[3])) / 2f + offset*2;
			offsetY = Vector2.Distance(WordToCanvasPos(canvas, corners[0]), WordToCanvasPos(canvas, corners[1])) / 2f + offset*2;
			(canvas.transform as RectTransform).GetWorldCorners(corners);

			for (int i = 0; i < corners.Length; i++)
			{
				if (i % 2 == 0)
				{
					currentX = Mathf.Max(Vector3.Distance(WordToCanvasPos(canvas, corners[i]), center), currentX) + offset*2;
				}
				else
				{
					currentY = Mathf.Max(Vector3.Distance(WordToCanvasPos(canvas, corners[i]), center), currentY) + offset*2;
				}
			}

			material.SetFloat("_SliderX", currentX);
			material.SetFloat("_SliderY", currentY);
		}


		
	}

	float xVelocity = 0f;
	float yVelocity = 0f;
	void Update()
	{

        if (isCircle)
        {
			float value = Mathf.SmoothDamp(current, diameter, ref yVelocity, 0.3f);
			if (!Mathf.Approximately(value, current))
			{
				current = value;
				material.SetFloat("_Silder", current);
			}
		}
        else
        {
			float valueX = Mathf.SmoothDamp(currentX, offsetX, ref xVelocity, 0.3f);
			float valueY = Mathf.SmoothDamp(currentY, offsetY, ref yVelocity, 0.3f);
			if (!Mathf.Approximately(valueX, currentX))
			{
				currentX = valueX;
				material.SetFloat("_SliderX", currentX);
			}
			if (!Mathf.Approximately(valueY, currentY))
			{
				currentY = valueY;
				material.SetFloat("_SliderY", currentY);
			}
		}
		
	}

	void OnGUI()
	{
		if (GUILayout.Button("Test"))
		{
			Awake();
		}
	}


	Vector2 WordToCanvasPos(Canvas canvas, Vector3 world)
	{
		Vector2 position = Vector2.zero;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, world, canvas.GetComponent<Camera>(), out position);
		return position;
	}
}