using UnityEngine;
using UnityEditor;

public class RulerWindow : EditorWindow
{

    static RulerWindow _instance;
    [MenuItem("Tools/RulerWindow")]
    static void RulerWindowOpen()
    {
        if (_instance == null)
        {
            _instance = EditorWindow.CreateInstance<RulerWindow>();
        }
        _instance.Show();
    }
    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneFunc;
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneFunc;
    }
    void OnGUI()
    {
        GUILayout.Label("辅助线颜色");
        line_color = EditorGUILayout.ColorField(line_color);
        GUILayout.Label("辅助线速度");
        ruler_sp = EditorGUILayout.Slider(ruler_sp, 0f, 0.1f);
        GUILayout.Label("辅助线X");
        ruler_x = EditorGUILayout.FloatField(ruler_x);
        GUILayout.Label("辅助线Y");
        ruler_y = EditorGUILayout.FloatField(ruler_y);
        GUILayout.Label("左右线条数量（一半）");
        count_x = EditorGUILayout.IntField(count_x);
        GUILayout.Label("上下线条数量（一半）");
        count_y = EditorGUILayout.IntField(count_y);
        GUILayout.Label("格子宽度，米：像素(1:100)");
        gridWidth = EditorGUILayout.FloatField(gridWidth);
    }
    private int count_x = 20;//以中心线隔开左右各（count_x）条线
    private int count_y = 20;//以中心线隔开上下各（count_y）条线
    private float gridWidth = 1;//格子宽度
    private float ruler_x;
    private float ruler_y;
    private float ruler_sp = 0.05f;
    const int MAXVALUE = 10000;
    private Color line_color = Color.blue;

    void OnSceneFunc(SceneView sceneView)
    {
        Event e = Event.current;
        if (e.isKey)
        {
            if (e.keyCode == KeyCode.A)
            {
                ruler_x -= ruler_sp;
            }
            else if (e.keyCode == KeyCode.W)
            {
                ruler_y += ruler_sp;
            }
            else if (e.keyCode == KeyCode.S)
            {
                ruler_y -= ruler_sp;
            }
            else if (e.keyCode == KeyCode.D)
            {
                ruler_x += ruler_sp;
            }
        }
        Handles.color = Color.red;
        Vector3 pos = sceneView.camera.WorldToViewportPoint(new Vector3(ruler_x, ruler_y, 0));
        var rect = sceneView.position;
        float width = rect.width;
        float height = rect.height;
        float x_percent = Mathf.Clamp01(pos.x);
        float y_percent = Mathf.Clamp01(1 - pos.y);

        // horizontal line
        Handles.DrawLine(new Vector3(-MAXVALUE, ruler_y, 0), new Vector3(MAXVALUE, ruler_y, 0));
        EditorGUIUtility.AddCursorRect(new Rect(0, height * y_percent - 10, width, 20), MouseCursor.ResizeVertical);
        // vertical line
        Handles.DrawLine(new Vector3(ruler_x, -MAXVALUE, 0), new Vector3(ruler_x, MAXVALUE, 0));
        EditorGUIUtility.AddCursorRect(new Rect(width * x_percent - 10, 0, 20, height), MouseCursor.ResizeHorizontal);
        Handles.color = Color.white;

        Handles.color = line_color;
        // horizontal line
        for (int i = -count_x; i <= count_x; i++)
        {
            if (i == 0) continue;
            Handles.DrawLine(new Vector3(-MAXVALUE, ruler_y + i * gridWidth, 0), new Vector3(MAXVALUE, ruler_y + i * gridWidth, 0));
            EditorGUIUtility.AddCursorRect(new Rect(0, height * y_percent - 10, width, 20), MouseCursor.ResizeVertical);
        }
        // vertical line
        for (int i = -count_y; i <= count_y; i++)
        {
            if (i == 0) continue;
            Handles.DrawLine(new Vector3(ruler_x + i * gridWidth, -MAXVALUE, 0), new Vector3(ruler_x + i * gridWidth, MAXVALUE, 0));
            EditorGUIUtility.AddCursorRect(new Rect(width * x_percent - 10, 0, 20, height), MouseCursor.ResizeHorizontal);
        }
        Handles.color = Color.white;
        // Debug.Log(sceneView.camera.ScreenToWorldPoint(new Vector3(e.mousePosition.x, e.mousePosition.y, 10f)));
        // Vector3 pos = sceneView.camera.transform.position;
        // Debug.Log("m_pos:" + e.mousePosition);
        // Debug.LogFormat("x:{0},y:{1},z:{2}",pos.x,pos.y,pos.z);
        //鼠标位置进行3D的转换
        // GameObject.Find("Cube").transform.position = worldPos;

        if (e.control)
        {
            Vector3 mousePos = e.mousePosition;
            float viewpos_x = mousePos.x / width;
            float viewpos_y = 1 - mousePos.y / height;
            Vector3 worldPos = sceneView.camera.ViewportToWorldPoint(new Vector3(viewpos_x, viewpos_y, 0));
            if (e.type == EventType.MouseDown)
            {
                if (Mathf.Abs(worldPos.x - ruler_x) < 1f)
                {
                    moveX = true;
                    moveY = false;
                }
                else if (Mathf.Abs(worldPos.y - ruler_y) < 1f)
                {
                    moveX = false;
                    moveY = true;
                }
            }
            if (e.type == EventType.Layout)
            {
                if (moveX)
                {
                    ruler_x = worldPos.x;
                }
                if (moveY)
                {
                    ruler_y = worldPos.y;
                }
            }
        }
        else
        {
            moveX = false;
            moveY = false;
        }
    }

    bool moveX = false;
    bool moveY = false;

}