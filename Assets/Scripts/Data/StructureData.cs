using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class StructureData {

    public int Sizex { get; set; }
    public int Sizey { get; set; }
    public int BaseCost { get; set; }

    public float Alignx { get { return .5f * (Sizex - 1); } }
    public float Aligny { get { return .5f * (Sizey - 1); } }

    public bool CanDrag { get; set; }
    public bool HasWaterTiles { get; set; }
    public bool HasRoadTiles { get; set; }
    public bool BuiltOnce { get; set; }

    public string Name { get; set; }
    public string DisplayName { get; set; }
    public int[,] WaterTiles { get; set; }
    public int[,] RoadTiles { get; set; }

    public StructureData(Dictionary<string, string> contents) {

        Name = contents["Name"];
        DisplayName = contents["DisplayName"];

        Sizex = int.Parse(contents["Sizex"]);
        Sizey = int.Parse(contents["Sizey"]);
        BaseCost = int.Parse(contents["BaseCost"]);

        CanDrag = bool.Parse(contents["CanDrag"]);
        BuiltOnce = bool.Parse(contents["BuiltOnce"]);

        HasWaterTiles = bool.Parse(contents["HasWaterTiles"]);
        if (HasWaterTiles)
            LoadWaterTiles();

        HasRoadTiles = bool.Parse(contents["HasRoadTiles"]);
        if (HasRoadTiles)
            LoadRoadTiles();

    }

    public void LoadWaterTiles() {
        
        TextAsset file = Resources.Load<TextAsset>("BuildingData/Water/" + Name);
        WaterTiles = new int[Sizex, Sizey];

        int a = 0;
        int b = 0;

        foreach(string s in new List<string>(file.text.Split(' '))) {

            WaterTiles[a, b] = int.Parse(s);

            a++;
            if(a >= Sizex) {
                a = 0;
                b++;
            }
            if(b >= Sizey)
                break;

        }

    }

    public void LoadRoadTiles() {

        TextAsset file = Resources.Load<TextAsset>("BuildingData/Roads/" + Name);
        RoadTiles = new int[Sizex, Sizey];

        int a = 0;
        int b = 0;

        foreach (string s in new List<string>(file.text.Split(' '))) {

            RoadTiles[a, b] = int.Parse(s);

            a++;
            if (a >= Sizex) {
                a = 0;
                b++;
            }
            if (b >= Sizey)
                break;

        }


    }
}
