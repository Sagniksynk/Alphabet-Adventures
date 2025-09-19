using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Dialog : MonoBehaviour
{
	/// <summary>
	/// The animator of Dialog.
	/// </summary>
	public Animator animator;

	/// <summary>
	/// Wheter the Dialog is visible or not.
	/// </summary>
	[HideInInspector]
	public bool visible;

	/// <summary>
	/// The White Area animator.
	/// </summary>
	public Animator whiteAreaAnimator;

	void Start ()
	{
		if (animator == null) {
			animator = GetComponent<Animator> ();
		}

		if (whiteAreaAnimator == null) {
			whiteAreaAnimator = GameObject.Find ("WhiteArea").GetComponent<Animator> ();
		}
	}

	/// <summary>
	/// Show the dialog.
	/// </summary>
	public void Show ()
	{
		visible = true;
		whiteAreaAnimator.SetTrigger ("Running");
		animator.SetBool ("Off", false);
		animator.SetTrigger ("On");
	}

	/// <summary>
	/// Hide the dialog.
	/// </summary>
	public void Hide ()
	{
		visible = false;
		whiteAreaAnimator.SetBool ("Running", false);
		animator.SetBool ("On", false);
		animator.SetTrigger ("Off");
	}
}