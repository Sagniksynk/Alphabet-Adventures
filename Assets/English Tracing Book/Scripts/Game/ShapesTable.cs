using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.

[DisallowMultipleComponent]
public class ShapesTable : MonoBehaviour
{
	/// <summary>
	/// Whether to create groups pointers or not.
	/// </summary>
	public bool createGroupsPointers = true;

	/// <summary>
	/// Whether to save the last selected group or not.
	/// </summary>
	public bool saveLastSelectedGroup = true;

	/// <summary>
	/// The shapes list.
	/// </summary>
	public static List<TableShape> shapes;

	/// <summary>
	/// The groups parent.
	/// </summary>
	public Transform groupsParent;

	/// <summary>
	/// The pointers parent.
	/// </summary>
	public Transform pointersParent;

	/// <summary>
	/// The collected stars text.
	/// </summary>
	public Text collectedStarsText;

	/// <summary>
	/// The shape bright.
	/// </summary>
	public Transform shapeBright;

	/// <summary>
	/// The star on sprite.
	/// </summary>
	public Sprite starOn;

	/// <summary>
	/// The star off sprite.
	/// </summary>
	public Sprite starOff;

	/// <summary>
	/// The shape prefab.
	/// </summary>
	public GameObject shapePrefab;
	
	/// <summary>
	/// The shapes group prefab.
	/// </summary>
	public GameObject shapesGroupPrefab;

	/// <summary>
	/// The pointer prefab.
	/// </summary>
	public GameObject pointerPrefab;

	/// <summary>
	/// temporary transform.
	/// </summary>
	private Transform tempTransform;

	public float shapeScaleFactor = 0.7f;

	/// <summary>
	/// The Number of shapes per group.
	/// </summary>
	[Range (1, 100)]
	public int shapesPerGroup = 12;

	/// <summary>
	/// Number of columns per group.
	/// </summary>
	[Range (1, 10)]
	public int columnsPerGroup = 3;

	/// <summary>
	/// Whether to enable group grid layout.
	/// </summary>
	public bool EnableGroupGridLayout = true;

	/// <summary>
	/// The last shape that user reached.
	/// </summary>
	private Transform lastShape;

	/// <summary>
	/// The collected star.
	/// </summary>
	private int collectedStars;

	/// <summary>
	/// The shapes manager.
	/// </summary>
	private ShapesManager shapesManager;

	void Awake ()
	{
		if (!string.IsNullOrEmpty (ShapesManager.shapesManagerReference)) {
			shapesManager = GameObject.Find (ShapesManager.shapesManagerReference).GetComponent<ShapesManager> ();
		} else {
			Debug.LogError ("You have to start the game from the Main scene");
		}

		collectedStars = 0;

		//define the shapes list
		shapes = new List<TableShape> ();

		//Create new shapes
		StartCoroutine("CreateShapes");
	
		//Setup the last selected group index
		/*
		ScrollSlider scrollSlider = GameObject.FindObjectOfType<ScrollSlider> ();
		if (saveLastSelectedGroup && shapesManager != null) {
			scrollSlider.currentGroupIndex = shapesManager.lastSelectedGroup;
		}*/
	}

	void Update ()
	{
		if (lastShape != null) {
			//Set the bright postion to the last shape postion
			if (!Mathf.Approximately (lastShape.position.magnitude, shapeBright.position.magnitude)) {
				shapeBright.position = lastShape.position;
			}
		}
	}


	/// <summary>
	/// Creates the shapes in Groups.
	/// </summary>
	private IEnumerator CreateShapes ()
	{
		yield return 0;

		//Clear current shapes list
		shapes.Clear ();

		//The ID of the shape
		int ID = 0;
			
		//The scale ratio for the shape
		float ratio = Mathf.Max (Screen.width, Screen.height) / 1000.0f;

		//The group of the shape
		GameObject shapesGroup = null;

		//The index of the group
		int groupIndex = 0;

		pointersParent.gameObject.SetActive (false);
		groupsParent.gameObject.SetActive (false);

		//Create Shapes inside groups
		for (int i = 0; i < shapesManager.shapes.Count; i++) {

			if (i % shapesPerGroup == 0) {
				 groupIndex = (i / shapesPerGroup);
				shapesGroup = Group.CreateGroup (shapesGroupPrefab, groupsParent, groupIndex, columnsPerGroup);
				if (!EnableGroupGridLayout) {
					shapesGroup.GetComponent<GridLayoutGroup> ().enabled = false;
				}
				if (createGroupsPointers) {
					Pointer.CreatePointer (groupIndex, shapesGroup, pointerPrefab, pointersParent);
				}
			}

			//Create Shape
			ID = (i + 1);//the id of the shape
			GameObject tableShapeGameObject = Instantiate (shapePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			tableShapeGameObject.transform.SetParent (shapesGroup.transform);//setting up the shape's parent
			TableShape tableShapeComponent = tableShapeGameObject.GetComponent<TableShape> ();//get TableShape Component
			tableShapeComponent.ID = ID;//setting up shape ID
			tableShapeGameObject.name = "Shape-" + ID;//shape name
			tableShapeGameObject.transform.localScale = Vector3.one;
			tableShapeGameObject.GetComponent<RectTransform> ().offsetMax = Vector2.zero;
			tableShapeGameObject.GetComponent<RectTransform> ().offsetMin = Vector2.zero;

			GameObject uiShape = Instantiate (shapesManager.shapes [i].gamePrefab, Vector3.zero, Quaternion.identity) as GameObject;

			uiShape.transform.SetParent (tableShapeGameObject.transform.Find ("Content"));

			RectTransform rectTransform = tableShapeGameObject.transform.Find ("Content").GetComponent<RectTransform> ();
		
			uiShape.transform.localScale = new Vector3 (ratio * shapeScaleFactor, ratio * shapeScaleFactor);
			uiShape.GetComponent<RectTransform> ().anchoredPosition3D = Vector3.zero;

			List<Shape> shapeComponents = new List<Shape> ();
			if (uiShape.GetComponent<CompoundShape> () != null) {
				Shape[] tempS = uiShape.GetComponentsInChildren<Shape> ();
				foreach (Shape s in tempS) {
					shapeComponents.Add (s);
				}
			} else {
				shapeComponents.Add (uiShape.GetComponent<Shape> ());
			}

			int compoundID;
			for (int s = 0 ;s <shapeComponents.Count;s++) {
				CompoundShape compundShape = shapeComponents [s].transform.parent.GetComponent<CompoundShape> ();
				compoundID = 0;
				if (compundShape != null) {
					compoundID = compundShape.GetShapeIndexByInstanceID(shapeComponents[s].GetInstanceID());
				}

				shapeComponents[s].enabled = false;
				//release unwanted resources
				shapeComponents[s].GetComponent<Animator> ().enabled = false;
				shapeComponents[s].transform.Find ("TracingHand").gameObject.SetActive (false);
				shapeComponents[s].transform.Find ("Collider").gameObject.SetActive (false);

				Animator[] animators = shapeComponents[s].transform.GetComponentsInChildren<Animator> ();
				foreach (Animator a in animators) {
					a.enabled = false;
				}

				int from, to;
				string[] slices;
				List <Transform> paths = CommonUtil.FindChildrenByTag (shapeComponents[s].transform.Find ("Paths"), "Path");
				foreach (Transform p in paths) {
					slices = p.name.Split ('-');
					from = int.Parse (slices [1]);
					to = int.Parse (slices [2]);

					p.Find ("Start").gameObject.SetActive (false);
					Image img = CommonUtil.FindChildByTag (p, "Fill").GetComponent<Image> ();

					if (PlayerPrefs.HasKey (DataManager.GetPathStrKey(ID,compoundID,from,to,shapesManager))) {

						List<Transform> numbers = CommonUtil.FindChildrenByTag (p.transform.Find ("Numbers"), "Number");
						foreach (Transform n in numbers) {
							n.gameObject.SetActive (false);
						}
						img.fillAmount = 1;
						img.color =	DataManager.GetShapePathColor (ID, compoundID,from, to, shapesManager);
					}
				}

			}
			tableShapeGameObject.GetComponent<Button> ().onClick.AddListener (() => GameObject.FindObjectOfType<UIEvents> ().AlbumShapeEvent (tableShapeGameObject.GetComponent<TableShape> ()));

			SettingUpShape (tableShapeComponent, ID,groupIndex);//setting up the shape contents (stars number ,islocked,...)
			shapes.Add (tableShapeComponent);//add table shape component to the list
		}

		collectedStarsText.text = collectedStars + "/" + (3 * shapesManager.shapes.Count);
		if (shapesManager.shapes.Count == 0) {
			Debug.Log ("There are no Shapes found");
		} else {
			Debug.Log ("New shapes have been created");
		}

		GameObject.Find ("Loading").SetActive (false);

		pointersParent.gameObject.SetActive (true);
		groupsParent.gameObject.SetActive (true);

		GameObject.FindObjectOfType<ScrollSlider> ().Init ();

	}

	/// <summary>
	/// Settings up the shape contents in the table.
	/// </summary>
	/// <param name="tableShape">Table shape.</param>
	/// <param name="ID">ID of the shape.</param>
	/// <param name="groupIndex">Index of the group.</param>
	private void SettingUpShape (TableShape tableShape, int ID,int groupIndex)
	{
		if (tableShape == null) {
			return;
		}

		tableShape.isLocked = DataManager.IsShapeLocked (ID, shapesManager);
		tableShape.starsNumber = DataManager.GetShapeStars (ID, shapesManager);

		if (tableShape.ID == 1) {
			tableShape.isLocked = false;
		}

		if (!tableShape.isLocked) {
			tableShape.transform.Find ("Cover").gameObject.SetActive (false);
			tableShape.transform.Find ("Lock").gameObject.SetActive (false);
		} else {
			tableShape.GetComponent<Button> ().interactable = false;
			tableShape.transform.Find ("Stars").gameObject.SetActive (false);
		}

		//Set Last reached shape
		if (!tableShape.isLocked) {
			if (PlayerPrefs.HasKey (DataManager.GetLockedStrKey (ID + 1, shapesManager))) {
				if (DataManager.IsShapeLocked (ID + 1, shapesManager)) {
					SetSelectedGroup (groupIndex);
				} 
			} else if (!PlayerPrefs.HasKey (DataManager.GetStarsStrKey (ID, shapesManager))) {
				SetSelectedGroup (groupIndex);
			}
		}

		tempTransform = tableShape.transform.Find ("Stars");

		//Apply the current Stars Rating 
		if (tableShape.starsNumber == TableShape.StarsNumber.ONE) {//One Star
			tempTransform.Find ("FirstStar").GetComponent<Image> ().sprite = starOn;
			tempTransform.Find ("SecondStar").GetComponent<Image> ().sprite = starOff;
			tempTransform.Find ("ThirdStar").GetComponent<Image> ().sprite = starOff;
			collectedStars += 1;
		} else if (tableShape.starsNumber == TableShape.StarsNumber.TWO) {//Two Stars
			tempTransform.Find ("FirstStar").GetComponent<Image> ().sprite = starOn;
			tempTransform.Find ("SecondStar").GetComponent<Image> ().sprite = starOn;
			tempTransform.Find ("ThirdStar").GetComponent<Image> ().sprite = starOff;
			collectedStars += 2;
		} else if (tableShape.starsNumber == TableShape.StarsNumber.THREE) {//Three Stars
			tempTransform.Find ("FirstStar").GetComponent<Image> ().sprite = starOn;
			tempTransform.Find ("SecondStar").GetComponent<Image> ().sprite = starOn;
			tempTransform.Find ("ThirdStar").GetComponent<Image> ().sprite = starOn;
			collectedStars += 3;
		} else {//Zero Stars
			tempTransform.Find ("FirstStar").GetComponent<Image> ().sprite = starOff;
			tempTransform.Find ("SecondStar").GetComponent<Image> ().sprite = starOff;
			tempTransform.Find ("ThirdStar").GetComponent<Image> ().sprite = starOff;
		}
	}

	/// <summary>
	/// Set the selected group.
	/// </summary>
	/// <param name="groupIndex">Group index.</param>
	private void SetSelectedGroup(int groupIndex){
		//Setup the last selected group index
		ScrollSlider scrollSlider = GameObject.FindObjectOfType<ScrollSlider> ();
		scrollSlider.currentGroupIndex = groupIndex;
	}

	/// <summary>
	/// Raise the change group event.
	/// </summary>
	/// <param name="currentGroup">Current group.</param>
	public void OnChangeGroup (int currentGroup)
	{
		if (saveLastSelectedGroup) {
			shapesManager.lastSelectedGroup = currentGroup;
		}
	}
}