using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VenderSave : WalkerSave {

    public int[] FoodDemand { get; set; }
    
    public VenderSave(GameObject go) : base(go) {

        Vender v = go.GetComponent<Vender>();
        FoodDemand = v.FoodDemand;

    }

}

public class Vender : RandomWalker {

    public int[] FoodDemand { get; set; }

    public override void Load(ObjSave o) {

        base.Load(o);

        VenderSave v = (VenderSave)o;
        FoodDemand = v.FoodDemand;

    }

    public override void Activate() {

        base.Activate();

        //instantiate food demand list with some spots; key represents how many types, value represents how much of that type
        FoodDemand = new int[5];

    }

    public override void VisitBuilding(int a, int b) {

        base.VisitBuilding(a, b);

        House h = world.Map.GetBuildingAt(a, b).GetComponent<House>();
        if (h == null)
            return;

        if (VisitedSpots.Contains(h.name))
            return;
        else
            VisitedSpots.Add(h.name);

        SetDemand(h);
        GiveFoodTo(h);

    }

    void SetDemand(House h) {

        //add demand for food; if food wanted is different from food needed, add demand for desired food as well
        if (h.foodTypesNeeded != 0)
            FoodDemand[h.foodTypesNeeded] += h.FoodMax;
        if (h.foodTypesWant != h.foodTypesNeeded)
            FoodDemand[h.foodTypesWant] += h.FoodMax;

    }

    void GiveFoodTo(House h) {

        Market m = (Market)Origin;
        int foodTypesWant = h.foodTypesWant;
        int foodTypesHave = m.NumOfFoodsStored();

        //if only have the food it NEEDS (not wants), give that
        if (foodTypesHave < foodTypesWant)
            foodTypesWant = foodTypesHave;

        //if it doesn't want any food or we don't have any, don't keep going
        if (foodTypesWant == 0)
            return;

        //how many foods to give, and how much of each
        int foodToGive = h.FoodMax / foodTypesWant;
        int timesToGive = foodTypesWant;

        //go through food inventory; if have food at all, give that food
        for (int b = 0; b < (int)FoodType.END && timesToGive > 0; b++) {

            //if no food of this type, don't go
            if (m.Food[b] == 0)
                continue;

            //if stored food is more than needed
            if(m.Food[b] >= foodToGive) {
                h.Food[b] += foodToGive;
                m.Food[b] -= foodToGive;
            }
            //else if stored food is less than needed
            else {
                h.Food[b] += m.Food[b];
                m.Food[b] = 0;
            }

            timesToGive--;

        }

    }

    public override void Kill() {

        base.Kill();

        //set fooddemand and (ADD) goodsdemand for home market
        if(Origin != null) {

            Market m = (Market)Origin;
            m.SetFoodDemand(FoodDemand);

        }

    }

}
