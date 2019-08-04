using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class StructureDatabase : MonoBehaviour {

    public TextAsset database;
    public static Dictionary<string, StructureData> structureData = new Dictionary<string, StructureData>();

    private List<Dictionary<string, string>> contentsList = new List<Dictionary<string, string>>();

    public void Awake() {
        Enums.LoadDictionaries();
        ReadItemsFromDatabase();

        for (int i = 0; i < contentsList.Count; i++) {

            StructureData newBuilding = new StructureData(contentsList[i]);

            structureData[newBuilding.Name] = newBuilding;

        }

    }

    public void ReadItemsFromDatabase() {
        XmlDocument doc = new XmlDocument();

        using (StringReader s = new StringReader(database.text)) {
            doc.Load(s);
        }

        XmlNodeList struList = doc.GetElementsByTagName("Structure");

        foreach (XmlNode stru in struList) {

            XmlNodeList children = stru.ChildNodes;
            Dictionary<string, string> contents = new Dictionary<string, string>();

            foreach (XmlNode thing in children)
                contents.Add(thing.Name, thing.InnerText);

            contentsList.Add(contents);

        }
    }

    public static int GetModifiedPrice(string s) {
        
        return Mathf.RoundToInt(GetBasePrice(s) * Difficulty.GetModifier());

    }

    public static int GetBasePrice(string s) {

        return structureData[s].BaseCost;

    }

    public static StructureData GetData(string s) {

        if (!structureData.ContainsKey(s))
            return null;

        return structureData[s];

    }

    public static bool HasData(string s) {

        return structureData.ContainsKey(s);

    }

}
