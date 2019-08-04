using UnityEngine;

public class Road : TiledStructure {

    public override void Activate() {

        base.Activate();

        world.Map.roads[X, Y] = 2;

    }

    public new void Update() {

        UpdateTiling();

        if (world.Map.roads[X, Y] != 2)
            world.Map.roads[X, Y] = 2;

    }

    public override int FindNeighbors() {

        int n = 0;

        if (world.Map.IsRoadAt(X - 1, Y))
            n += 1;
        if (world.Map.IsRoadAt(X, Y + 1))
            n += 2;
        if (world.Map.IsRoadAt(X + 1, Y))
            n += 4;
        if (world.Map.IsRoadAt(X, Y - 1))
            n += 8;

        return n;

    }
    
}
