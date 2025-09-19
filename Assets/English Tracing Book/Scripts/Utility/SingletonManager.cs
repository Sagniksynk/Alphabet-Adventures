using UnityEngine;
using System.Collections;

public class SingletonManager : MonoBehaviour {

	public GameObject[] values;

	// Use this for initialization
	void Awake () {
		InitManagers ();
	}

	private void InitManagers ()
	{
		if (values == null) {
			return;
		}

		foreach (GameObject value in values) {
			if (GameObject.Find (value.name) == null) {
				GameObject temp = Instantiate (value, Vector3.zero, Quaternion.identity) as GameObject;
				temp.name = value.name;
			}
		}
	}
}
