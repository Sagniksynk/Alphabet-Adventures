using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Required for modern scene loading


[DisallowMultipleComponent]
public class Logo : MonoBehaviour
{
	public float sleepTime = 5;
	public string nextSceneName;

	void Start ()
	{
		Invoke ("LoadScene", sleepTime);
	}

	private void LoadScene ()
	{
		if (string.IsNullOrEmpty (nextSceneName)) {
			return;
		}
		// MODIFIED: Replaced obsolete Application.LoadLevel
		SceneManager.LoadScene (nextSceneName);
	}
}