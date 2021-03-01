using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TextureCheckEditorWindow : EditorWindow
{
	public static List<string> imageList = new List<string> ();
	public static List<string> errorList = new List<string> ();

	public Vector2 scrollPosition = new Vector2 (0, 0);

	//GameObject gameObject;
	//Editor gameObjectEditor;

	[MenuItem ("Window/GameObject Editor")]
	public static void ShowWindow ()
	{
		if (imageList.Count != 0)
			GetWindow<TextureCheckEditorWindow> ("GameObject Editor");

	}

	void OnGUI ()
	{

		scrollPosition = GUILayout.BeginScrollView (scrollPosition, GUILayout.Width (666), GUILayout.Height (555));
		for (int i = 0; i < imageList.Count; i++) {

			if (GUILayout.Button (new GUIContent (imageList [i] + "," + errorList [i], AssetDatabase.GetCachedIcon (imageList [i])))) {
				var asset = AssetDatabase.LoadAssetAtPath<Object> (imageList [i]);
				Selection.activeObject = asset;
			}
		}
		GUILayout.EndScrollView ();
	}
}