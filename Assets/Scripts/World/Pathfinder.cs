using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pathfinder {

    public World map;

    public Pathfinder(World w) {

        map = w;

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

                float tempGscore = current.gScore + 1;

                if (neighbor.DistanceTo(current) > 1)
                    tempGscore += neighbor.sqrt2;

                if (open.Contains(neighbor))
                    if (gscores[neighbor] <= tempGscore)
                        continue;
                    else
                        open.Remove(neighbor);

                neighbor.gScore = tempGscore;
                neighbor.fScore = tempGscore + neighbor.DistanceTo(goal);
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
                if ((a != 0 && b != 0) || (a == 0 && b == 0))
                    continue;

                else if (CanGoTo(a + currx, b + curry))
                    neighbors.Add(new Node(a + currx, b + curry));
            }

        return neighbors;
    }

    public bool CanGoTo(int a, int b) {

        //if out of bounds, can't go
        if (map.OutOfBounds(a, b))
            return false;

        //if there's a building at coordinates
        else if (!string.IsNullOrEmpty(map.GetBuildingNameAt(a, b))) {

            if (map.GetBuildingNameAt(a, b).Contains("MapEntrance") || map.GetBuildingNameAt(a, b).Contains("MapExit") || map.GetBuildingNameAt(a, b).Contains("House") || map.GetBuildingNameAt(a, b).Contains("EmptyLot") || map.GetBuildingNameAt(a, b).Contains("Rubble"))
                return true;

            //if there's a building but not a road, can't go
            else if (!map.IsRoadAt(a, b))
                return false;

        }

        //return false if there's water
        return map.terrain[a, b] != (int)Terrain.Water;

    }

}
