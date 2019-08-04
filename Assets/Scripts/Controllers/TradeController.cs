using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TradeSave {

    public DictContainer<ItemOrder, bool> tradeOrders;
    public int caravansThisMonth;
    public float timeDelta;

    public TradeSave(TradeController tc) {

        tradeOrders = new DictContainer<ItemOrder, bool>(tc.TradeOrders);
        caravansThisMonth = tc.caravansThisMonth;
        timeDelta = tc.TimeDelta;

    }

}

public class TradeController : MonoBehaviour {

    public Dictionary<ItemOrder, bool> TradeOrders { get; set; }
    public WorldController worldController;

    public int caravansThisMonth;
    public float CaravanRate { get { return (TimeController.WeekTime * TimeController.MonthTime) / caravansThisMonth; } }
    public float TimeDelta { get; set; }

    public void Load(TradeSave tc) {

        TradeOrders = tc.tradeOrders.GetDictionary();
        caravansThisMonth = tc.caravansThisMonth;
        TimeDelta = tc.timeDelta;

    }

	// Use this for initialization
	void Start () {

        TradeOrders = new Dictionary<ItemOrder, bool>();
		
	}

    private void Update() {

        TimeDelta += Time.deltaTime;

        if (TimeDelta >= CaravanRate) {

            TimeDelta = 0;
            NextCaravan();

        }

    }

    public void NextCaravan() {

        foreach(ItemOrder co in TradeOrders.Keys)
            if (!TradeOrders[co]) {

                if (co.city.attitude < 0) {
                    TradeOrders.Remove(co);
                    continue;
                }

                if (co.direction == TradeDirection.Export)
                    SpawnExporter(co);
                else
                    SpawnImporter(co);

                if (TradeOrders[co])
                    break;

            }

    }

    public void SpawnExporter(ItemOrder co) {

        Structure mapEntrance = GameObject.FindGameObjectWithTag("MapEntrance").GetComponent<Structure>();
        Structure target = mapEntrance.FindStorageBuildingThatHas(co);

        if (mapEntrance == null || target == null || co.direction != TradeDirection.Export)
            return;

        List<Node> entrances = target.CheckAdjRoads();
        if (entrances.Count == 0)
            return;

        Node start = new Node(mapEntrance.X, mapEntrance.Y);
        Node end = entrances[0];

        List<Node> testpath = worldController.Map.pathfinder.FindPath(start, end);
        if (testpath.Count == 0)
            return;

        GameObject go = Instantiate(Resources.Load<GameObject>("Walkers/Exporter"));
        go.transform.position = mapEntrance.transform.position;
        go.name = "CaravanTo_" + target.name;

        Walker w = go.GetComponent<Walker>();
        w.Order = co;
        w.world = worldController;
        w.Origin = mapEntrance;
        w.Destination = target;
        w.Path = w.FindPath(start, end);
        w.Activate();

    }

    public void SpawnImporter(ItemOrder co) {

        Structure mapEntrance = GameObject.FindGameObjectWithTag("MapEntrance").GetComponent<Structure>();
        Structure target = mapEntrance.FindStorageBuildingToAccept(co);

        if (mapEntrance == null || target == null || co.direction != TradeDirection.Import)
            return;

        List<Node> entrances = target.CheckAdjRoads();
        if (entrances.Count == 0)
            return;

        Node start = new Node(mapEntrance.X, mapEntrance.Y);
        Node end = entrances[0];
        target.GetComponent<StorageBuilding>().Queue[co.item] += co.amount;

        List<Node> testpath = worldController.Map.pathfinder.FindPath(start, end);
        if (testpath.Count == 0)
            return;

        GameObject go = Instantiate(Resources.Load<GameObject>("Walkers/Importer"));
        go.transform.position = mapEntrance.transform.position;
        go.name = "CaravanTo_" + target.name;

        Walker w = go.GetComponent<Walker>();
        w.Order = co;
        w.world = worldController;
        w.Origin = mapEntrance;
        w.Destination = target;
        w.Path = w.FindPath(start, end);
        w.Activate();

    }

    

}
