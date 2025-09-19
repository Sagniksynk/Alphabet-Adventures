using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompoundShape : MonoBehaviour {

	/// <summary>
	/// The shapes list.
	/// </summary>
	public List<Shape> shapes = new List<Shape>();

	/// <summary>
	/// Whether to enable the shapes priority order or not.
	/// </summary>
	//public bool enablePriorityOrder = true;

	// Use this for initialization
	void Start () {
	}


	/// <summary>
	/// Get the index of the current shape.
	/// </summary>
	/// <returns>The current shape index.</returns>
	public int GetCurrentShapeIndex ()
	{
		int index = -1;
		for (int i = 0; i < shapes.Count; i++) {

			if (shapes [i].completed) {
				continue;
			}

			bool isCurrentPath = true;
			for (int j = 0; j < i; j++) {
				if (!shapes [j].completed) {
					isCurrentPath = false;
					break;
				}
			}

			if (isCurrentPath) {
				index = i;
				break;
			}
		}

		return index;
	}

	/// <summary>
	/// Gets the shape's index by instance ID.
	/// </summary>
	/// <returns>The shape index by instance Id.</returns>
	/// <param name="ID">Instance ID.</param>
	public int GetShapeIndexByInstanceID(int ID){
		for (int i = 0; i < shapes.Count;i++) {
			if (ID == shapes[i].GetInstanceID ()) {
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// Determine whether the compound shape is completed.
	/// </summary>
	/// <returns><c>true</c> if this instance is completed; otherwise, <c>false</c>.</returns>
	public bool IsCompleted(){
		bool completed = true;
		foreach (Shape shape in shapes) {
			if (!shape.completed) {
				completed = false;
				break;
			}
		}
		return completed;
	}

}
