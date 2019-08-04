using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionController : MonoBehaviour {

    public ActionController actionController;
    public CameraController cameraController;
    public ModeController modeController;
    public MoneyController moneyController;
    public TileMap tilemap;
    public TimeController timeController;
    public TooltipController tooltipController;
    public WorldController worldController;
    public Material cannotDo;
    public Material canDo;

    public bool mouseHittingSmth { get; set; }
    public float prevx { get; set; }
    public float prevy { get; set; }
    public GameObject preview { get; set; }
    public Node lastPos { get; set; }
    public Vector3 touchCoords { get; set; }
    public Vector3 dragStartPosition { get; set; }
    public List<GameObject> dragPreviewGameObjects { get; set; }

    // Use this for initialization
    void Start() {

        dragPreviewGameObjects = new List<GameObject>();

    }

    public void Update() {

        Mode m = modeController.currentMode;

        Destroy(preview);
        UpdateCoords();

        //if unpaused while window is open, close window
        if (!timeController.IsPaused && worldController.objWindow != null)
            worldController.objWindow.Close();

        if (m == Mode.Construct || m == Mode.Edit)
            MapManipulation();

    }

    public bool dragEnabled = true;
    public void EnableDrag(bool b) {

        dragEnabled = b;

    }
    
    public void UpdateCoords() {

        Vector3 pos = GetInteractionPoint();
        if (pos == new Vector3(-1, -1, -1))
            return;

        if (this is TouchController && (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null))
            return;

        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;

        mouseHittingSmth = tilemap.GetComponent<MeshCollider>().Raycast(ray, out hit, Mathf.Infinity);

        if (mouseHittingSmth) {
            int x = Mathf.FloorToInt(hit.point.x + .5f);
            int z = Mathf.FloorToInt(hit.point.z + .5f);

            touchCoords = new Vector3(x, 0, z);
            AlignMouse();

        }
        else if (this is TouchController)
            ClearDragPreviews();

    }

    public virtual Vector3 GetInteractionPoint() {

        return new Vector3(-1, -1, -1);

    }

    public virtual void MapManipulation() {



    }

    public virtual void ObjectWindow() {



    }

    public virtual void UpdateDragging() {



    }

    public virtual void UpdatePathing() {



    }

    public virtual void UpdateDropping() {



    }

    public void AlignMouse() {

        float x = touchCoords.x;
        float y = touchCoords.z;

        //default mouse alignment is 1
        int sizex = 1;
        int sizey = 1;

        //if action is placing a structure, check its size
        Action act = actionController.currentAction;
        if (act == null)
            return;
        if (act.Do == "place") {
            StructureData data = StructureDatabase.structureData[act.What];

            sizex = data.Sizex;
            sizey = data.Sizey;

            //switch size dimensions if building is rotated
            if (actionController.buildingRotation % 180 != 0) {
                int tempszx = sizex;
                int tempszy = sizey;
                sizex = tempszy;
                sizey = tempszx;
            }
        }

        Vector3 rotation = cameraController.transform.eulerAngles;

        //ADJUST ALIGNMENT OF BUILDING IF MAPVIEW IS ROTATED
        if (rotation.y == 90 || rotation.y == 180)
            y -= sizey - 1;

        if (rotation.y == 270 || rotation.y == 180)
            x -= sizex - 1;

        //KEEP BUILDING PREVIEW WITHIN WORLD LIMITS
        World world = worldController.Map;
        if (world.OutOfBounds((int)x, (int)y, sizex, sizey)) {
            if (x + sizex > world.size.X || x < 0)
                x = prevx;
            if (y + sizey > world.size.Y || y < 0)
                y = prevy;
        }

        //UPDATE MOUSE COORDS
        touchCoords = new Vector3(x, 0, y);


        //UPDATE PREV COORDS
        prevx = (int)x;
        prevy = (int)y;
    }

    public void DropPreview() {

        //grab preview object from actioncontroller
        preview = actionController.GetPreview(actionController.currentAction);

        //if the object is grabbed, move it to the mouse coords
        if (preview != null) {

            int x = (int)touchCoords.x;
            int y = (int)touchCoords.z;

            preview.transform.position = new Vector3(x, 0, y);
            preview.transform.eulerAngles = new Vector3(0, actionController.buildingRotation, 0);

            //if action is a strucutre, align it
            Action act = actionController.currentAction;
            if (act.Do == "place") {
                StructureData data = StructureDatabase.structureData[act.What];

                int sizex = data.Sizex;
                int sizey = data.Sizey;
                float alignx = data.Alignx;
                float aligny = data.Aligny;

                //switch size dimensions if building is rotated
                if (actionController.buildingRotation % 180 != 0) {
                    int tempszx = sizex;
                    int tempszy = sizey;
                    sizex = tempszy;
                    sizey = tempszx;

                    float tempalignx = alignx;
                    float tempaligny = aligny;
                    alignx = tempaligny;
                    aligny = tempalignx;

                }

                preview.transform.position += new Vector3(alignx, 0, aligny);
            }

            Material m = cannotDo;
            if (actionController.CanDo(act, x, y))
                m = canDo;
            preview.GetComponent<MeshRenderer>().material = m;

            for (int a = 0; a < preview.GetComponent<MeshRenderer>().materials.Length; a++)
                preview.GetComponent<MeshRenderer>().materials[a] = m;

        }


    }

    public void DragPreview() {

        Dictionary<Node, Action> actionLocations = actionController.actionLocations;

        foreach (Node n in actionLocations.Keys) {

            int x = (int)n.X;
            int y = (int)n.Y;

            Action act = actionLocations[n];

            //display preview of action at tile
            GameObject go = actionController.GetPreview(act);
            go.transform.position = new Vector3(x, 0, y);
            dragPreviewGameObjects.Add(go);

            if (actionController.CanDo(act, x, y))
                preview.GetComponent<MeshRenderer>().material = canDo;

            //if paint, then make preview invisible. otherwise make it red
            else {

                if (act.Do == "paint")
                    go.SetActive(false);

                else if (act.Do == "place")
                    go.GetComponent<MeshRenderer>().material = cannotDo;

            }

        }

    }
    
    public void ClearDragPreviews() {
        while (dragPreviewGameObjects.Count > 0) {
            GameObject go = dragPreviewGameObjects[0];
            dragPreviewGameObjects.RemoveAt(0);
            SimplePool.Despawn(go);
        }
    }

    public void PerformSelectedActions() {

        actionController.PerformActions();
        ClearDragPreviews();
        dragStartPosition = touchCoords;

    }

    public void UndoActions() {

        actionController.UndoActions();

    }

}
