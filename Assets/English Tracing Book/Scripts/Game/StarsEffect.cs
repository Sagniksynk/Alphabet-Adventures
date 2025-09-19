using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class StarsEffect : MonoBehaviour
{
		/// <summary>
		/// The position of Stars Effect in the World Space.
		/// </summary>
		private Vector3 tempPosition;

		/// <summary>
		/// The stars effect prefab.
		/// </summary>
		public GameObject starsEffectPrefab;

		/// <summary>
		/// The star effect Z position.
		/// </summary>
		[Range(-50,50)]
		public float starEffectZPosition = -5;

		/// <summary>
		/// The stars effect parent.
		/// </summary>
		public Transform starsEffectParent;

		/// <summary>
		/// Create the stars effect.
		/// </summary>
		public void CreateStarsEffect ()
		{
				if (starsEffectPrefab == null) {
					return;
				}
				tempPosition = transform.position;
				tempPosition.z = starEffectZPosition;
				GameObject starsEffect = Instantiate (starsEffectPrefab, tempPosition, Quaternion.identity) as GameObject;
				if(starsEffectParent!=null)
					starsEffect.transform.parent = starsEffectParent;//setting up Stars Effect Parent
				starsEffect.transform.eulerAngles = new Vector3(0,180,0);
		}
}