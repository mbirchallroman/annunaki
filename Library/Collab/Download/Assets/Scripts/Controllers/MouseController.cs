using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : InteractionController {

    
    public bool UIhover { get; set; }
    public void MouseOverUI(bool b) {
        UIhover = b;
    }

    public override Vector3 GetInteractionPoint() {

        return Input.mousePosition;

    }

    public override void MapManipulation() {

        if (UIhover || !mouseHittingSmth)
            return;

        //if there is a current action, try construction
        Mode m = modeController.currentMode;
        Action act = actionController.currentAction;
        if (act == null) {

            //if construct mode is on, try opening a window
            if (m == Mode.Construct)
                ObjectWindow();

        }
        
        else {

            //if game is NOT paused during a non-construction mode
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

        //right click initial closes any open window
        if (Input.GetMouseButtonDown(1))
            if (worldController.objWindow != null)
                worldController.objWindow.Close();

        //right click release opens window if possible
        if (Input.GetMouseButtonUp(1)) {

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

    public override void UpdateDragging() {

        
        Action act = actionController.currentAction;

        // Start Drag
        if (Input.GetMouseButtonDown(0))
            dragStartPosition = touchCoords;

        int start_x = Mathf.FloorToInt(dragStartPosition.x);
        int end_x = Mathf.FloorToInt(touchCoords.x);
        int start_y = Mathf.FloorToInt(dragStartPosition.z);
        int end_y = Mathf.FloorToInt(touchCoords.z);

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

        // Clean up old drag previews
        

        if (Input.GetMouseButton(0)) {

            Dictionary<Node, Action> locs = new Dictionary<Node, Action>();


            // Display a preview of the drag area
            for (int x = start_x; x <= end_x; x++) {
                for (int y = start_y; y <= end_y; y++) {

                    if(actionController.CanDo(act, x, y))
                        locs.Add(new Node(x, y), act);

                }
            }

            actionController.actionLocations = locs;

        }

        
        ClearDragPreviews();
        DragPreview();

        // End Drag
        if (Input.GetMouseButtonUp(0))
            PerformSelectedActions();

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

        if (Input.GetMouseButton(0) && lastPos != end) {

            Pathfinder p = worldController.Map.pathfinder;
            List<Node> path = p.FindPath(start, end);
            Dictionary<Node, Action> locs = new Dictionary<Node, Action>();
            lastPos = end;
            foreach (Node n in path)
                if (actionController.CanDo(act, (int)n.X, (int)n.Y))
                    locs.Add(n, act);

            actionController.actionLocations = locs;

        }
        
        ClearDragPreviews();
        DragPreview();


        // End Drag
        if (Input.GetMouseButtonUp(0))
            PerformSelectedActions();
        

    }

    public override void UpdateDropping() {


        Action act = actionController.currentAction;

        // Start Drag
        if (Input.GetMouseButtonDown(0))
            dragStartPosition = touchCoords;


        //UPDATE TOOLTIP
        //if painting terrain, tooltip is terrain type
        if (act.Do == "paint")
            tooltipController.SetText(act.What);

        else if (modeController.currentMode == Mode.Construct && act.Do == "place")
            tooltipController.SetText(StructureDatabase.structureData[act.What].BaseCost + " shql.");

        // When mouse is clicked, drop a single building
        if (Input.GetMouseButtonUp(0)) {

            int x = Mathf.FloorToInt(touchCoords.x);
            int y = Mathf.FloorToInt(touchCoords.z);

            Dictionary<Node, Action> locs = new Dictionary<Node, Action>();
            locs.Add(new Node(x, y), act);

            actionController.actionLocations = locs;
            PerformSelectedActions();

        }
    }
    
}
