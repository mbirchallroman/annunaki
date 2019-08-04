using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjSave {

    public Node location;
    public Node3d position, rotation;
    public string resourcePath, name, tag;
    public bool rightclick;

    public ObjSave(GameObject go) {
        Vector3 pos = go.transform.position;
        Vector3 rot = go.transform.eulerAngles;

        position = new Node3d(pos);
        rotation = new Node3d(rot);

        tag = go.tag;
        name = go.name;

        Obj mo = go.GetComponent<Obj>();

        location = new Node(mo.X, mo.Y);

        resourcePath = mo.resourcePath;
        rightclick = mo.rightclick;

    }


}

public class Obj : MonoBehaviour {

    [Header("Object")]
    public string DisplayName;
    public string resourcePath;
    public int radiusOfInfluence;
    public bool rightclick;
	public int X { get; set; }
    public int Y { get; set; }
    
    public WorldController world { get; set; }
    public ModeController mode { get; set; }
    public TimeController time { get; set; }
    public LaborController labor { get; set; }
    public MoneyController money { get; set; }
    public ImmigrationController immigration { get; set; }
    public UIObjectDatabase UIObjects { get; set; }
    public TradeController trade { get; set; }
    public ScenarioController scenario { get; set; }
    public NotificationController notifications { get; set; }

    public virtual void VisitBuilding(int a, int b) {

        

    }

    private void Awake() {

        if (GetComponent<Obj>() != this)
            Debug.LogError("This has multiple obj components!");

    }

    //ADD TO SAVE
    public float TimeDelta { get; set; }

    public virtual void Activate() {

        LoadControllers();

    }

    public virtual void Load(ObjSave o) {

        transform.position = o.position.GetVector3();
        transform.eulerAngles = o.rotation.GetVector3();

        LoadControllers();

        X = (int)o.location.X;
        Y = (int)o.location.Y;
        tag = o.tag;
        name = o.name;
        rightclick = o.rightclick;

    }

    public void LoadControllers() {

        mode = world.modeController;
        time = world.timeController;
        labor = world.labor;
        immigration = world.immigration;
        money = world.money;
        trade = world.trade;
        UIObjects = world.uiObjectDatabase;
        notifications = world.notifications;
        scenario = world.scenario;

    }

    public void OpenWindow(GameObject g) {

        if (world.objWindow != null)
            world.objWindow.Close();

        //instantiate object
        GameObject go = Instantiate(g);
        go.transform.SetParent(GameObject.Find("OpenedMenus").transform);
        go.transform.SetAsFirstSibling();
        go.name = "ObjWindow";

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.localScale = new Vector3(1, 1, 1);

        //window vars
        ObjWindow objWindow = go.GetComponent<ObjWindow>();
        objWindow.obj = this;
        objWindow.WorldController = world;
        world.objWindow = objWindow;

        //open window
        objWindow.Open();

    }

    public virtual void OpenWindow() {

        OpenWindow(UIObjects.objWindow);

    }

    public Node FindClosestFire() {

        Pathfinder pathfinder = world.Map.pathfinder;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Fire");
        Node pos = new Node(this);

        SimplePriorityQueue<Structure, float> fires = new SimplePriorityQueue<Structure, float>();
        foreach(GameObject go in objs) {

            Structure s = go.GetComponent<Structure>();
            float distance = pos.DistanceTo(new Node(s));
            fires.Enqueue(s, distance);

        }

        int count = 5;
        foreach(Structure s in fires) {

            count--;
            if (count == 0)
                break;

            Node f = new Node(s);

            List<Node> path = pathfinder.FindPath(pos, f);
            if (path.Count == 0 )
                continue;
            
            return f;

        }

        return null;

    }

    

}
