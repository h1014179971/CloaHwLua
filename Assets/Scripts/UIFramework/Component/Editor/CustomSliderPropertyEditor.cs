using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(CustomSlider),true)]
[CanEditMultipleObjects]
public class CustomSliderPropertyEditor : SliderEditor
{
    private SerializedProperty enableMaxValue;

    protected override void OnEnable()
    {
        base.OnEnable();
        enableMaxValue = serializedObject.FindProperty("enableMaxValue");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();
        EditorGUILayout.PropertyField(enableMaxValue);
        serializedObject.ApplyModifiedProperties();
    }
    
}
