using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneratorSave : WorkplaceSave {

    public int timeToProduce;
    public bool producing;

    public GeneratorSave(GameObject go) : base(go) {

        Generator g = go.GetComponent<Generator>();
        
        timeToProduce = g.TimeToProduce;
        producing = g.Producing;

    }

}


public class Generator : Workplace {

    //SERIALIZE ALL OF THIS
    [Header("Generator")]
    public int amountProduced;
    public string item;
    public int timeToProduceMax;
    public int TimeToProduce { get; set; }
    public ItemOrder Product { get; set; }
    public bool Producing { get; set; }

    public override void Load(ObjSave o) {
        base.Load(o);

        GeneratorSave g = (GeneratorSave)o;
        TimeToProduce = g.timeToProduce;
        SetProduct();
        Producing = g.producing;

    }

    public override void Activate() {

        base.Activate();

        //set product
        SetProduct();

        //start timer
        TimeToProduce = timeToProduceMax;
    }

    void SetProduct() {
        if (item != null && amountProduced > 0)
            Product = new ItemOrder(amountProduced, item);
    }

    public override void DoEveryDay() {

        base.DoEveryDay();

        if (this is Factory || this is CropField || this is Stable)
            return;

        //restart production if building is inactive or sending out cart
        if (HasEnoughStaff && !ActiveCarryerWalker)
            ProductionTimer();

    }

    public void ProductionTimer() {

        if (TimeToProduce > 0)
            TimeToProduce--;

        else
            ExportProduct();

    }

    public void ExportProduct() {

        if (!ActiveCarryerWalker) {

            StorageBuilding s = FindStorageBuildingToAccept(Product);
            if (s == null)
                return;

            SpawnGiver(Product, s);
            TimeToProduce = timeToProduceMax;
            Producing = false;

        }

    }

    public override void OpenWindow() {

        OpenWindow(UIObjects.generatorWindow);

    }

}
