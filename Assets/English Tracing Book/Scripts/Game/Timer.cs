using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Timer : MonoBehaviour
{
		/// <summary>
		/// Text Component
		/// </summary>
		public Text uiText;

		/// <summary>
		/// The time in seconds.
		/// </summary>
		[HideInInspector]
		public int
				timeInSeconds;

		/// <summary>
		/// The progress reference.
		/// </summary>
		public Progress progress;

		/// <summary>
		/// Whether the Timer is running
		/// </summary>
		private bool isRunning;

		/// <summary>
		/// The time counter.
		/// </summary>
		private float timeCounter;

		/// <summary>
		/// The sleep time.
		/// </summary>
		private float sleepTime;


		void Awake ()
		{
				if (uiText == null) {
						uiText = GetComponent<Text> ();
				}
				///Start the Timer
				Start ();
		}

		/// <summary>
		/// Start the Timer.
		/// </summary>
		public void Start ()
		{
				if (!isRunning) {
						timeCounter = 0;
						sleepTime = 0.01f;
						isRunning = true;
						timeInSeconds = 0;
						InvokeRepeating("Wait",0,sleepTime);
				}
		}

		/// <summary>
		/// Stop the Timer.
		/// </summary>
		public void Stop ()
		{
				if (isRunning) {
						isRunning = false;
						CancelInvoke();
				}
		}

		/// <summary>
		/// Reset the timer.
		/// </summary>
		public void Reset ()
		{
				Stop ();
				Start ();
		}

		/// <summary>
		/// Wait.
		/// </summary>
		private void Wait ()
		{
				timeCounter += sleepTime;
				timeInSeconds = (int)timeCounter;
				ApplyTime ();
				if (progress != null)
					progress.SetProgress (timeCounter);

		}

		/// <summary>
		/// Applies the time into TextMesh Component.
		/// </summary>
		private void ApplyTime ()
		{
				if (uiText == null) {
						return;
				}
				//	int mins = timeInSeconds / 60;
				//	int seconds = timeInSeconds % 60;

				//	uiText.text = "Time : " + GetNumberWithZeroFormat (mins) + ":" + GetNumberWithZeroFormat (seconds);
				uiText.text = timeInSeconds.ToString ();
		}

		/// <summary>
		/// Get the number with zero format.
		/// </summary>
		/// <returns>The number with zero format.</returns>
		/// <param name="number">Ineger Number.</param>
		public static string GetNumberWithZeroFormat (int number)
		{
				string strNumber = "";
				if (number < 10) {
						strNumber += "0";
				}
				strNumber += number;
		
				return strNumber;
		}
}