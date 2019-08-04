using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class MarketSave: WorkplaceSave {

    public int[] Food;
    public int[] Goods;

    public MarketSave(GameObject go) : base(go) {

        Market m = go.GetComponent<Market>();
        Food = m.Food;
        Goods = m.Goods;

    }
}

public class Market : Workplace {

    public int[] Food;
    public int[] Goods;

    public int[] FoodDemand { get; set; }

    public override void Load(ObjSave o) {

        base.Load(o);

        MarketSave m = (MarketSave)o;
        Food = m.Food;
        Goods = m.Goods;

    }

    public override void Activate() {

        base.Activate();

        Food = new int[(int)FoodType.END];
        Goods = new int[(int)GoodType.END];
        //Food[0] = 1000;
        //Food[1] = 1000;

        //instantiate food demand list with some spots; key represents how many types, value represents how much of that type
        FoodDemand = new int[5];

    }

    public override void DoEveryWeek() {

        base.DoEveryWeek();

        if(!ActiveCarryerWalker)
            GetFood();

    }

    public int NumOfFoodsStored() {

        int s = 0;

        for (int b = 0; b < Food.Length; b++)
            if (Food[b] > 0)
                s++;

        return s;

    }

    public void SetFoodDemand(int[] houseDem) {

        int maxNumOfFoods = houseDem.Length;
        FoodDemand = new int[maxNumOfFoods];

        for (int n = 0; n < maxNumOfFoods; n++)
            for (int t = 0; t < n; t++)
                FoodDemand[t] += houseDem[n] / n;

    }

    void GetFood() {

        List<int> amounts = new List<int>();
        List<FoodType> types = new List<FoodType>();

        //get all the foods (types and amounts) the market has
        for(int z = 0; z < Food.Length; z++) {

            if (Food[z] == 0)
                continue;

            amounts.Add(Food[z]);
            types.Add((FoodType)z);

        }

        int[] am = amounts.ToArray();
        FoodType[] ft = types.ToArray();

        /*sort both arrays, with types being based off of amounts
          then reverse so it's big-to-small instead of small-to-big*/
        Array.Sort(am, ft);
        Array.Reverse(am);
        Array.Reverse(ft);

        amounts = new List<int>(am);
        types = new List<FoodType>(ft);

        int index = -1;
        float prcnt = 1;

        //go through the demand array
        for (int a = 0; a < FoodDemand.Length; a++) {

            int demand = FoodDemand[a];

            if (demand == 0)
                continue;

            //if the amount of foods we have is less than what's being demanded, look for that first
            if (a >= amounts.Count) {

                for(int b = 0; b < (int)FoodType.END; b++) {

                    //if we have this type of food already, don't keep going
                    if (types.Contains((FoodType)b))
                        continue;

                    ItemOrder ord = new ItemOrder(demand, b, ItemType.Food);

                    //find storage building that has this type of food
                    StorageBuilding storage = FindStorageBuildingThatHas(ord);

                    //if not this kind of food, look for another
                    if (storage == null)
                        continue;

                    //else, send cart there and close method
                    else {

                        SpawnGetter(ord, storage);
                        return;

                    }


                }

                return;

            }

            //get the amount of food that is demanded, the amount that's had, and the percent of have vs demand
            
            int have = amounts[a];
            float havePrcnt = (float)have / demand;

            //if we're missing more from this type, set it to the current
            if (havePrcnt < prcnt) {

                prcnt = havePrcnt;
                index = a;

            }

        }

        if (index == -1)
            return;

        int typeNeeded = (int)types[index];
        int amountNeeded = FoodDemand[index] - amounts[index];
        ItemOrder io = new ItemOrder(amountNeeded, typeNeeded, ItemType.Food);
        StorageBuilding sb = FindGranaryThatHas(amountNeeded, (int)typeNeeded);
        if (sb == null)
            return;
        SpawnGetter(io, sb);

    }



    public override void ReceiveItem(ItemOrder io) {

        if (io.type == ItemType.Food)
            Food[io.item] += io.amount;

        else if (io.type == ItemType.Good)
            Goods[io.item] += io.amount;

    }

}