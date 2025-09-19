using UnityEngine;
using UnityEditor;
using System.Collections;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

[CustomEditor(typeof(ShapesManager))]
public class ShapesManagerEditor : Editor
{
		private Color greenColor = Color.green;
		private Color whiteColor = Color.white;
		private Color redColor = new Color (255, 0, 0, 255) / 255.0f;
		private static bool showInstructions = true;

		public override void OnInspectorGUI ()
		{
				ShapesManager shapesManager = (ShapesManager)target;//get the target

				EditorGUILayout.Separator ();

					
				EditorGUILayout.Separator ();
				EditorGUILayout.HelpBox ("Follow the instructions below on how to add new shape", MessageType.Info);
				EditorGUILayout.Separator ();

				showInstructions = EditorGUILayout.Foldout (showInstructions, "Instructions");
				EditorGUILayout.Separator ();

				if (showInstructions) {
					EditorGUILayout.HelpBox ("- Click on 'Add New Shape' button to add new Shape", MessageType.None);
					EditorGUILayout.HelpBox ("- Click on 'Remove Last Shape' button to remove the lastest shape in the list", MessageType.None);
					EditorGUILayout.HelpBox ("- Click on 'Apply' button that located at the top to save your changes ", MessageType.None);
				}

				EditorGUILayout.Separator ();
				
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Review English Tracing Book", GUILayout.Width (180), GUILayout.Height (25))) {
					Application.OpenURL (Links.packageURL);
				}

				GUI.backgroundColor = greenColor;         

				if (GUILayout.Button ("More Assets", GUILayout.Width (110), GUILayout.Height (25))) {
					Application.OpenURL (Links.indieStudioStoreURL);
				}
				GUI.backgroundColor = whiteColor;         

				GUILayout.EndHorizontal ();

				EditorGUILayout.Separator ();
				
				shapesManager.shapeLabel= EditorGUILayout.TextField ("Shape Label", shapesManager.shapeLabel);
				shapesManager.shapePrefix= EditorGUILayout.TextField ("Shape Prefix", shapesManager.shapePrefix);
				shapesManager.sceneName= EditorGUILayout.TextField ("Scene Name", shapesManager.sceneName);

				EditorGUILayout.Separator ();

				GUILayout.BeginHorizontal ();
				GUI.backgroundColor = greenColor;         

				if (GUILayout.Button ("Add New Shape", GUILayout.Width (110), GUILayout.Height (20))) {
						shapesManager.shapes.Add (new ShapesManager.Shape ());
				}

				GUI.backgroundColor = redColor;         
				if (GUILayout.Button ("Remove Last Shape", GUILayout.Width (150), GUILayout.Height (20))) {
						if (shapesManager.shapes.Count != 0) {
								shapesManager.shapes.RemoveAt (shapesManager.shapes.Count - 1);
						}
				}

				GUI.backgroundColor = whiteColor;
				GUILayout.EndHorizontal ();

				EditorGUILayout.Separator ();

				for (int i = 0; i <  shapesManager.shapes.Count; i++) {
						shapesManager.shapes [i].showContents = EditorGUILayout.Foldout (shapesManager.shapes [i].showContents, "Shape[" + i + "]");

						if (shapesManager.shapes [i].showContents) {
								EditorGUILayout.Separator ();
								shapesManager.shapes [i].gamePrefab = EditorGUILayout.ObjectField ("Game Prefab", shapesManager.shapes [i].gamePrefab, typeof(GameObject), true) as GameObject;
								shapesManager.shapes [i].picture =EditorGUILayout.ObjectField ("Picture", shapesManager.shapes [i].picture, typeof(Sprite), true) as Sprite;
								EditorGUILayout.Separator ();
								GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (2));
						}
				}

				if (GUI.changed) {
						DirtyUtil.MarkSceneDirty ();
				}
		}
}