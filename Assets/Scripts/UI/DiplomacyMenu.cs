using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DiplomacyMenu : MonoBehaviour {
    
    public DiplomacyController diplomacyController;
    public TradeController tradeController;

    public MenuController cityMenu;
    public GameObject cityPage;
    public Text cityName;

    City CurrentCity { get; set; }
    List<ItemOrder> exports;
    List<ItemOrder> imports;

    public void OpenTrade(ItemOrder io) {

        if (CurrentCity == null)
            return;

        if (tradeController.TradeOrders.ContainsKey(io))
            return;

        tradeController.TradeOrders.Add(io, false);

    }

    public void CloseTrade(ItemOrder io) {

        if (CurrentCity == null)
            return;

        tradeController.TradeOrders.Remove(io);

    }

    public void OpenCity(City c) {

        CurrentCity = c;

        if (c == null)
            return;

        cityName.text = CurrentCity.name;
        cityMenu.OpenMenu(cityPage);

        exports = c.GetPossibleExports();
        imports = c.GetPossibleImports();

    }
}
