using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour {

    public WorldController worldController;
    public DifficultyController difficultyController;
    public ModeController modeController;
    public MoneyController moneyController;
    public TooltipController tooltipController;
    public MenuController constructionController;
    public MenuController editController;

    public PlacementMenu placementMenu;

    public Action currentAction { get; set; }
    public int timeToUndoMax;
    public int TimeToUndo { get; set; }

    public Dictionary<Node, Action> actionLocations = new Dictionary<Node, Action>();
    public Dictionary<Node, Action> undoActions = new Dictionary<Node, Action>();

    private void Start() {

        currentAction = null;

    }

    float timeDelta = 0;

    public void Update() {

        timeDelta += Time.deltaTime;
        if (timeDelta >= 1) {

            timeDelta = 0;
            UndoCounter();

        }
            

        if (Input.GetMouseButton(1)) {
            currentAction = null;
            tooltipController.SetText("");
            constructionController.CloseMenu();
            editController.CloseMenu();
        }

        if (Input.GetKeyDown("space"))
            RotateStructure(90);
    }

    public float buildingRotation { get; set; }

    public void RotateStructure(float deg) {
        buildingRotation += deg;
        if (buildingRotation >= 360)
            buildingRotation = 0;
    }

    public void StopAction() {

        currentAction = null;
        tooltipController.SetText("");
        constructionController.CloseMenu();
        editController.CloseMenu();
        placementMenu.OpenClose();

    }

    //check if an action is possible
    public bool CanDo(Action act, int x, int y) {

        if (act.Do == "paint")
            return worldController.CanPaintTerrain(act.What, x, y);

        else if (act.Do == "place")
            return worldController.CanSpawnStructure(act.What, x, y, buildingRotation);

        else if (act.Do == "demolish" || act.Do == "unplace")
            return worldController.Map.GetBuildingAt(x, y) != null;

        else if (act.Do == "rebuild")
            return true;

        return false;
    }

    //do an action
    public void Do(Action act, int x, int y) {

        if (act.Do == "paint")
            worldController.PaintTerrain(act.What, x, y);

        else if (act.Do == "place")
            worldController.SpawnStructure(act.What, x, y, buildingRotation);

        else if (act.Do == "demolish" || act.Do == "unplace")
            worldController.Demolish(x, y);

        else if (act.Do == "rebuild")
            worldController.LoadMapObject(act.data);

    }

    public void PerformActions(Dictionary<Node, Action> acts) {

        undoActions = new Dictionary<Node, Action>();

        foreach (Node n in acts.Keys) {

            int x = (int)n.X;
            int y = (int)n.Y;

            Action a = acts[n];

            if (CanDo(a, x, y)) {

                undoActions.Add(n, ReverseAction(a, n));
                Do(a, x, y);

                if (modeController.currentMode == Mode.Construct && a.Do == "place")
                    moneyController.Sheqels -= StructureDatabase.structureData[a.What].BaseCost;

                else if(a.Do == "unplace")
                    moneyController.Sheqels += StructureDatabase.structureData[a.What].BaseCost;


            }

        }

    }

    public void PerformActions() {

        PerformActions(actionLocations);
        actionLocations = new Dictionary<Node, Action>();

    }

    public void UndoCounter() {

        if (undoActions.Count > 0)
            TimeToUndo--;

        else
            TimeToUndo = timeToUndoMax;

        if (TimeToUndo <= 0)
            undoActions = new Dictionary<Node, Action>();

    }

    public void UndoActions() {

        PerformActions(undoActions);
        undoActions = new Dictionary<Node, Action>();

    }

    public int GetPrice(Dictionary<Node, Action> acts, Action a) {

        int count = acts.Count;

        int singlePrice = StructureDatabase.GetModifiedPrice(a.What);
        int price = 0;

        if (count == 0)
            price = singlePrice;

        else
            price = singlePrice * count;

        return price;

    }
    
    //get preview object for an action
    public GameObject GetPreview(Action act) {

        if (act.Do == "paint" || act.Do == "demolish") {
            GameObject go = (GameObject)Instantiate(Resources.Load("Previews/Hover"));

            go.name = "preview";
            go.transform.SetParent(worldController.transform, true);

            return go;

        }

        if(act.Do == "place") {
            GameObject go = (GameObject)Instantiate(Resources.Load("Previews/" + act.What));

            go.name = "preview";
            go.transform.SetParent(worldController.transform, true);

            return go;
        }

        return null;
    }

    //check if action can be dragged
    public bool CanDrag(Action act) {

        if (act == null)
            return false;

        if (act.Do == "paint" || act.Do == "demolish")
            return true;

        else if(act.Do == "place") {

            StructureData data = StructureDatabase.structureData[act.What];
            return data.CanDrag;

        }

        return false;

    }

    public void SetAction(string s) {

        SetAction(new Action(s));

    }

    public void SetAction(Action a) {

        if (Time.timeScale == 0 && modeController.currentMode != Mode.Edit)
            return;
        currentAction = a;
        buildingRotation = 0;
        placementMenu.OpenClose();

    }

    public Action ReverseAction(Action a, Node n) {

        if (a.Do == "place")
            return new Action("unplace", a.What);

        else if (a.Do == "demolish")
            return new Action("rebuild", worldController.Map.GetBuildingAt(n));

        return null;

    }
    
}

[System.Serializable]
public class Action {

    public string Do;
    public string What;
    public StructureSave data;

    public Action(string a, string b) {
        Do = a;
        What = b;
    }

    public Action(string a, GameObject str) {

        Do = a;

        if (str.GetComponent<StorageBuilding>() != null)
            data = (new StorageBuildingSave(str));

        else if (str.GetComponent<Stable>() != null)
            data = (new StableSave(str));

        else if (str.GetComponent<Factory>() != null)
            data = (new FactorySave(str));

        else if (str.GetComponent<Generator>() != null)
            data = (new GeneratorSave(str));

        else if (str.GetComponent<Market>() != null)
            data = (new MarketSave(str.gameObject));

        else if (str.GetComponent<House>() != null)
            data = (new HouseSave(str.gameObject));

        else if (str.GetComponent<Workplace>() != null)
            data = (new WorkplaceSave(str.gameObject));

        else if (str.GetComponent<Structure>() != null)
            data = (new StructureSave(str.gameObject));

    }

    public Action(string a) {

        string[] s = a.Split(' ');
        if (s.Length != 2)
            return;
        Do = s[0];
        What = s[1];

    }

    public override string ToString() {
        return Do + " " + What;
    }

}