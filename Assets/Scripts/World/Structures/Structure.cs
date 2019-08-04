using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

[System.Serializable]
public class StructureSave : ObjSave {

    public int sizex, sizey, days, weeks;
    public bool activeRandomWalker, activeCarryerWalker, activeImmigrant, OnFire;
    public float fireCount, collapseCount, burnCount;

    public StructureSave(GameObject go) : base(go) {

        Structure s = go.GetComponent<Structure>();

        sizex = s.Sizex;
        sizey = s.Sizey;
        days = s.Days;
        weeks = s.Weeks;
        fireCount = s.FireCount;
        collapseCount = s.CollapseCount;
        burnCount = s.BurnCount;

        OnFire = s.OnFire;
        activeRandomWalker = s.ActiveRandomWalker;
        activeCarryerWalker = s.ActiveCarryerWalker;
        activeImmigrant = s.ActiveImmigrant;

    }
}


public class Structure : Obj {

    [Header("Structure")]
    public int collapseCountMax, fireCountMax;
    public bool canBeDemolished = true;
    public bool radiusActive;

    public int Sizex { get; set; }
    public int Sizey { get; set; }
    public int Days { get; set; }
    public int Weeks { get; set; }

    public float FireCount { get; set; }
    public float CollapseCount { get; set; }
    public float BurnCount { get; set; }

    //stuff for spawning random walkers
    public bool ActiveRandomWalker { get; set; }
    public bool ActiveCarryerWalker { get; set; }
    public bool ActiveImmigrant { get; set; }
    public bool OnFire { get; set; }
    public string RandomWalker;

    public override void Activate() {
        base.Activate();
        transform.parent = world.structures.transform;
        ActiveRandomWalker = false;
    }

    public override void Load(ObjSave o) {
        base.Load(o);

        StructureSave s = (StructureSave)o;
        
        Sizex = s.sizex;
        Sizey = s.sizey;
        Days = s.days;
        Weeks = s.weeks;
        FireCount = s.fireCount;
        CollapseCount = s.collapseCount;
        BurnCount = s.burnCount;

        ActiveRandomWalker = s.activeRandomWalker;
        ActiveCarryerWalker = s.activeCarryerWalker;
        ActiveImmigrant = s.activeImmigrant;
        OnFire = s.OnFire;

        world.Map.RenameArea(name, X, Y, Sizex, Sizey);
    }

    private void Update() {

        TimeDelta += Time.deltaTime;
        if (time == null)
            return;
        if(TimeDelta >= TimeController.DayTime) {

            TimeDelta = 0;
            Days++;
            DoEveryDay();

            if (Days >= TimeController.WeekTime) {

                Days = 0;
                Weeks++;
                DoEveryWeek();

            }

            //convert weeks to month
            if (Weeks >= TimeController.MonthTime) {

                Weeks = 0;
                DoEveryMonth();

            }

        }

    }

    public void OnDestroy() {

        UponDestruction();

    }

    public virtual void UponDestruction() {

        if (world != null) {
            if (!world.PlaceOnRoads) {

                for (int a = X; a < X + Sizex; a++)
                    for (int b = Y; b < Y + Sizey; b++) {
                        world.Map.roads[a, b] = 0;
                    }

            }

            else
                world.PlaceOnRoads = false;
        }

    }

    public virtual void DoEveryDay() {

        //Debug.Log("does this every day");

    }

    public virtual void DoEveryWeek() {

        //Debug.Log("does this every week");

        if (radiusActive)
            VisitBuildings();

    }

    public virtual void DoEveryMonth() {

        //Debug.Log("does this every month");
        BurnCounter();
        FireCounter();
        CollapseCounter();

    }

    public List<Node> CheckAdjRoads() {

        List<Node> tiles = new List<Node>();
        World map = world.Map;

        //this demon magic works, don't think about it too much but it just checks surrounding tiles for roads
        for (int a = X; a < X + Sizex; a++) {
            if (map.OutOfBounds(a, Y - 1))
                continue;
            if (map.IsUnblockedRoadAt(a, Y - 1)) {
                tiles.Add(new Node(a, Y - 1));
            }
        }

        for (int b = Y; b < Y + Sizey; b++) {
            if (map.OutOfBounds(X + Sizex, b))
                continue;
            if (map.IsUnblockedRoadAt(X + Sizex, b)) {
                tiles.Add(new Node(X + Sizex, b));
            }
        }

        for (int a = X; a < X + Sizex; a++) {
            if (map.OutOfBounds(a, Y + Sizey))
                continue;
            if (map.IsUnblockedRoadAt(a, Y + Sizey)) {
                tiles.Add(new Node(a, Y + Sizey));
            }
        }

        for (int b = Y; b < Y + Sizey; b++) {
            if (map.OutOfBounds(X - 1, b))
                continue;
            if (map.IsUnblockedRoadAt(X - 1, b)) {
                tiles.Add(new Node(X - 1, b));
            }
        }

        if (map.IsUnblockedRoadAt(X, Y) && Sizex == 1 && Sizey == 1)
            tiles.Add(new Node(X, Y));

        return tiles;
    }

    public List<Node> CheckAdjGround() {

        List<Node> tiles = new List<Node>();
        World map = world.Map;
        
        for (int a = X; a < X + Sizex; a++) {
            if (map.OutOfBounds(a, Y - 1))
                continue;
            if (map.terrain[a, Y - 1] != (int)Terrain.Water && string.IsNullOrEmpty(map.structures[a, Y - 1])) {
                tiles.Add(new Node(a, Y - 1));
            }
        }

        for (int b = Y; b < Y + Sizey; b++) {
            if (map.OutOfBounds(X + Sizex, b))
                continue;
            if (map.terrain[X + Sizex, b] != (int)Terrain.Water && string.IsNullOrEmpty(map.structures[X + Sizex, b])) {
                tiles.Add(new Node(X + Sizex, b));
            }
        }

        for (int a = X; a < X + Sizex; a++) {
            if (map.OutOfBounds(a, Y + Sizey))
                continue;
            if (map.terrain[a, Y + Sizey] != (int)Terrain.Water && string.IsNullOrEmpty(map.structures[a, Y + Sizey])) {
                tiles.Add(new Node(a, Y + Sizey));
            }
        }

        for (int b = Y; b < Y + Sizey; b++) {
            if (map.OutOfBounds(X - 1, b))
                continue;
            if (map.terrain[X - 1, b] != (int)Terrain.Water && string.IsNullOrEmpty(map.structures[X - 1, b])) {
                tiles.Add(new Node(X - 1, b));
            }
        }

        return tiles;
    }

    public List<Node> CheckAdjWater() {

        List<Node> tiles = new List<Node>();
        World map = world.Map;

        for (int a = X; a < X + Sizex; a++) {
            if (map.OutOfBounds(a, Y - 1))
                continue;
            if (map.terrain[a, Y - 1] == (int)Terrain.Water && string.IsNullOrEmpty(map.structures[a, Y - 1])) {
                tiles.Add(new Node(a, Y - 1));
            }
        }

        for (int b = Y; b < Y + Sizey; b++) {
            if (map.OutOfBounds(X + Sizex, b))
                continue;
            if (map.terrain[X + Sizex, b] == (int)Terrain.Water && string.IsNullOrEmpty(map.structures[X + Sizex, b])) {
                tiles.Add(new Node(X + Sizex, b));
            }
        }

        for (int a = X; a < X + Sizex; a++) {
            if (map.OutOfBounds(a, Y + Sizey))
                continue;
            if (map.terrain[a, Y + Sizey] == (int)Terrain.Water && string.IsNullOrEmpty(map.structures[a, Y + Sizey])) {
                tiles.Add(new Node(a, Y + Sizey));
            }
        }

        for (int b = Y; b < Y + Sizey; b++) {
            if (map.OutOfBounds(X - 1, b))
                continue;
            if (map.terrain[X - 1, b] == (int)Terrain.Water && string.IsNullOrEmpty(map.structures[X - 1, b])) {
                tiles.Add(new Node(X - 1, b));
            }
        }

        return tiles;
    }

    public void SpawnRandomWalker() {

        //spawn if no active walker and the building has a walker
        if (!string.IsNullOrEmpty(RandomWalker)) {

            List<Node> entrances = CheckAdjRoads();

            //proceed only if there are available roads
            if (entrances.Count == 0)
                return;

            GameObject go = Instantiate(Resources.Load<GameObject>("Walkers/RandomWalkers/" + RandomWalker));
            go.transform.position = entrances[0].GetVector3();

            Walker w = go.GetComponent<Walker>();
            w.world = world;
            w.Origin = this;
            w.Activate();
        }

    }

    public virtual bool AllowWalkerIn(Walker w) {

        return false;

    }

    public void FireCounter() {

        if (fireCountMax == 0 || OnFire)
            return;

        FireCount -= Difficulty.GetModifier();

        if (FireCount <= 0)
            CatchOnFire();

    }

    public void CatchOnFire() {

        //BurnCount = 2;
        //OnFire = true;

        world.Demolish(X, Y);
        for (int a = X; a < X + Sizex; a++)
            for (int b = Y; b < Y + Sizey; b++) {
                world.SpawnStructure("RubbleOnFire", a, b, 0);
            }

        Notification n = new Notification(NotificationType.Issue, DisplayName + " caught on fire!");
        notifications.NewNotification(n);

    }

    public void BurnCounter() {

        if (!OnFire)
            return;

        BurnCount -= Difficulty.GetModifier();

        if (BurnCount <= 0)
            BurnDown();

    }

    public void BurnDown() {

        Notification n = new Notification(NotificationType.Issue, DisplayName + " burned down!");
        notifications.NewNotification(n);
        TurnToRubble();

    }

    public void CollapseCounter() {

        if (collapseCountMax == 0 || OnFire)
            return;

        CollapseCount -= Difficulty.GetModifier();

        if (CollapseCount <= 0)
            CollapseStructure();

    }

    public void CollapseStructure() {
        
        Notification n = new Notification(NotificationType.Issue, DisplayName + " collapsed!");
        notifications.NewNotification(n);

        TurnToRubble();
        
    }

    public void TurnToRubble() {

        world.Demolish(X, Y);
        for (int a = X; a < X + Sizex; a++)
            for (int b = Y; b < Y + Sizey; b++) {
                world.SpawnStructure("Rubble", a, b, 0);
            }

    }
    
    public virtual void ReceiveItem(ItemOrder io) { Debug.LogError(name + " received an item (" + io.ToString() + ") but doesn't know what to do with it"); }

    public StorageBuilding FindStorageBuildingToAccept(int num, int item, ItemType type) {

        GameObject[] objs = GameObject.FindGameObjectsWithTag("StorageBuilding");

        if (objs.Length == 0)
            return null;

        StorageBuilding destination = null;

        SimplePriorityQueue<StorageBuilding> queue = new SimplePriorityQueue<StorageBuilding>();

        foreach (GameObject go in objs) {

            StorageBuilding strg = go.GetComponent<StorageBuilding>();

            //only add to list if it stores this type
            if (strg.typeStored != type)
                continue;

            //only add to list if it has an entrance
            List<Node> entrancesHere = CheckAdjRoads();
            List<Node> entrancesThere = strg.CheckAdjRoads();
            if (entrancesHere.Count == 0 || entrancesThere.Count == 0)
                continue;

            //only add to list if it can accept amount
            if (!strg.CanAcceptAmount(num, item))
                continue;

            float distance = entrancesHere[0].DistanceTo(entrancesThere[0]);

            queue.Enqueue(strg, distance);

        }

        int numChecking = 0;

        foreach (StorageBuilding strg in queue) {

            //only check first five closest buildings
            numChecking++;
            if (numChecking == 5)
                break;

            List<Node> entrancesHere = CheckAdjRoads();
            List<Node> entrancesThere = strg.CheckAdjRoads();

            //find path to new place
            List<Node> path = world.Map.pathfinder.FindPath(entrancesHere[0], entrancesThere[0]);
            if (path.Count == 0)
                continue;
            else {

                destination = strg;
                break;

            }

        }

        return destination;

    }

    public StorageBuilding FindStorageBuildingThatHas(int num, int item, ItemType type) {

        GameObject[] objs = GameObject.FindGameObjectsWithTag("StorageBuilding");

        if (objs.Length == 0)
            return null;

        StorageBuilding destination = null;
        SimplePriorityQueue<StorageBuilding> queue = new SimplePriorityQueue<StorageBuilding>();

        int smallestAmountOfItem = 0;

        foreach (GameObject go in objs) {

            StorageBuilding strg = go.GetComponent<StorageBuilding>();
            if (this is StorageBuilding)
                if (strg == (StorageBuilding)this)
                    continue;

            //if storage building does not store that type of item, continue
            if (strg.typeStored != type)
                continue;

            //if storage building or this structure have no entrances, continue
            List<Node> entrancesHere = CheckAdjRoads();
            List<Node> entrancesThere = strg.CheckAdjRoads();
            if (entrancesHere.Count == 0 || entrancesThere.Count == 0)
                continue;

            if (strg.Inventory[item] == 0)
                continue;

            //if has less than the needed amount of Item and less than other discovered inventories, continue
            if (strg.Inventory[item] < num && strg.Inventory[item] < smallestAmountOfItem)
                continue;

            smallestAmountOfItem = strg.Inventory[item];
            float distance = entrancesHere[0].DistanceTo(entrancesThere[0]);

            queue.Enqueue(strg, distance);

        }

        int numChecking = 0;

        foreach (StorageBuilding strg in queue) {

            numChecking++;
            if (numChecking == 5)
                break;

            List<Node> entrancesHere = CheckAdjRoads();
            List<Node> entrancesThere = strg.CheckAdjRoads();

            //find path to new place
            List<Node> path = world.Map.pathfinder.FindPath(entrancesHere[0], entrancesThere[0]);
            if (path.Count == 0)
                continue;
            else
                destination = strg;

        }

        return destination;

    }

    public StorageBuilding FindStorageBuildingToAccept(ItemOrder io) { return FindStorageBuildingToAccept(io.amount, io.item, io.type); }
    public StorageBuilding FindGranaryToAccept(int num, int item) { return FindStorageBuildingToAccept(num, item, ItemType.Food); }
    public StorageBuilding FindWarehouseToAccept(int num, int item) { return FindStorageBuildingToAccept(num, item, ItemType.Good); }
    public StorageBuilding FindStorageYardToAccept(int num, int item) { return FindStorageBuildingToAccept(num, item, ItemType.Resource); }

    public StorageBuilding FindStorageBuildingThatHas(ItemOrder io) { return FindStorageBuildingThatHas(io.amount, io.item, io.type); }
    public StorageBuilding FindGranaryThatHas(int num, int item) { return FindStorageBuildingThatHas(num, item, ItemType.Food); }
    public StorageBuilding FindWarehouseThatHas(int num, int item) { return FindStorageBuildingThatHas(num, item, ItemType.Good); }
    public StorageBuilding FindStorageYardThatHas(int num, int item) { return FindStorageBuildingThatHas(num, item, ItemType.Resource); }

    public void SpawnGetter(ItemOrder io, Structure s) {

        Node start = CheckAdjRoads()[0];
        Node stop = s.CheckAdjRoads()[0];

        GameObject go = Instantiate(Resources.Load<GameObject>("Walkers/GetterCart"));
        go.transform.position = start.GetVector3();
        go.name = "CartFrom_" + name;

        Carryer c = go.GetComponent<Carryer>();
        c.world = world;
        c.Order = io;
        c.Origin = this;
        c.Destination = s;
        c.Path = c.FindPath(start, stop);
        c.Activate();

    }

    public void SpawnGiver(ItemOrder io, Structure s) {

        Node start = CheckAdjRoads()[0];
        Node stop = s.CheckAdjRoads()[0];
        s.GetComponent<StorageBuilding>().Queue[io.item] += io.amount;

        GameObject go = Instantiate(Resources.Load<GameObject>("Walkers/GiverCart"));
        go.transform.position = start.GetVector3();
        go.name = "CartFrom_" + name;

        Carryer c = go.GetComponent<Carryer>();
        c.world = world;
        c.Order = io;
        c.Origin = this;
        c.Destination = s;
        c.Path = c.FindPath(start, stop);
        c.Activate();

    }

    public void RequestImmigrant() {

        immigration.Requests.Add(this);

    }

    public virtual void ReceiveImmigrant() { Debug.LogError("Structure received a immigrant but doesn't know what to do with it"); }

    public void VisitBuildings() {

        for (int a = X - radiusOfInfluence; a <= X + Sizex + radiusOfInfluence; a++)
            for (int b = Y - radiusOfInfluence; b <= Y + Sizey + radiusOfInfluence; b++)
                if (world.Map.IsBuildingAt(a, b) && !world.Map.IsRoadAt(a, b)) {

                    if (world.Map.GetBuildingNameAt(a, b) == name)
                        continue;
                    VisitBuilding(a, b);

                }

    }

}
