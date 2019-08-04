using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FactorySave : GeneratorSave {

    public int[] ingredientAmount;

    public FactorySave(GameObject go) : base(go) {

        Factory f = go.GetComponent<Factory>();
        
        ingredientAmount = f.IngredientAmount;

    }

}


public class Factory : Generator {
    
    [Header("Factory")]
    public int[] ingredientMax;
    public int[] IngredientAmount { get; set; }
    public string[] ingredients;

    public override void Load(ObjSave o) {

        base.Load(o);

        FactorySave f = (FactorySave)o;
        IngredientAmount = f.ingredientAmount;

    }

    public override void Activate() {

        base.Activate();

        IngredientAmount = new int[ingredientMax.Length];
        Producing = false;

    }

    public bool HaveEnough(int a) { return IngredientAmount[a] >= ingredientMax[a]; }
    public int IngredientNeeded(int a) { return ingredientMax[a] - IngredientAmount[a]; }

    public override void DoEveryDay() {

        base.DoEveryDay();

        if (HasEnoughStaff && !ActiveCarryerWalker) {

            if (Producing)
                ProductionTimer();

            else {

                if (HaveEnough())
                    BeginProduction();
                else
                    GetIngredients();

            }
                

        }

    }

    public void GetIngredients() {

        for(int a = 0; a < ingredientMax.Length; a++)
            if (!HaveEnough(a)) {

                ItemOrder io = new ItemOrder(IngredientNeeded(a), ingredients[a]);

                StorageBuilding sb = FindStorageBuildingThatHas(io);

                if(sb != null)
                    SpawnGetter(io, sb);

                return;

            }

    }

    public bool HaveEnough() {

        for (int a = 0; a < ingredientMax.Length; a++)
            if (!HaveEnough(a))
                return false;
        return true;

    }

    public void BeginProduction() {

        for (int a = 0; a < ingredientMax.Length; a++)
            IngredientAmount[a] -= ingredientMax[a];
        Producing = true;

    }

    public override void ReceiveItem(ItemOrder io) {

        for (int a = 0; a < ingredientMax.Length; a++)
            if (ingredients[a] == io.GetItemName())
                IngredientAmount[a] += io.amount;

    }
    

}
