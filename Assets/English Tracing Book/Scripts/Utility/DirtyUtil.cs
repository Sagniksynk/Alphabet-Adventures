using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DirtyUtil
{
		public static void MarkSceneDirty ()
		{
			#if UNITY_5 && UNITY_EDITOR
				if(!EditorApplication.isSceneDirty){
					EditorApplication.MarkSceneDirty(); 
				}
			#endif
		}
}