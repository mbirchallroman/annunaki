using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour {

    public Action act;
    public ActionController actionController;
    public Image img;
    public Text desc;
    public Text price;

    public void Start() {

        if (StructureDatabase.HasData(act.What) && desc != null) {

            StructureData s = StructureDatabase.GetData(act.What);
            desc.text = s.DisplayName;
            price.text = StructureDatabase.GetModifiedPrice(act.What) + " shql.";

            Sprite i = Resources.Load<Sprite>("ActionImgs/" + act.What);

            if (i != null)
                img.sprite = i;
            else
                Debug.Log(act.What + " has no image!");

        }

    }

    public void SetAction() {
        
        actionController.SetAction(act);
        actionController.constructionController.CloseMenu();
        actionController.editController.CloseMenu();

    }

}
