using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class PricesDatabase : MonoBehaviour {

    public TextAsset database;

    public static Dictionary<string, float> Prices = new Dictionary<string, float>();

	// Use this for initialization
	void Awake () {

        ReadItemsFromDatabase();
		
	}

    public static int GetBasePrice(ItemOrder io) {

        string item = io.GetItemName();
        float price = 1;
        if (Prices.ContainsKey(item))
            price = Prices[item];
        return (int)(price * io.amount);

    }

    void ReadItemsFromDatabase() {

        XmlDocument doc = new XmlDocument();

        using (StringReader s = new StringReader(database.text)) {
            doc.Load(s);
        }

        XmlNodeList struList = doc.GetElementsByTagName("Price");

        foreach (XmlNode stru in struList) {

            XmlNodeList children = stru.ChildNodes;
            string item = null;
            float price = 0;

            foreach (XmlNode thing in children)
                if (thing.Name == "Item")
                    item = thing.InnerText;
                else if(thing.Name == "Money")
                    price = float.Parse(thing.InnerText);

            if (!string.IsNullOrEmpty(item) && price != 0)
                Prices[item] = price;
            

        }

    }
	

}
