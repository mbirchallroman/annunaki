using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorWindow : WorkplaceWindow {

    public Image clock;
    public Text productionLabel;

    public override void Open() {

        base.Open();

        Generator g = obj.GetComponent<Generator>();
        clock.fillAmount = (float)g.TimeToProduce / g.timeToProduceMax;

    }

    public override void DoDuringUpdate() {

        base.DoDuringUpdate();

        Generator g = obj.GetComponent<Generator>();

        if (!g.HasEnoughStaff)
            productionLabel.text = "This building is understaffed.";
        else if (g.ActiveCarryerWalker)
            productionLabel.text = "This building's cart is transporting goods.";
        else if (g.TimeToProduce > 0)
            productionLabel.text = "Production is " + (int)((float)g.TimeToProduce / g.timeToProduceMax * 100) + "% complete!";



    }

    

}
