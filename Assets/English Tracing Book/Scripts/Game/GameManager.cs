using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Whether the script is running or not.
    /// </summary>
    public bool isRunning = true;

    /// <summary>
    /// The current pencil.
    /// </summary>
    public Pencil currentPencil;

    /// <summary>
    /// The shape order.
    /// </summary>
    public Text shapeOrder;

    /// <summary>
    /// The write shape name text.
    /// </summary>
    public Text writeText;

    /// <summary>
    /// The path.
    /// </summary>
    private EnglishTracingBook.Path path;

    /// <summary>
    /// The shape parent.
    /// </summary>
    public Transform shapeParent;

    /// <summary>
    /// The shape reference.
    /// </summary>
    [HideInInspector]
    public Shape shape;

    /// <summary>
    /// The path fill image.
    /// </summary>
    private Image pathFillImage;

    /// <summary>
    /// The click postion.
    /// </summary>
    private Vector3 clickPostion;

    /// <summary>
    /// The direction between click and shape.
    /// </summary>
    private Vector2 direction;

    /// <summary>
    /// The current angle , angleOffset and fill amount.
    /// </summary>
    private float angle, angleOffset, fillAmount;

    /// <summary>
    /// The clock wise sign.
    /// </summary>
    private float clockWiseSign;

    /// <summary>
    /// The hand reference.
    /// </summary>
    public Transform hand;

    /// <summary>
    /// The default size of the cursor.
    /// </summary>
    private Vector3 cursorDefaultSize;

    /// <summary>
    /// The click size of the cursor.
    /// </summary>
    private Vector3 cursorClickSize;

    /// <summary>
    /// The target quarter of the radial fill.
    /// </summary>
    private float targetQuarter;

    /// <summary>
    /// The effects audio source.
    /// </summary>
    private AudioSource effectsAudioSource;

    /// <summary>
    /// The bright effect.
    /// </summary>
    public Transform brightEffect;

    /// <summary>
    /// The complete effect.
    /// </summary>
    public ParticleSystem completeEffect; //MODIFIED: Changed ParticleEmitter to ParticleSystem

    /// <summary>
    /// The timer reference. 
    /// </summary>
    public Timer timer;

    /// <summary>
    /// The window dialog reference.
    /// </summary>
    public WinDialog winDialog;

    /// <summary>
    /// The completed sound effect.
    /// </summary>
    public AudioClip completedSFX;

    /// <summary>
    /// The correct sound effect.
    /// </summary>
    public AudioClip correctSFX;

    /// <summary>
    /// The wrong sound effect.
    /// </summary>
    public AudioClip wrongSFX;

    /// <summary>
    /// The locked sound effect.
    /// </summary>
    public AudioClip lockedSFX;

    /// <summary>
    /// The hit2d reference.
    /// </summary>
    private RaycastHit2D hit2d;

    /// <summary>
    /// The shapes manager reference.
    /// </summary>
    private ShapesManager shapesManager;

    /// <summary>
    /// The compound shape reference.
    /// </summary>
    public static CompoundShape compoundShape;

    public Image shapePicture;

    void Awake ()
    {
        //Initiate values and setup the references
        cursorDefaultSize = hand.transform.localScale;
        cursorClickSize = cursorDefaultSize / 1.2f;

        if (!string.IsNullOrEmpty (ShapesManager.shapesManagerReference)) {
            shapesManager = GameObject.Find (ShapesManager.shapesManagerReference).GetComponent<ShapesManager>();
        } else {
            Debug.LogErrorFormat ("You have to start the game from the Main scene");
        }

        if (currentPencil != null) {
            currentPencil.EnableSelection ();
        }

        shapePicture.sprite = shapesManager.shapes [TableShape.selectedShape.ID - 1].picture;
        if (shapePicture.sprite == null) {
            shapePicture.enabled = false;
        }
        ResetTargetQuarter ();
        SetShapeOrderColor ();
        CreateShape ();
    }

    void Start(){
        //Initiate values and setup the references
        if (effectsAudioSource == null) {
            effectsAudioSource = GameObject.Find ("AudioSources").GetComponents<AudioSource> () [1];
        }
    }

    // Update is called once per frame
    void Update ()
    {
        //Game Logic is here

        DrawHand (GetCurrentPlatformClickPosition (Camera.main));
        DrawBrightEffect (GetCurrentPlatformClickPosition (Camera.main));

        if (shape == null) {
            return;
        }   

        if (shape.completed) {
            return;
        }

        if (Input.GetMouseButtonDown (0)) {
            if (!shape.completed)
                //MODIFIED: Replaced .emit with Play()
                brightEffect.GetComponent<ParticleSystem>().Play();

            hit2d = Physics2D.Raycast (GetCurrentPlatformClickPosition (Camera.main), Vector2.zero);
            if (hit2d.collider != null) {
                if (hit2d.transform.tag == "Start") {
                    OnStartHitCollider (hit2d);
                    shape.CancelInvoke ();
                    shape.DisableTracingHand ();
                    EnableHand ();
                } else if (hit2d.transform.tag == "Collider") {
                    shape.DisableTracingHand ();
                    EnableHand ();
                }
            }

        } else if (Input.GetMouseButtonUp (0)) {
            //MODIFIED: Replaced .emit with Stop()
            brightEffect.GetComponent<ParticleSystem>().Stop();
            DisableHand ();
            shape.Invoke ("EnableTracingHand", 1);
            ResetPath ();
        }

        if (!isRunning || path == null || pathFillImage == null) {
            return;
        }

        if (path.completed) {
            return;
        }
        
        if (path.fillMethod == EnglishTracingBook.Path.FillMethod.Radial) {
            RadialFill ();
        } else if (path.fillMethod == EnglishTracingBook.Path.FillMethod.Linear) {
            LinearFill ();
        } else if (path.fillMethod == EnglishTracingBook.Path.FillMethod.Point) {
            PointFill ();
        }
    }

    /// <summary>
    /// On the start hit collider event.
    /// </summary>
    /// <param name="hit2d">Hit2d.</param>
    private void OnStartHitCollider (RaycastHit2D hit2d)
    {
        path = hit2d.transform.GetComponentInParent<EnglishTracingBook.Path> ();

        pathFillImage = CommonUtil.FindChildByTag (path.transform, "Fill").GetComponent<Image> ();

        if (path.completed || !shape.IsCurrentPath (path)) {
            ReleasePath ();
        } else {
            path.StopAllCoroutines ();
            CommonUtil.FindChildByTag (path.transform, "Fill").GetComponent<Image> ().color = currentPencil.value;
        }

        if(path!=null)
        if (!path.shape.enablePriorityOrder) {
            shape = path.shape;
        }
    }

    /// <summary>
    /// Go to the Next shape.
    /// </summary>
    public void NextShape ()
    {
        if (TableShape.selectedShape.ID >= 1 && TableShape.selectedShape.ID < ShapesTable.shapes.Count) {
            //Get the next shape and check if it's locked , then do not load the next shape
            if (TableShape.selectedShape.ID + 1 <=shapesManager.shapes.Count) {

                if (DataManager.IsShapeLocked (TableShape.selectedShape.ID + 1,shapesManager)) {
                    //Play lock sound effectd
                    if (lockedSFX != null && effectsAudioSource != null) {
                        CommonUtil.PlayOneShotClipAt (lockedSFX, Vector3.zero, effectsAudioSource.volume);
                    }
                    //Skip the next
                    return;
                }
            }
            TableShape.selectedShape = ShapesTable.shapes [TableShape.selectedShape.ID];//Set the selected shape
            CreateShape ();//Create new shape

        } else {
            if (TableShape.selectedShape.ID == ShapesTable.shapes.Count) {
                GameObject.FindObjectOfType<UIEvents> ().LoadAlbumScene ();
            } else {
                //Play lock sound effectd
                if (lockedSFX != null && effectsAudioSource != null) {
                    CommonUtil.PlayOneShotClipAt (lockedSFX, Vector3.zero, effectsAudioSource.volume);
                }
            }

        }
    }

    /// <summary>
    /// Go to the previous shape.
    /// </summary>
    public void PreviousShape ()
    {
        if (TableShape.selectedShape.ID > 1 && TableShape.selectedShape.ID <= ShapesTable.shapes.Count) {
            TableShape.selectedShape = ShapesTable.shapes [TableShape.selectedShape.ID - 2];
            CreateShape ();
        } else {
            //Play lock sound effectd
            if (lockedSFX != null && effectsAudioSource != null) {
                CommonUtil.PlayOneShotClipAt (lockedSFX, Vector3.zero, effectsAudioSource.volume);
            }
        }
    }


    /// <summary>
    /// Create new shape.
    /// </summary>
    private void CreateShape ()
    {
        timer.Reset ();
        //MODIFIED: Replaced .emit with Stop()
        if(completeEffect != null)
            completeEffect.Stop ();
        GameObject.Find ("ResetConfirmDialog").GetComponent<Dialog> ().Hide ();
        Area.Hide ();
        winDialog.Hide ();
        GameObject.Find ("NextButton").GetComponent<Animator> ().SetBool ("Select", false);

        CompoundShape currentCompoundShape = GameObject.FindObjectOfType<CompoundShape> ();
        if (currentCompoundShape != null) {
            DestroyImmediate (currentCompoundShape.gameObject);
        } else {
            Shape shapeComponent = GameObject.FindObjectOfType<Shape> ();
            if (shapeComponent != null) {
                DestroyImmediate (shapeComponent.gameObject);
            }
        }

        try {
            shapeOrder.text = TableShape.selectedShape.ID + "/" + shapesManager.shapes.Count;
            shapesManager.lastSelectedGroup = TableShape.selectedShape.ID - 1;
            GameObject shapePrefab = shapesManager.shapes [TableShape.selectedShape.ID - 1].gamePrefab;
            GameObject shapeGameObject = Instantiate (shapePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            shapeGameObject.transform.SetParent (shapeParent);
            shapeGameObject.transform.localPosition = shapePrefab.transform.localPosition;
            shapeGameObject.name = shapePrefab.name;
            shapeGameObject.transform.localScale = shapePrefab.transform.localScale;

            compoundShape = GameObject.FindObjectOfType<CompoundShape> ();
            if(compoundShape!=null){
                shape = compoundShape.shapes[0];
                StartAutoTracing(shape);
            }else{
                shape = GameObject.FindObjectOfType<Shape> ();
            }
        } catch (System.Exception) {
            //Catch the exception or display an alert
            //Debug.LogError(ex.Message);
        }

        if (shape == null) {
            return;
        }

        shape.Spell ();

        if (writeText != null)
            writeText.text = "Write the " + shapesManager.shapeLabel.ToLower () + " '" + shape.GetTitle () + "'";
        Transform restConfirmMessage = CommonUtil.FindChildByTag (GameObject.Find ("ResetConfirmDialog").transform, "Message");
        restConfirmMessage.GetComponent<Text> ().text = "Reset " + shapesManager.shapeLabel + " " + shape.GetTitle () + " ?";
        EnableGameManager ();
    }

    /// <summary>
    /// Draw the hand.
    /// </summary>
    /// <param name="clickPosition">Click position.</param>
    private void DrawHand (Vector3 clickPosition)
    {
        if (hand == null) {
            return;
        }

        hand.transform.position = clickPosition;
    }

    /// <summary>
    /// Set the size of the hand to default size.
    /// </summary>
    private void SetHandDefaultSize ()
    {
        hand.transform.localScale = cursorDefaultSize;
    }

    /// <summary>
    /// Set the size of the hand to click size.
    /// </summary>
    private void SetHandClickSize ()
    {
        hand.transform.localScale = cursorClickSize;
    }

    /// <summary>
    /// Get the current platform click position.
    /// </summary>
    /// <returns>The current platform click position.</returns>
    private Vector3 GetCurrentPlatformClickPosition (Camera camera)
    {
        Vector3 clickPosition = Vector3.zero;

        if (Application.isMobilePlatform) {//current platform is mobile
            if (Input.touchCount != 0) {
                Touch touch = Input.GetTouch (0);
                clickPosition = touch.position;
            }
        } else {//others
            clickPosition = Input.mousePosition;
        }

        clickPosition = camera.ScreenToWorldPoint (clickPosition);//get click position in the world space
        clickPosition.z = 0;
        return clickPosition;
    }

    /// <summary>
    /// Radial the fill method.
    /// </summary>
    private void RadialFill ()
    {
        clickPostion = Camera.main.ScreenToWorldPoint (Input.mousePosition);

        direction = clickPostion - path.transform.position;

        angleOffset = 0;
        clockWiseSign = (pathFillImage.fillClockwise ? 1 : -1);

        if (pathFillImage.fillOrigin == 0) {//Bottom
            angleOffset = 0;
        } else if (pathFillImage.fillOrigin == 1) {//Right
            angleOffset = clockWiseSign * 90;
        } else if (pathFillImage.fillOrigin == 2) {//Top
            angleOffset = -180;
        } else if (pathFillImage.fillOrigin == 3) {//left
            angleOffset = -clockWiseSign * 90;
        }

        angle = Mathf.Atan2 (-clockWiseSign * direction.x, -direction.y) * Mathf.Rad2Deg + angleOffset;

        if (angle < 0)
            angle += 360;

        angle = Mathf.Clamp (angle, 0, 360);
        angle -= path.radialAngleOffset;

        if (path.quarterRestriction) {
            if (!(angle >= 0 && angle <= targetQuarter)) {
                pathFillImage.fillAmount = 0;
                return;
            }

            if (angle >= targetQuarter / 2) {
                targetQuarter += 90;
            } else if (angle < 45) {
                targetQuarter = 90;
            }

            targetQuarter = Mathf.Clamp (targetQuarter, 90, 360);
        }

        fillAmount = Mathf.Abs (angle / 360.0f);
        pathFillImage.fillAmount = fillAmount;
        CheckPathComplete ();
    }

    /// <summary>
    /// Linear fill method.
    /// </summary>
    private void LinearFill ()
    {
        clickPostion = Camera.main.ScreenToWorldPoint (Input.mousePosition);

        Vector3 rotation = path.transform.eulerAngles;
        rotation.z -= path.offset;

        Rect rect = CommonUtil.RectTransformToScreenSpace (path.GetComponent<RectTransform> ());

        Vector3 pos1 = Vector3.zero, pos2 = Vector3.zero;

        if (path.type == EnglishTracingBook.Path.ShapeType.Horizontal) {
            pos1.x = path.transform.position.x - Mathf.Sin (rotation.z * Mathf.Deg2Rad) * rect.width / 2.0f;
            pos1.y = path.transform.position.y - Mathf.Cos (rotation.z * Mathf.Deg2Rad) * rect.width / 2.0f;

            pos2.x = path.transform.position.x + Mathf.Sin (rotation.z * Mathf.Deg2Rad) * rect.width / 2.0f;
            pos2.y = path.transform.position.y + Mathf.Cos (rotation.z * Mathf.Deg2Rad) * rect.width / 2.0f;
        } else {

            pos1.x = path.transform.position.x - Mathf.Cos (rotation.z * Mathf.Deg2Rad) * rect.height / 2.0f;
            pos1.y = path.transform.position.y - Mathf.Sin (rotation.z * Mathf.Deg2Rad) * rect.height / 2.0f;

            pos2.x = path.transform.position.x + Mathf.Cos (rotation.z * Mathf.Deg2Rad) * rect.height / 2.0f;
            pos2.y = path.transform.position.y + Mathf.Sin (rotation.z * Mathf.Deg2Rad) * rect.height / 2.0f;
        }

        pos1.z = path.transform.position.z;
        pos2.z = path.transform.position.z;

        if (path.flip) {
            Vector3 temp = pos2;
            pos2 = pos1;
            pos1 = temp;
        }

        clickPostion.x = Mathf.Clamp (clickPostion.x, Mathf.Min (pos1.x, pos2.x), Mathf.Max (pos1.x, pos2.x));
        clickPostion.y = Mathf.Clamp (clickPostion.y, Mathf.Min (pos1.y, pos2.y), Mathf.Max (pos1.y, pos2.y));
        fillAmount = Vector2.Distance (clickPostion, pos1) / Vector2.Distance (pos1, pos2);
        pathFillImage.fillAmount = fillAmount;
        CheckPathComplete ();
    }

    /// <summary>
    /// Point fill.
    /// </summary>
    private void PointFill ()
    {
        pathFillImage.fillAmount = 1;
        CheckPathComplete ();
    }

    /// <summary>
    /// Checks wehther path completed or not.
    /// </summary>
    private void CheckPathComplete ()
    {
        if (fillAmount >= path.completeOffset) {

            path.completed = true;
            path.AutoFill ();
            path.SetNumbersVisibility (false);
            ReleasePath ();
            if (CheckShapeComplete ()) {
                shape.completed = true;
                OnShapeComplete ();
            } else {
                PlayCorrectSFX ();
            }

            shape.ShowPathNumbers (shape.GetCurrentPathIndex ());

            hit2d = Physics2D.Raycast (GetCurrentPlatformClickPosition (Camera.main), Vector2.zero);
            if (hit2d.collider != null) {
                if (hit2d.transform.tag == "Start") {
                    if (shape.IsCurrentPath (hit2d.transform.GetComponentInParent<EnglishTracingBook.Path> ())) {
                        OnStartHitCollider (hit2d);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Check whether the shape completed or not.
    /// </summary>
    /// <returns><c>true</c>, if shape completed, <c>false</c> otherwise.</returns>
    private bool CheckShapeComplete ()
    {
        bool shapeCompleted = true;
        EnglishTracingBook.Path [] paths = shape.GetComponentsInChildren<EnglishTracingBook.Path>();
        foreach (EnglishTracingBook.Path path in paths) {
            if (!path.completed) {
                shapeCompleted = false;
                break;
            }
        }
        return shapeCompleted;
    }

    /// <summary>
    /// On shape completed event.
    /// </summary>
    private void OnShapeComplete ()
    {
        bool allDone = true;

        List<Shape> shapes = new List<Shape> ();

        if (compoundShape != null) {
            shapes = compoundShape.shapes;
            allDone = compoundShape.IsCompleted ();

            if (!allDone) {
                shape = compoundShape.shapes [compoundShape.GetCurrentShapeIndex ()];
                StartAutoTracing (shape);
            }
        } else {
            shapes.Add (shape);
        }

        if (allDone) {
            SaveShapeStatus (shapes);

            DisableHand ();
            //MODIFIED: Replaced .emit with Stop()
            brightEffect.GetComponent<ParticleSystem>().Stop();

            foreach (Shape s in shapes) {
                Animator shapeAnimator = s.GetComponent<Animator> ();
                shapeAnimator.SetBool (s.name, false);
                shapeAnimator.SetTrigger ("Completed");
            }

            timer.Stop ();
            Area.Show ();
            winDialog.Show ();
            GameObject.Find ("NextButton").GetComponent<Animator> ().SetTrigger ("Select");
            //MODIFIED: Replaced .emit with Play()
            completeEffect.Play ();
            if (completedSFX != null && effectsAudioSource != null) {
                CommonUtil.PlayOneShotClipAt (completedSFX, Vector3.zero, effectsAudioSource.volume);
            }
            
        } else {
            PlayCorrectSFX ();
        }
    }

    /// <summary>
    /// Save the status of the shape(stars,path colors) .
    /// </summary>
    private void SaveShapeStatus(List<Shape>shapes){

        DataManager.SaveShapeStars (TableShape.selectedShape.ID, CommonUtil.GetTableShapeStars (GameObject.FindObjectOfType<Progress> ().starsNumber),shapesManager);
        if (TableShape.selectedShape .ID + 1 <= shapesManager.shapes.Count) {
            DataManager.SaveShapeLockedStatus (TableShape.selectedShape.ID + 1, false,shapesManager);
        }

        int compundID = 0;

        foreach(Shape s in shapes){
            if (compoundShape != null) {
                compundID = compoundShape.GetShapeIndexByInstanceID (s.GetInstanceID ());
            }
            List <Transform> paths = CommonUtil.FindChildrenByTag (s.transform.Find ("Paths"), "Path");
            int from, to;
            string [] slices;
            foreach (Transform p in paths) {
                slices = p.name.Split ('-');
                from = int.Parse (slices [1]);
                to = int.Parse (slices [2]);
                DataManager.SaveShapePathColor (TableShape.selectedShape.ID, compundID,from, to, CommonUtil.FindChildByTag (p, "Fill").GetComponent<Image> ().color,shapesManager);
            }
        }
    }

    /// <summary>
    /// Draw the bright effect.
    /// </summary>
    /// <param name="clickPosition">Click position.</param>
    private void DrawBrightEffect (Vector3 clickPosition)
    {
        if (brightEffect == null) {
            return;
        }

        clickPosition.z = 0;
        brightEffect.transform.position = clickPosition;
    }

    /// <summary>
    /// Reset the shape.
    /// </summary>
    public void ResetShape ()
    {
        List<Shape> shapes = new List<Shape> ();
        if (compoundShape != null) {
            shapes = compoundShape.shapes;
        } else {
            shapes.Add (shape);
        }

        //MODIFIED: Replaced .emit with Stop()
        completeEffect.Stop ();
        GameObject.Find ("NextButton").GetComponent<Animator> ().SetBool ("Select", false);
        Area.Hide ();
        winDialog.Hide ();

        foreach (Shape s in shapes) {
            if (s == null)
                continue;
            
            s.completed = false;
            s.GetComponent<Animator> ().SetBool ("Completed", false);
            s.CancelInvoke ();
            s.DisableTracingHand ();
            EnglishTracingBook.Path[] paths = s.GetComponentsInChildren<EnglishTracingBook.Path> ();
            foreach (EnglishTracingBook.Path path in paths) {
                path.Reset ();
            }

            if (compoundShape == null) {
                StartAutoTracing (s);
            } else if (compoundShape.GetShapeIndexByInstanceID (s.GetInstanceID()) == 0) {
                shape = compoundShape.shapes[0];
                StartAutoTracing (shape);
            }

            s.Spell ();
        }
        timer.Reset ();
    }


    /// <summary>
    /// Starts the auto tracing for the current path.
    /// </summary>
    /// <param name="s">Shape Reference.</param>
    public void StartAutoTracing(Shape s){
        if (s == null) {
            return;
        }

        //Hide Numbers for other shapes , if we have compound shape
        if (compoundShape != null) {
            foreach (Shape ts in compoundShape.shapes) {
                if(s.GetInstanceID()!= ts.GetInstanceID())
                    ts.ShowPathNumbers (-1);
            }
        }

        s.Invoke ("EnableTracingHand", 2);
        s.ShowPathNumbers (s.GetCurrentPathIndex ());
    }

    /// <summary>
    /// Play the correct SFX.
    /// </summary>
    public void PlayCorrectSFX(){
        if (correctSFX != null && effectsAudioSource != null) {
            CommonUtil.PlayOneShotClipAt (correctSFX, Vector3.zero, effectsAudioSource.volume);
        }
    }

    /// <summary>
    /// Reset the path.
    /// </summary>
    private void ResetPath ()
    {
        if (path != null) 
            path.Reset ();
        ReleasePath ();
        ResetTargetQuarter ();
    }

    /// <summary>
    /// Reset the target quarter.
    /// </summary>
    private void ResetTargetQuarter ()
    {
        targetQuarter = 90;
    }

    /// <summary>
    /// Release the path.
    /// </summary>
    private void ReleasePath ()
    {
        path = null;
        pathFillImage = null;
    }

    /// <summary>
    /// Set the color of the shape order.
    /// </summary>
    public void SetShapeOrderColor ()
    {
        if (currentPencil == null) {
            return;
        }
        shapeOrder.color = currentPencil.value;
    }

    /// <summary>
    /// Enable the hand.
    /// </summary>
    public void EnableHand ()
    {

        hand.GetComponent<SpriteRenderer> ().enabled = true;
    }

    /// <summary>
    /// Disable the hand.
    /// </summary>
    public void DisableHand ()
    {
        hand.GetComponent<SpriteRenderer> ().enabled = false;
    }

    /// <summary>
    /// Disable the game manager.
    /// </summary>
    public void DisableGameManager ()
    {
        isRunning = false;
    }

    /// <summary>
    /// Enable the game manager.
    /// </summary>
    public void EnableGameManager ()
    {
        isRunning = true;
    }
}