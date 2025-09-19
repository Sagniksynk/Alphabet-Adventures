using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class Pencil : MonoBehaviour
{
	/// <summary>
	/// The color of the pencil.
	/// </summary>
	public Color value;

	void Start(){
		GetComponent<Button> ().onClick.AddListener (() => GameObject.FindObjectOfType<UIEvents> ().PencilClickEvent (this));
	}

	/// <summary>
	/// Enable pencil selection.
	/// </summary>
	public void EnableSelection(){
		GetComponent<Animator>().SetBool("RunScale",true);
	}

	/// <summary>
	/// Disable pencil selection.
	/// </summary>
	public void DisableSelection(){
		GetComponent<Animator>().SetBool("RunScale",false);
	}
}
