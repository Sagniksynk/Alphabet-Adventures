using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pointer : MonoBehaviour
{
		public Group group;//the group reference
		
		/// <summary>
		/// Create a pointer.
		/// </summary>
		/// <param name="groupIndex">Group index.</param>
		/// <param name="levelsGroup">Levels group.</param>
		/// <param name="pointerPrefab">Pointer prefab.</param>
		/// <param name="pointersParent">Pointers parent.</param>
		public static void CreatePointer (int groupIndex, GameObject levelsGroup, GameObject pointerPrefab, Transform pointersParent)
		{
				if (levelsGroup == null || pointerPrefab == null || pointersParent == null) {
						return;
				}

				//Create Slider Pointer
				GameObject pointer = Instantiate (pointerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				pointer.transform.SetParent (pointersParent);
				pointer.name = "Pointer-" + CommonUtil.IntToString(groupIndex + 1);
				pointer.transform.localScale = Vector3.one;
				pointer.GetComponent<RectTransform> ().offsetMax = Vector2.zero;
				pointer.GetComponent<RectTransform> ().offsetMin = Vector2.zero;
				pointer.GetComponent<Pointer> ().group = levelsGroup.GetComponent<Group> ();
				pointer.GetComponent<Button> ().onClick.AddListener (() => GameObject.FindObjectOfType<UIEvents> ().PointerButtonEvent (pointer.GetComponent<Pointer> ()));
		}
}
