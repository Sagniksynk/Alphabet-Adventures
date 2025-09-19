using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataManager
{
	/// <summary>
	/// Save the number of stars of the given shape
	/// </summary>
	/// <param name="ID">Shape ID.</param>
	/// <param name="stars">Stars.</param>
	/// <param name="shapesManager">Shapes manager.</param>
	public static void SaveShapeStars (int ID, TableShape.StarsNumber stars,ShapesManager shapesManager)
	{
		PlayerPrefs.SetInt (GetStarsStrKey(ID,shapesManager), CommonUtil.ShapeStarsNumberEnumToIntNumber (stars));
		PlayerPrefs.Save ();
	}

	/// <summary>
	/// Get the number of stars of the given shape
	/// </summary>
	/// <returns>The shape stars.</returns>
	/// <param name="ID">Shape ID.</param>
	/// <param name="shapesManager">Shapes manager.</param>
	public static TableShape.StarsNumber GetShapeStars (int ID,ShapesManager shapesManager)
	{
		TableShape.StarsNumber stars = TableShape.StarsNumber.ZERO;
		string key = GetStarsStrKey(ID,shapesManager);
		if (PlayerPrefs.HasKey (key)) {
			stars = CommonUtil.IntNumberToShapeStarsNumberEnum (PlayerPrefs.GetInt (key));
		}
		return stars;
	}

	/// <summary>
	/// Save the color of the path.
	/// </summary>
	/// <param name="Shape ID">Shape ID.</param>
	/// <param name="compundID">Compund ID.</param>
	/// <param name="from">From.</param>
	/// <param name="to">To.</param>
	/// <param name="color">Color value.</param>
	/// <param name="shapesManager">Shapes manager.</param>
	public static void SaveShapePathColor (int shapeID,int compundID, int from, int to, Color color,ShapesManager shapesManager)
	{
		string key = GetPathStrKey(shapeID,compundID,from,to,shapesManager);
		string value = color.r + "," + color.g + "," + color.b + "," + color.a;

		PlayerPrefs.SetString (key, value);
		PlayerPrefs.Save ();
	}


	/// <summary>
	/// Get the color of the shape path.
	/// </summary>
	/// <returns>The shape path color.</returns>
	/// <param name="Shape ID">Shape ID.</param>
	/// <param name="compundID">Compund ID.</param>
	/// <param name="from">From.</param>
	/// <param name="to">To.</param>
	/// <param name="shapesManager">Shapes manager.</param>
	public static Color GetShapePathColor (int shapeID, int compundID,int from, int to,ShapesManager shapesManager)
	{
		Color color = Color.white;
		string key = GetPathStrKey(shapeID,compundID,from,to,shapesManager);

		if (PlayerPrefs.HasKey (key)) {
			color = CommonUtil.StringRGBAToColor (PlayerPrefs.GetString (key));
		}
	
		return color;
	}


	/// <summary>
	/// Return the string key of specific path.
	/// </summary>
	/// <returns>The string key.</returns>
	/// <param name="shapeID">Shape ID.</param>
	/// <param name="compundID">Compund ID.</param>
	/// <param name="from">From.</param>
	/// <param name="to">To.</param>
	/// <param name="shapesManager">Shapes manager.</param>
	public static string GetPathStrKey(int shapeID, int compundID,int from, int to,ShapesManager shapesManager){
		return shapesManager.shapePrefix + "-Shape-" + shapeID + "-Compound-" + compundID + "-Path-" + from + "-" + to;
	}

	/// <summary>
	/// Return the locked string key of specific shape.
	/// </summary>
	/// <returns>The locked string key.</returns>
	/// <param name="ID">Shape ID.</param>
	/// <param name="shapesManager">Shapes manager.</param>
	public static string GetLockedStrKey(int ID,ShapesManager shapesManager){
		return shapesManager.shapePrefix + "-Shape-" + ID + "-isLocked";
	}

	/// <summary>
	/// Return the stars string key of specific shape.
	/// </summary>
	/// <returns>The stars string key.</returns>
	/// <param name="ID">Shape ID.</param>
	/// <param name="shapesManager">Shapes manager.</param>
	public static string GetStarsStrKey(int ID,ShapesManager shapesManager){
		return shapesManager.shapePrefix + "-Shape-" + ID + "-Stars";
	}

	/// <summary>
	/// Determine if is shape locked the specified ID.
	/// </summary>
	/// <returns><c>true</c> if is shape locked the specified ID shapesManager; otherwise, <c>false</c>.</returns>
	/// <param name="ID">I.</param>
	/// <param name="shapesManager">Shapes manager.</param>
	public static bool IsShapeLocked (int ID,ShapesManager shapesManager)
	{
		bool isLocked = true;
		string key = GetLockedStrKey(ID,shapesManager);
		if (PlayerPrefs.HasKey (key)) {
			isLocked = CommonUtil.ZeroOneToTrueFalseBool (PlayerPrefs.GetInt (key));
		}
		return isLocked;
	}

	/// <summary>
	/// Save the shape locked status.
	/// </summary>
	/// <param name="ID">I.</param>
	/// <param name="isLocked">If set to <c>true</c> is locked.</param>
	/// <param name="shapesManager">Shapes manager.</param>
	public static void SaveShapeLockedStatus (int ID, bool isLocked,ShapesManager shapesManager)
	{
		PlayerPrefs.SetInt (GetLockedStrKey(ID,shapesManager), CommonUtil.TrueFalseBoolToZeroOne (isLocked));
		PlayerPrefs.Save ();
	}

	/// <summary>
	/// Get the collected stars for the shapes in the given shapes manager.
	/// </summary>
	/// <returns>The collected stars.</returns>
	/// <param name="shapesManager">Shapes manager.</param>
	public static int GetCollectedStars(ShapesManager shapesManager){

		int ID = 0;
		int cs = 0;
		for (int i = 0; i < shapesManager.shapes.Count; i++) {
			ID = (i + 1);
			TableShape.StarsNumber sn = GetShapeStars (ID, shapesManager);
			if (sn == TableShape.StarsNumber.ONE) {
				cs+=1;
			} else if (sn == TableShape.StarsNumber.TWO) {
				cs+=2;
			} else if (sn == TableShape.StarsNumber.THREE) {
				cs+=3;
			}
		}
		return cs;
	}

	/// <summary>
	/// Reset the game.
	/// </summary>
	public static void ResetGame ()
	{
		PlayerPrefs.DeleteAll ();
	}
}