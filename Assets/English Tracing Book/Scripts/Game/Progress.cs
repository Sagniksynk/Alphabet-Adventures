using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{
		/// <summary>
		/// The star off sprite.
		/// </summary>
		public Sprite starOff;
		
		/// <summary>
		/// The star on sprite.
		/// </summary>
		public Sprite starOn;

		/// <summary>
		/// The level stars.
		/// </summary>
		public Image[] levelStars;

		/// <summary>
		/// The game manager reference.
		/// </summary>
		public GameManager gameManager;

		/// <summary>
		/// The progress image.
		/// </summary>
		public Image progressImage;

		/// <summary>
		/// The stars number.
		/// </summary>
		[HideInInspector]
		public WinDialog.StarsNumber starsNumber;

		// Use this for initialization
		void Start ()
		{
				if (progressImage == null) {
						progressImage = GetComponent<Image> ();
				}

				if (gameManager == null) {
						gameManager = GameObject.FindObjectOfType<GameManager> ();
				}
		}

		/// <summary>
		/// Set the value of the progress.
		/// </summary>
		/// <param name="currentTime">Current time.</param>
		public void SetProgress (float currentTime)
		{
				if (gameManager == null) {
						return;
				}

				if (gameManager.shape == null) {
						return;
				}

				if (progressImage != null)
						progressImage.fillAmount = 1 - (currentTime / (gameManager.shape.twoStarsTimePeriod * 1.0f + 1));
			
				if (currentTime >= 0 && currentTime <= gameManager.shape.threeStarsTimePeriod) {
						if (levelStars [0] != null) {
								levelStars [0].sprite = starOn;
						}
						if (levelStars [1] != null) {
								levelStars [1].sprite = starOn;
						}
						if (levelStars [2] != null) {
								levelStars [2].sprite = starOn;
						}
						if (progressImage != null)
								progressImage.color = Colors.greenColor;

						starsNumber = WinDialog.StarsNumber.THREE;
				} else if (currentTime > gameManager.shape.threeStarsTimePeriod && currentTime <= gameManager.shape.twoStarsTimePeriod) {
						if (levelStars [2] != null) {
								levelStars [2].sprite = starOff;
						}
						if (progressImage != null)
								progressImage.color = Colors.yellowColor;
						starsNumber = WinDialog.StarsNumber.TWO;

				} else {
						if (levelStars [1] != null) {
								levelStars [1].sprite = starOff;
						}
						if (levelStars [2] != null) {
								levelStars [2].sprite = starOff;
						}
						if (progressImage != null)
								progressImage.color = Colors.redColor;
						starsNumber = WinDialog.StarsNumber.ONE;
				}
		}

}
