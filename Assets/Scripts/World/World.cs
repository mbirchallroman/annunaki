using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class World {

    public float frequency = 3;
    public float water = 20;
    public float lush = 10;
    public float grass = 10;
    public float mud = 10;
    public float sand = 50;

    public Node size;
    public int[,] terrain, roads;
    public string[,] structures;
    public Pathfinder pathfinder;

    public World(float x, float y) : this(new Node(x, y)) { }

    public World(Node sz) {

        size = sz;
        terrain = sz.CreateArrayOfSize<int>();
        roads = sz.CreateArrayOfSize<int>();
        structures = sz.CreateArrayOfSize<string>();
        pathfinder = new Pathfinder(this);

    }

    public void RandomizeTerrain() {

        float offset = Random.Range(0, 1000);

        for (int x = 0; x < size.X; x++)
            for (int y = 0; y < size.Y; y++) {

                float nx = x / size.X * frequency + offset;
                float ny = y / size.Y * frequency + offset;

                float noise = Mathf.PerlinNoise(nx, ny);

                noise *= 100;
                
                if(noise < water)
                    terrain[x,y] = 0;
                else if(noise < lush + water)
                    terrain[x, y] = 1;
                else if (noise < grass + lush + water)
                    terrain[x, y] = 2;
                else if (noise < mud + grass + lush + water)
                    terrain[x, y] = 3;
                else
                    terrain[x, y] = 4;

            }
                

        terrain[0, 0] = (int)Terrain.Grass;
        terrain[(int)size.X - 1, (int)size.Y - 1] = (int)Terrain.Grass;

    }

    public void PlainTerrain() {
        for (int x = 0; x < size.X; x++)
            for (int y = 0; y < size.Y; y++)
                terrain[x, y] = (int)Terrain.Grass;
    }

    public bool OutOfBounds(int x, int y) {
        return x < 0 || x >= size.X || y < 0 || y >= size.Y;
    }

    public bool OutOfBounds(int x, int y, int szx, int szy) {

        for (int x2 = x; x2 < szx + x; x2++) {
            for (int y2 = y; y2 < szy + y; y2++) {
                if (OutOfBounds(x2, y2))
                    return true;
            }

        }

        return false;
    }

    public void RenameArea(string s, int x, int y, int szx, int szy) {
        structures[x, y] = s;
        for (int a = x; a < szx + x; a++) {
            for (int b = y; b < szy + y; b++) {
                structures[a, b] = s;
            }
        }
    }

    void ClearArea(int x, int y, int szx, int szy) {
        RenameArea(null, x, y, szx, szy);
    }

    public bool IsRoadAt(int x, int y) {

        if (OutOfBounds(x, y))
            return false;

        return roads[x, y] > 0;

    }

    public bool IsUnblockedRoadAt(int x, int y) {

        if (OutOfBounds(x, y))
            return false;

        return roads[x, y] > 1;

    }

    public GameObject GetBuildingAt(int x, int y) {
        string n = GetBuildingNameAt(x, y);

        if (n == null)
            return null;

        return GameObject.Find(n);
    }

    public GameObject GetBuildingAt(Node n) {

        return GetBuildingAt((int)n.X, (int)n.Y);

    }

    public string GetBuildingNameAt(int x, int y){
        if(OutOfBounds(x,y))
            return "";

        return structures[x, y];
    }

    public string GetBuildingNameAt(Node n) {
        return GetBuildingNameAt((int)n.X, (int)n.Y);
    }

    public bool IsBuildingAt(int x, int y) {
        return !string.IsNullOrEmpty(GetBuildingNameAt(x, y));
    }

    public int TileCost(int x, int y) {
        if (IsRoadAt(x, y))
            return 1;
        return 5;
    }

    public int TileCost(Node n) {
        return TileCost((int)n.X, (int)n.Y);
    }
    
}
