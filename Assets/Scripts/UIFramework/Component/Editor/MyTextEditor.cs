using UnityEditor;
using UnityEditor.UI;

//指定我们要自定义编辑器的脚本 
[CustomEditor(typeof(MyText), true)]
//使用了 SerializedObject 和 SerializedProperty 系统，因此，可以自动处理“多对象编辑”，“撤销undo” 和 “预制覆盖prefab override”。
[CanEditMultipleObjects]
public class MyTextEditor : TextEditor
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    //并且特别注意，如果用这种序列化方式，需要在 OnInspectorGUI 开头和结尾各加一句 serializedObject.Update();  serializedObject.ApplyModifiedProperties();
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
