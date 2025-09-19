using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ShapesManager : MonoBehaviour
{
	/// <summary>
	/// The shapes list.
	/// </summary>
	public List<Shape> shapes = new List<Shape> ();

	/// <summary>
	/// The shape label (example Letter or Number).
	/// </summary>
	public string shapeLabel = "Shape";

	/// <summary>
	/// The shape prefix used for DataManager only (example Lowercase or Uppercase or Number).
	/// </summary>
	public string shapePrefix = "Shape";

	/// <summary>
	/// The name of the scene.
	/// </summary>
	public string sceneName = "";
		
	/// <summary>
	/// The last selected group.
	/// </summary>
	[HideInInspector]
	public int lastSelectedGroup;

	/// <summary>
	/// The name of the shapes manager.
	/// </summary>
	public static string shapesManagerReference = "";
		
	/// <summary>
	/// The init shapes managers flags.
	/// </summary>
	public static Hashtable initFlags = new Hashtable ();

	void Awake ()
	{
		if (initFlags.Contains (gameObject.name)) {
			Destroy (gameObject);
		} else {
			initFlags.Add (gameObject.name, true);
			DontDestroyOnLoad (gameObject);
			lastSelectedGroup = 0;
		}
	}

	[System.Serializable]
	public class Shape
	{
		public bool showContents = true;
		public GameObject gamePrefab;
		public Sprite picture;
	}
}
