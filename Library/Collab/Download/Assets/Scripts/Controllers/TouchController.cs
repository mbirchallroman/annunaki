using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchController : InteractionController {
    
    public bool PlacingStructure { get; set; }

    public override Vector3 GetInteractionPoint() {

        if (Input.touchCount != 1)
            return new Vector3(-1, -1, -1);

        Touch touch = Input.GetTouch(0);
        return touch.position;

    }

    public override void MapManipulation() {

        if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject != null)
            return;

        Mode m = modeController.currentMode;
        Action act = actionController.currentAction;
        

        if (act == null) {

            PlacingStructure = false;
            ClearDragPreviews();

            if (m == Mode.Construct)
                ObjectWindow();

        }

        else {

            PlacingStructure = true;

            if (Time.timeScale == 0 && m == Mode.Construct)
                return;

            DropPreview();
            
            if (actionController.CanDrag(act) && dragEnabled) {

                if (act.What == "Road")
                    UpdatePathing();
                else
                    UpdateDragging();

            }

            else
                UpdateDropping();


        }

    }

    public override void ObjectWindow() {

        if (Input.touchCount != 1)
            return;

        Touch touch = Input.GetTouch(0);

        //right click release opens window if possible
        if (touch.tapCount == 2) {

            //don't open if paused game
            if (timeController.IsPaused)
                return;

            //grab coordinates
            int x = (int)touchCoords.x;
            int y = (int)touchCoords.z;

            //grab gameobject from world; only continue if it exists
            GameObject go = worldController.Map.GetBuildingAt(x, y);
            if (go == null)
                return;

            //open window only if it can be right clicked
            Obj o = go.GetComponent<Obj>();
            if (o.rightclick)
                go.GetComponent<Obj>().OpenWindow();

        }

    }

    public override void UpdateDropping() {


        Action act = actionController.currentAction;

        if (Input.touchCount != 1)
            return;

        Touch touch = Input.GetTouch(0);

        // Start Drag
        if (touch.phase == TouchPhase.Began)
            PlacingStructure = true;
        

        int x = Mathf.FloorToInt(touchCoords.x);
        int y = Mathf.FloorToInt(touchCoords.z);

        actionController.actionLocations = new Dictionary<Node, Action>();
        actionController.actionLocations.Add(new Node(x, y), act);
        
    }

    public override void UpdatePathing() {

        
        Action act = actionController.currentAction;
        
        // Start Drag
        if (Input.GetMouseButtonDown(0))
            dragStartPosition = touchCoords;

        int start_x = Mathf.FloorToInt(dragStartPosition.x);
        int end_x = Mathf.FloorToInt(touchCoords.x);
        int start_y = Mathf.FloorToInt(dragStartPosition.z);
        int end_y = Mathf.FloorToInt(touchCoords.z);

        Node start = new Node(start_x, start_y);
        Node end = new Node(end_x, end_y);

        if (lastPos != end) {

            Pathfinder p = worldController.Map.pathfinder;
            List<Node> path = p.FindPath(start, end);
            Dictionary<Node, Action> locs = new Dictionary<Node, Action>();
            lastPos = end;
            foreach (Node n in path)
                locs.Add(n, act);
            actionController.actionLocations = locs;

        }

        
        ClearDragPreviews();
        DragPreview();

    }

    public override void UpdateDragging() {

        Dictionary<Node, Action> locs = new Dictionary<Node, Action>();
        Action act = actionController.currentAction;

        if (Input.touchCount != 1)
            return;

        Touch touch = Input.GetTouch(0);

        // Start Drag
        if (touch.phase == TouchPhase.Began) {
            ClearDragPreviews();
            dragStartPosition = touchCoords;
            PlacingStructure = true;
        }

        int start_x = Mathf.FloorToInt(dragStartPosition.x);
        int end_x = Mathf.FloorToInt(touchCoords.x);
        int start_y = Mathf.FloorToInt(dragStartPosition.z);
        int end_y = Mathf.FloorToInt(touchCoords.z);

        Node end = new Node(end_x, end_y);

        // We may be dragging in the "wrong" direction, so flip things if needed.
        if (end_x < start_x) {
            int tmp = end_x;
            end_x = start_x;
            start_x = tmp;
        }
        if (end_y < start_y) {
            int tmp = end_y;
            end_y = start_y;
            start_y = tmp;
        }

        //only do if new position
        if(end != lastPos)
            for (int x = start_x; x <= end_x; x++) {
                for (int y = start_y; y <= end_y; y++) {
                
                    if(actionController.CanDo(act, x, y))
                        locs.Add(new Node(x, y), act);

                }
            }

        lastPos = end;

        actionController.actionLocations = locs;
        ClearDragPreviews();
        DragPreview();

    }

}
