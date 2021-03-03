using UnityEngine;

public class FPS : MonoBehaviour
{
	private float m_LastUpdateShowTime = 0f;  //上一次更新帧率的时间;

    public float m_UpdateShowDeltaTime = 0.1f;//更新帧率的时间间隔;

	private int m_FrameUpdate = 0;//帧数;

	private float m_FPS = 0;
    
	// Use this for initialization
	void Start()
	{
		m_LastUpdateShowTime = Time.realtimeSinceStartup;
	}

	// Update is called once per frame
	void Update()
	{
		m_FrameUpdate++;
		if (Time.realtimeSinceStartup - m_LastUpdateShowTime >= m_UpdateShowDeltaTime)
		{
			m_FPS = m_FrameUpdate / (Time.realtimeSinceStartup - m_LastUpdateShowTime);
			m_FrameUpdate = 0;
			m_LastUpdateShowTime = Time.realtimeSinceStartup;
		}
	}

    Rect _rect = new Rect(0, 0, Screen.width, 200);
	void OnGUI()
	{
		GUI.skin.label.fontSize = 50;
        GUI.color = Color.red;
        GUI.skin.label.alignment = TextAnchor.UpperRight;
        GUI.Label(_rect, m_FPS.ToString("0.00"));
	}
}
