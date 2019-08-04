using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class CityDatabase : MonoBehaviour {

    public TextAsset database;
    public static Dictionary<string, City> cityData = new Dictionary<string, City>();

    private List<Dictionary<string, string>> contentsList = new List<Dictionary<string, string>>();

    //public void Awake() {
    //    Enums.LoadDictionaries();
    //    ReadItemsFromDatabase();

    //    for (int i = 0; i < contentsList.Count; i++) {

    //        City newCity = new City(contentsList[i]);
    //        cityData[newCity.name] = newCity;

    //    }

    //}

    public void ReadItemsFromDatabase() {
        XmlDocument doc = new XmlDocument();

        using (StringReader s = new StringReader(database.text)) {
            doc.Load(s);
        }

        XmlNodeList struList = doc.GetElementsByTagName("City");

        foreach (XmlNode stru in struList) {

            XmlNodeList children = stru.ChildNodes;
            Dictionary<string, string> contents = new Dictionary<string, string>();

            foreach (XmlNode thing in children)
                contents.Add(thing.Name, thing.InnerText);

            contentsList.Add(contents);

        }
    }
}