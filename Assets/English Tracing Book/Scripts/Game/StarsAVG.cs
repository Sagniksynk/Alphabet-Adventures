using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class StarsAVG : MonoBehaviour {

	public Image[] stars;
	public string shapesManagerReference;
	public Sprite starOn, starOff;

	IEnumerator Start () {
		
		ShapesManager shapesManager = null;

		// This loop will wait patiently until the ShapesManager is found, preventing crashes.
		while (shapesManager == null)
		{
			GameObject managerObject = GameObject.Find(shapesManagerReference);
			if (managerObject != null)
			{
				shapesManager = managerObject.GetComponent<ShapesManager>();
			}

			// If still not found, wait for the next frame before trying again.
			if (shapesManager == null)
			{
				yield return null;
			}
		}

		// Now we can be sure shapesManager is not null.
		int collectedStars = DataManager.GetCollectedStars (shapesManager);
		int starsRate = Mathf.FloorToInt(collectedStars / (shapesManager.shapes.Count * 3.0f) * 3.0f);
	
		if (starsRate == 0) {
			stars [0].sprite = starOff;
			stars [1].sprite = starOff;
			stars [2].sprite = starOff;
		} else if (starsRate == 1) {
			stars [0].sprite = starOn;
			stars [1].sprite = starOff;
			stars [2].sprite = starOff;
		} else if (starsRate == 2) {
			stars [0].sprite = starOn;
			stars [1].sprite = starOn;
			stars [2].sprite = starOff;
		} else {
			stars [0].sprite = starOn;
			stars [1].sprite = starOn;
			stars [2].sprite = starOn;
		}
	}
}