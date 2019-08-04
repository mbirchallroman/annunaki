using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StorageBuildingSave : WorkplaceSave {
    
    public int[] inventory, queue;
    public float[] willAccept;
    public bool[] willGet, cantGiveToHouses;

    public StorageBuildingSave(GameObject go) : base(go) {

        StorageBuilding s = go.GetComponent<StorageBuilding>();
        
        inventory = s.Inventory;
        queue = s.Queue;
        willAccept = s.WillAccept;
        willGet = s.WillGet;
        cantGiveToHouses = s.CantGiveToHouses;

    }

}


public class StorageBuilding : Workplace {

    //implement WillGet system for individual buildings

    //SERIALIZE ALL THESE
    [Header("Storage Building")]
    public int storageSpace;
    public ItemType typeStored;
    public int[] Inventory { get; set; }
    public int[] Queue { get; set; }
    public float[] WillAccept { get; set; }
    public bool[] WillGet { get; set; }
    public bool[] CantGiveToHouses { get; set; }

    public List<StorageBlock> storageBlocks;

    public override void Load(ObjSave o) {
        base.Load(o);

        StorageBuildingSave s = (StorageBuildingSave)o;
        Inventory = s.inventory;
        Queue = s.queue;
        WillAccept = s.willAccept;
        WillGet = s.willGet;
        CantGiveToHouses = s.cantGiveToHouses;
        

    }

    public override void DoEveryDay() {

        base.DoEveryDay();
        GetOrRemove();

    }

    public void GetOrRemove() {

        //count through inventory so long as carryerwalker isn't active
        for (int a = 0; a < NumOfTotalTypes && !ActiveCarryerWalker; a++) {

            //only keep going if WillGet[a] is true
            if (!WillGet[a])
                continue;

            //if empty space, find other storage with stuff
            if (EmptySpaceFor(a) > 0) {

                ItemOrder io = new ItemOrder(EmptySpaceFor(a), a, typeStored);

                //if building is found, send getter
                StorageBuilding sb = FindStorageBuildingThatHas(io);
                if (sb != null)
                    SpawnGetter(io, sb);

            }

            //else find storage to accept surplus
            else {

                int amountToRemove = (int)(Inventory[a] - (storageSpace * WillAccept[a]));
                if (amountToRemove == 0)
                    continue;

                ItemOrder io = new ItemOrder(amountToRemove, a, typeStored);

                //if building is found, remove stuff from inventory and send giver
                StorageBuilding sb = FindStorageBuildingToAccept(io);
                if (sb != null) {

                    Inventory[a] -= amountToRemove;
                    SpawnGiver(io, sb);
                    UpdateVisibleGoods();

                }


            }
        }

    }

    public override void Activate() {

        base.Activate();

        Inventory = new int[NumOfTotalTypes];
        Queue = new int[NumOfTotalTypes];

        WillAccept = new float[NumOfTotalTypes];
        //set all types to accept all
        for (int a = 0; a < NumOfTotalTypes; a++)
            WillAccept[a] = 1;

        WillGet = new bool[NumOfTotalTypes];
        CantGiveToHouses = new bool[NumOfTotalTypes];

    }

    public int NumOfTotalTypes {
        get {

            if (typeStored == ItemType.Food)
                return (int)FoodType.END;

            if (typeStored == ItemType.Good)
                return (int)GoodType.END;

            if (typeStored == ItemType.Resource)
                return (int)ResourceType.END;

            return 0;
        }
    }

    //amount of overall empty space, both for food currently present and food about to be stored
    public int EmptySpace { get { return storageSpace - AmountStored() - AmountQueued(); } }

    //empty space for a particular type, both currently and futurely stored
    public int EmptySpaceFor(int a) {

        //actual space left, which is the max it will accept times the total potential space minus the amount stored
        int space = (int)(WillAccept[a] * storageSpace) - Inventory[a] - Queue[a];

        //if cannot accept type at all, return 0
        if (WillAccept[a] == 0)
            return 0;

        //if the total potential space is less than what could be stored individually, return the total potential space
        if (EmptySpace < space)
            return EmptySpace;

        //otherwise return specifically for this type
        return space;

    }

    //sums up total inventory
    public int AmountStored() {
        int sum = 0;
        foreach (int i in Inventory)
            sum += i;
        return sum;
    }

    //sums up total queue
    public int AmountQueued() {
        int sum = 0;
        foreach (int i in Queue)
            sum += i;
        return sum;
    }

    //how much of a type will this building accept
    public bool CanAcceptAmount(int num, int ft) {

        //if it can't accept it at all, return false
        if (WillAccept[ft] == 0)
            return false;

        //else return if currently stored, futurely stored, and the num to be added is less than the storagespace types what it will accept
        return Inventory[ft] + Queue[ft] + num <= storageSpace * WillAccept[ft];

    }

    public int NumOfStoredTypes() {

        int s = 0;

        for (int b = 0; b < Inventory.Length; b++)
            if (Inventory[b] > 0)
                s++;

        return s;

    }

    public int AvgAmountStored() {

        int s = 0;

        for (int b = 0; b < Inventory.Length; b++)
            s += Inventory[b];

        return s / NumOfStoredTypes();

    }
    
    public override void ReceiveItem(ItemOrder io) {

        //if does not accept this type of item, reject
        if (io.type != typeStored)
            return;

        //else remove from queue and add to inventory
        Queue[io.item] -= io.amount;
        Inventory[io.item] += io.amount;
        UpdateVisibleGoods();
    }

    public override void OpenWindow() {

        OpenWindow(UIObjects.storageWindow);

    }

    public void UpdateVisibleGoods() {

        int totalBlocks = storageBlocks.Count;
        int blockIndex = 0;

        //go through inventory
        for(int i = 0; i < Inventory.Length; i++) {

            //calculate percentage and how many blocks to take up
            int stored = Inventory[i];
            float percent = (float)stored / storageSpace;
            int numOfBlocks = (int)(percent * totalBlocks);

            for(int j = 0; j < numOfBlocks; j++) {

                if (blockIndex >= totalBlocks)
                    Debug.LogError("Storage block index is greater than the number of storage blocks " + name + " has.");
                storageBlocks[blockIndex].SetState(i);
                blockIndex++;

            }
            

        }

    }

}
