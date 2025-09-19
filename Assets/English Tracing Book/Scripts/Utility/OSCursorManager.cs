using UnityEngine;
using System.Collections;


[DisallowMultipleComponent]
public class OSCursorManager : MonoBehaviour
{
	/// <summary>
	/// The status of the OS cursor.
	/// </summary>
	public CursorStatus status = CursorStatus.ENABLED;

	// Update is called once per frame
	void Start ()
	{
		#if (!(UNITY_ANDROID || UNITY_IPHONE) || UNITY_EDITOR)
			if (status == CursorStatus.ENABLED) {
				Cursor.visible = true;
			} else if (status == CursorStatus.DISABLED) {
				Cursor.visible = false;
			}
		#endif
	}

	public enum CursorStatus
	{
		ENABLED,
		DISABLED
	};
}
