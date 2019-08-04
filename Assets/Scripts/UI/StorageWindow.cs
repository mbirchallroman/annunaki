using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageWindow : WorkplaceWindow {

    public GameObject simpleInventoryGrid;
    public GameObject advancedInventoryGrid;
    public GameObject noItems;
    public UIObjectDatabase uiObjectDatabase;

    public override void Open() {

        base.Open();

        StorageBuilding sb = (StorageBuilding)obj;

        for(int a = 0; a < sb.NumOfTotalTypes; a++) {

            uiObjectDatabase = obj.UIObjects;

            //for simple grid
            if(sb.Inventory[a] > 0 && simpleInventoryGrid != null) {

                GameObject g1 = Instantiate(uiObjectDatabase.storageItem_smpl);
                g1.transform.SetParent(simpleInventoryGrid.transform);

                StorageItem_smpl s = g1.GetComponent<StorageItem_smpl>();
                s.sb = sb;
                s.index = a;

                noItems.SetActive(false);

            }

            //entry in advanced grid
            GameObject g2 = Instantiate(uiObjectDatabase.storageItem_adv);
            g2.transform.SetParent(advancedInventoryGrid.transform);

            StorageItem_adv si = g2.GetComponent<StorageItem_adv>();
            si.index = a;
            si.sb = sb;


        }

    }


}
