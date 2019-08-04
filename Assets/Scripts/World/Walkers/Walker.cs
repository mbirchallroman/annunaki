using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WalkerSave : ObjSave {

    public int prevx, prevy, laborPoints;
    public float movementDistance;
    public Node origin, destination;
    public Node3d direction;
    public bool stuck, returningHome;
    public List<Node> path;
    public List<string> visitedSpots;
    public ItemOrder order;

    public WalkerSave(GameObject go) : base(go) {

        Walker w = go.GetComponent<Walker>();

        //save origin and destination as coordinates
        Structure o = w.Origin;
        if (o != null)
            origin = new Node(o.X, o.Y);
        Structure d = w.Destination;
        if (d != null)
            destination = new Node(d.X, d.Y);

        prevx = w.Prevx;
        prevy = w.Prevy;
        laborPoints = w.LaborPoints;

        movementDistance = w.MovementDistance;
        
        stuck = w.Stuck;
        returningHome = w.ReturningHome;

        direction = new Node3d(w.Direction);
        order = w.Order;

        path = w.Path;
        visitedSpots = w.VisitedSpots;
    }
}

public class Walker : Obj {

    public float MovementDistance { get; set; }

    [Header("Walker")]
    public int lifeTime;
    public int Prevx { get; set; }
    public int Prevy { get; set; }
    public int LaborPoints { get; set; }

    public bool laborSeeker;
    public bool roadWalker;
    public bool canGoDiagonal;
    public bool canGoThroughHouses;
    public bool Stuck { get; set; }
    public bool ReturningHome { get; set; }

    public float MovementSpeed { get { return 1; } }
    public float MovementTime { get { return MovementDistance / MovementSpeed; } }

    public Structure Origin { get; set; }
    public Structure Destination { get; set; }

    public ItemOrder Order { get; set; }

    public List<Node> Path { get; set; }
    public List<string> VisitedSpots { get; set; }
    public Vector3 Direction { get; set; }

    public void Move() {
        if (!Stuck)
            transform.Translate(0, 0, Time.deltaTime * MovementSpeed);
    }

    public virtual void Kill() {
        Destroy(gameObject);
    }

    public override void Activate() {
        base.Activate();
        name = "WalkerFrom_" + Origin.name;
        transform.SetParent(world.walkers.transform);

        X = (int)transform.position.x;
        Y = (int)transform.position.z;

        Prevx = X;
        Prevy = Y;

        MovementDistance = 1;
        Stuck = true;

        VisitedSpots = new List<string>();

    }

    public override void Load(ObjSave o) {
        base.Load(o);

        WalkerSave w = (WalkerSave)o;

        //retrieve origin and destination structures
        if (w.origin != null)
            Origin = GameObject.Find(world.Map.GetBuildingNameAt(w.origin)).GetComponent<Structure>();
        if (w.destination != null)
            Destination = GameObject.Find(world.Map.GetBuildingNameAt(w.destination)).GetComponent<Structure>();
        
        Stuck = w.stuck;
        ReturningHome = w.returningHome;

        Prevx = w.prevx;
        Prevy = w.prevy;
        LaborPoints = w.laborPoints;

        MovementDistance = w.movementDistance;

        Path = w.path;
        VisitedSpots = w.visitedSpots;
        Direction = w.direction.GetVector3();
        Order = w.order;
    }

    public List<Node> FindPath(Node start, Node goal) {

        start.fScore = start.DistanceTo(goal);

        SimplePriorityQueue<Node> open = new SimplePriorityQueue<Node>();
        open.Enqueue(start, start.fScore);

        List<Node> closed = new List<Node>();

        Dictionary<Node, float> gscores = new Dictionary<Node, float>();


        while (open.Count != 0) {

            Node current = open.Dequeue();


            if (current.Equals(goal))
                return ReconstructPath(current);


            closed.Add(current);

            foreach (Node neighbor in Neighbors(current)) {

                if (closed.Contains(neighbor))
                    continue;

                float tempGscore = current.gScore + world.Map.TileCost(neighbor);

                if (neighbor.DistanceTo(current) > 1)
                    tempGscore += neighbor.sqrt2;

                if (open.Contains(neighbor))
                    if (gscores[neighbor] <= tempGscore)
                        continue;
                    else
                        open.Remove(neighbor);

                neighbor.gScore = tempGscore;
                neighbor.fScore = tempGscore + neighbor.DistanceTo(goal, true);
                neighbor.CameFrom = current;

                gscores[neighbor] = neighbor.gScore;
                open.Enqueue(neighbor, neighbor.fScore);
            }
        }

        return new List<Node>();
    }

    public List<Node> ReconstructPath(Node current) {
        List<Node> path = new List<Node>();
        path.Add(current);
        while (current.CameFrom != null) {
            current = current.CameFrom;
            path.Add(current);

        }
        return path;
    }

    public List<Node> Neighbors(Node current) {
        int currx = (int)current.X;
        int curry = (int)current.Y;

        List<Node> neighbors = new List<Node>();

        for (int a = -1; a <= 1; a++)
            for (int b = -1; b <= 1; b++) {

                //don't do current tile and don't do diagonal tiles if not traversable
                if ((a != 0 && b != 0 && !canGoDiagonal) || (a == 0 && b == 0))
                    continue;

                else if (CanGoTo(a + currx, b + curry))
                    neighbors.Add(new Node(a + currx, b + curry));
            }

        return neighbors;
    }

    public virtual bool CanGoTo(int a, int b) {

        World map = world.Map;

        //if out of bounds, can't go
        if (map.OutOfBounds(a, b))
            return false;

        //if road-only walker, check for roadblocks
        if (roadWalker) {

            if (map.roads[a, b] == 1) {
                if (this is RandomWalker && lifeTime > 0)
                    return map.GetBuildingAt(a, b).GetComponent<Structure>().AllowWalkerIn(this);
                else
                    return true;
            }

            return map.roads[a, b] == 2;
        }

        //ADD WHEN MAP ENTRANCES ARE ADDED
        else if (map.IsBuildingAt(a, b)) {

            if (map.GetBuildingNameAt(a, b).Contains("MapEntrance") || map.GetBuildingNameAt(a, b).Contains("MapExit"))
                return true;

            else if (canGoThroughHouses && (map.GetBuildingNameAt(a, b).Contains("EmptyLot") || map.GetBuildingNameAt(a, b).Contains("House")))
                return true;

            if (map.roads[a, b] > 0)
                return true;

            return false;

        }

        //return false if there's water
        return map.terrain[a, b] != (int)Terrain.Water;
    }

    public void UpdateRandomMovement() {

        transform.position = new Vector3(X, 0, Y);

        //create potential moves and delete last position if found in list
        List<Node> moves = Neighbors(new Node(X, Y));
        moves.Remove(new Node(Prevx, Prevy));

        //if there are possible moves forward, choose one at random
        if (moves.Count > 0) {
            int rand = Random.Range(0, moves.Count);
            Prevx = X;
            Prevy = Y;
            X = (int)moves[rand].X;
            Y = (int)moves[rand].Y;
        }

        //otherwise go backwards
        else if (CanGoTo(Prevx, Prevy)) {
            int tempx = X;
            int tempy = Y;
            X = Prevx;
            Y = Prevy;
            Prevx = tempx;
            Prevy = tempy;
        }

        Direction = new Vector3((X - Prevx), 0, (Y - Prevy));

        SetDirection();
    }

    public void UpdateAStarMovement() {

        int nextx = (int)Path[Path.Count - 1].X;
        int nexty = (int)Path[Path.Count - 1].Y;

        if (nextx == X && nexty == Y) {
            Path.RemoveAt(Path.Count - 1);

            if (Path.Count > 1)
                UpdateAStarMovement();
            else
                Stuck = true;
            return;
        }

        transform.position = new Vector3(X, 0, Y);
        Prevx = X;
        Prevy = Y;
        X = nextx;
        Y = nexty;

        Path.RemoveAt(Path.Count - 1);

        Direction = new Vector3((X - Prevx), 0, (Y - Prevy));

        SetDirection();
    }

    public void SetDirection() {
        MovementDistance = Mathf.Sqrt(Mathf.Abs(Direction.x) + Mathf.Abs(Direction.z));

        if (MovementDistance == 0)
            MovementDistance = 1;

        Stuck = false;

        if (Direction == new Vector3(1, 0, 0))
            transform.rotation = Quaternion.Euler(0, 90, 0);

        else if (Direction == new Vector3(1, 0, -1))
            transform.rotation = Quaternion.Euler(0, 135, 0);

        else if (Direction == new Vector3(0, 0, -1))
            transform.rotation = Quaternion.Euler(0, 180, 0);

        else if (Direction == new Vector3(-1, 0, -1))
            transform.rotation = Quaternion.Euler(0, 225, 0);

        else if (Direction == new Vector3(-1, 0, 0))
            transform.rotation = Quaternion.Euler(0, 270, 0);

        else if (Direction == new Vector3(-1, 0, 1))
            transform.rotation = Quaternion.Euler(0, 315, 0);

        else if (Direction == new Vector3(0, 0, 1))
            transform.rotation = Quaternion.Euler(0, 360, 0);

        else if (Direction == new Vector3(1, 0, 1))
            transform.rotation = Quaternion.Euler(0, 45, 0);

        else
            Stuck = true;
    }

    public virtual void ReturnHome() {

        List<Node> entrances = Origin.CheckAdjRoads();
        if (entrances.Count == 0) {
            Kill();
            return;
        }
        Path = FindPath(new Node(this), entrances[0]);

        if (Path.Count == 0)
            Kill();

        ReturningHome = true;
        Stuck = true;

    }

    public void LeaveMap() {

        Structure mapExit = GameObject.FindGameObjectWithTag("MapExit").GetComponent<Structure>();
        if (mapExit == null)
            Kill();

        Node start = new Node(this);
        Node end = new Node(mapExit);

        Path = FindPath(start, end);
        ReturningHome = true;
        Stuck = true;

    }
}
