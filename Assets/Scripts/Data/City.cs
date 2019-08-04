using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class City {

    public string name;
    public int attitude;
    public List<string> exports;
    public List<string> imports;
    public bool activeCaravan;
    //public List<ItemOrder> currentDeals;

    public City() { }

    public List<ItemOrder> GetPossibleExports() {

        List<ItemOrder> items = new List<ItemOrder>();

        foreach (string s in exports) {

            string[] data = s.Split(' ');
            if (data.Length != 2)
                Debug.LogError("Bad data for " + name + "'s exports.");
            int amount = int.Parse(data[0]);
            string item = data[1];
            items.Add(new ItemOrder(amount, item, this, TradeDirection.Export));

        }

        return items;

    }

    public List<ItemOrder> GetPossibleImports() {

        List<ItemOrder> items = new List<ItemOrder>();

        foreach (string s in imports) {

            string[] data = s.Split(' ');
            if (data.Length != 2)
                Debug.LogError("Bad data for " + name + "'s imports.");
            int amount = int.Parse(data[0]);
            string item = data[1];
            items.Add(new ItemOrder(amount, item, this, TradeDirection.Import));

        }

        return items;

    }

    public override string ToString() {

        return name + " " + attitude + " (Can sell " + imports.Count + " things)";

    }

}
