using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UsefulThings;

[System.Serializable]
public class HouseSave : StructureSave {
    
    public int Residents { get; set; }
    public int HouseSize { get; set; }
    public int Prosperity;
    
    public int[] Water { get; set; }
    public Quality WaterQual { get; set; }
    
    public int[] Food { get; set; }

    public int[] Goods { get; set; }

    public DictContainer<string, int> VenueAccess { get; set; }
    public int Culture;

    public DictContainer<string, int> Faith;

    public HouseSave(GameObject go) : base(go) {

        House h = go.GetComponent<House>();
        
        Residents = h.Residents;
        HouseSize = h.HouseSize;
        Prosperity = h.Prosperity;

        Water = h.Water;
        WaterQual = h.WaterQual;

        Food = h.Food;

        Goods = h.Goods;

        VenueAccess = new DictContainer<string, int>(h.VenueAccess);
        Culture = h.Culture;

        Faith = new DictContainer<string, int>(h.Faith);

    }

}

public class House : Structure {

    [Header("House")]
    public int Level;
	public int Residents { get; set; }
    public int ResidentsMax;
    public int HouseSize { get; set; }
    public int Prosperity;

    public string evolvesTo;
    public string devolvesTo;
    public string biggerHouse;

    public override void Load(ObjSave o) {

        base.Load(o);

        HouseSave h = (HouseSave)o;
        
        Residents = h.Residents;
        HouseSize = h.HouseSize;
        Prosperity = h.Prosperity;

        Water = h.Water;
        WaterQual = h.WaterQual;

        Food = h.Food;

        Goods = h.Goods;

        VenueAccess = h.VenueAccess.GetDictionary();
        Culture = h.Culture;

        Faith = h.Faith.GetDictionary();

    }

    public override void Activate() {

        base.Activate();

        float rotation = Random.Range(0, 360);
        transform.eulerAngles = new Vector3(0, rotation, 0);
        
        //add population to world
        labor.AddPopulation(Residents);
        scenario.AddHouseLevel((Level - 1) * HouseSize * HouseSize);
        
        //housesize is equal to the size of the structure
        HouseSize = Sizex;

    }

    public override void DoEveryDay() {

        if(!ActiveImmigrant && !immigration.InQueue(this)) {

            if (Residents < ResidentsMax)
                RequestImmigrant();

            else if (Residents > ResidentsMax)
                SpawnEmigrant();
                

        }

        if (CanEvolve())
            ChangeHouse(evolvesTo);

        else if (CanDevolve())
            ChangeHouse(devolvesTo);

        CheckBiggerSize();

    }

    public override void DoEveryMonth() {

        base.DoEveryMonth();

        HealthTimer();
        ConsumeWater();
        ConsumeFood();
        ConsumeGoods();
        ConsumeCulture();
        ConsumeFaith();

    }

    public void AddResidents(int num) {

        Residents += num;
        labor.AddPopulation(num);

    }

    public void RemoveResidents(int num) {
        
        Residents -= num;
        labor.RemovePopulation(num);

    }

    public bool WantsBetterWater { get { return WaterQual < waterQualWanted; } }
    public bool WantsBetterFood { get { return NumOfFoods() < foodTypesWant; } }
    public bool WantsBetterGoods { get { return NumOfGoods() < goodsTypesWant; } }
    public bool WantsBetterCulture { get { return Culture < cultureWant; } }
    public bool WantsBetterReligion { get { return NumOfGods < godsWant; } }

    public bool CanEvolve() {

        if (WantsBetterWater)
            return false;
        if (WantsBetterFood)
            return false;
        if (WantsBetterGoods)
            return false;
        if (WantsBetterCulture)
            return false;
        if (WantsBetterReligion)
            return false;
        return true;

    }

    public bool CanDevolve() {

        if (WaterQual < waterQualNeeded)
            return true;
        if (NumOfFoods() < foodTypesNeeded)
            return true;
        if (NumOfGoods() < goodsTypesNeeded)
            return true;
        if (Culture < cultureNeeded)
            return true;
        if (NumOfGods < godsNeeded)
            return true;
        return false;

    }

    //ADD CONDITIONS TO MAKE BIGGER
    public void CheckBiggerSize() {

        //if no bigger house to evolve to, don't continue
        if (string.IsNullOrEmpty(biggerHouse))
            return;

        //make containers for adjacent houses
        World map = world.Map;
        House h1 = null;
        House h2 = null;
        House h3 = null;

        //check X+1, Y for small house
        //if there is no house or the house there is too big, don't evolve it
        if (map.IsBuildingAt(X + 1, Y)) {

            House h = map.GetBuildingAt(X + 1, Y).GetComponent<House>();
            if (h == null)
                return;
            if (h.HouseSize != 1)
                return;
            h1 = h;

        }

        //check X, Y+1 for small house
        if (map.IsBuildingAt(X, Y + 1)) {

            House h = map.GetBuildingAt(X, Y + 1).GetComponent<House>();
            if (h == null)
                return;
            if (h.HouseSize != 1)
                return;
            h2 = h;

        }

        //check X+1, Y+1 for small house
        if (map.IsBuildingAt(X + 1, Y + 1)) {

            House h = map.GetBuildingAt(X + 1, Y + 1).GetComponent<House>();
            if (h == null)
                return;
            if (h.HouseSize != 1)
                return;
            h3 = h;

        }

        //only proceed if all houses are same level
        if (h1 == null || h2 == null || h3 == null)
            return;

        if (h1.Level != Level || h2.Level != Level || h3.Level != Level)
            return;

        //combine arrays
        Water = ArrayFunctions.CombineArrays(Water, h1.Water, h2.Water, h3.Water);
        Food = ArrayFunctions.CombineArrays(Food, h1.Food, h2.Food, h3.Food);

        //combine stats
        Residents += h1.Residents + h2.Residents + h3.Residents;
        Health += h1.Health + h2.Health + h3.Health;

        world.Demolish(h1.X, h1.Y);
        world.Demolish(h2.X, h2.Y);
        world.Demolish(h3.X, h3.Y);
        labor.AddPopulation(h1.Residents + h2.Residents + h3.Residents);
        ChangeHouse(biggerHouse);

    }

    public void ChangeHouse(string s) {

        //if s is null, don't make the new house
        if (string.IsNullOrEmpty(s))
            return;

        //demolish this and build new house
        world.Demolish(X, Y);
        world.SpawnStructure(s, X, Y, transform.position.y);

        //set vars of new house to the ones from this one
        House newHouse = world.Map.GetBuildingAt(X, Y).GetComponent<House>();
        newHouse.Residents = Residents;
        newHouse.Health = Health;
        newHouse.Water = Water;
        newHouse.WaterQual = WaterQual;
        newHouse.Food = Food;
        newHouse.Goods = Goods;
        newHouse.Faith = Faith;
        newHouse.Culture = Culture;
        newHouse.VenueAccess = VenueAccess;

        //activate new house
        newHouse.Activate();

    }

    public void FreshHouse() {

        Residents = 5;
        Health = HealthMax;
        Water = new int[(int)Quality.END];
        WaterQual = Quality.None;
        Food = new int[(int)FoodType.END];
        Goods = new int[(int)GoodType.END];
        VenueAccess = new Dictionary<string, int>();
        Faith = new Dictionary<string, int>();

    }

    public override void UponDestruction() {

        base.UponDestruction();
        labor.RemovePopulation(Residents);
        scenario.RemoveHouseLevel((Level - 1) * HouseSize * HouseSize);

    }

    public override void ReceiveImmigrant() {

        AddResidents(ResidentsMax - Residents);

    }

    public void SpawnEmigrant() {

        Structure mapExit = GameObject.FindGameObjectWithTag("MapExit").GetComponent<Structure>();

        if (mapExit == null)
            return;

        Node start = new Node(X, Y);
        Node end = new Node(mapExit.X, mapExit.Y);

        List<Node> testpath = world.Map.pathfinder.FindPath(start, end);
        if (testpath.Count == 0)
            return;

        GameObject go = Instantiate(Resources.Load<GameObject>("Walkers/Emigrant"));
        go.transform.position = mapExit.transform.position;
        go.name = "ImmigrantTo_" + name;

        Walker w = go.GetComponent<Walker>();
        w.world = world;
        w.Origin = this;
        w.Destination = mapExit;
        w.Path = w.FindPath(start, end);
        w.Activate();
        
    }

    /*************************************
    HEALTH STATS
    *************************************/

    public int Health { get; set; }
    public int HealthMax { get { return Residents * 2; } }
    public int HealthNeeded { get { return HealthMax - Health; } }
    public int HealthToLose { get { return Residents / 4; } }

    public void AddHealth(int num) {
        Health += num;
    }

    public void HealthTimer() {
        if (Health > HealthMax)
            Health = HealthMax;

        Health -= HealthToLose;

        if (Health <= 0) {
            Health = 0;
        }
    }


    /*************************************
    WATER STATS
    *************************************/

    public int[] Water { get; set; }
    public Quality WaterQual { get; set; }
    public Quality waterQualNeeded;
    public Quality waterQualWanted;

    public int WaterMax { get { return Residents * 4; } }
    public int WaterNeeded(int q) { return WaterMax - Water[q]; }
    public int WaterNeeded(Quality q) { return WaterNeeded((int)q); }
    public int WaterToConsume { get { return Residents / 4; } }

    public void AddWater(int num, Quality qual) {
        Water[(int)qual] += num;
        WaterQual = qual;
    }

    void ConsumeWater() {
        int consume = WaterToConsume;
        while (consume > 0 && WaterQual != 0) {

            //consume water from current quality level
            if (consume < Water[(int)WaterQual]) {
                Water[(int)WaterQual] -= consume;
                consume = 0;
            }
            else if (consume >= Water[(int)WaterQual]) {
                consume -= Water[(int)WaterQual];
                Water[(int)WaterQual] = 0;
            }

            //if quality level has 0 water or less, move quality level down
            if (Water[(int)WaterQual] <= 0) {
                Water[(int)WaterQual] = 0;
                WaterQual--;
            }
        }
    }

    /*************************************
    FOOD STATS
    *************************************/

    public int[] Food { get; set; }
    public int foodTypesNeeded;
    public int foodTypesWant;

    public int FoodMax { get { return Residents * 4; } }
    public int FoodNeeded(int item) { return FoodMax - Food[item]; }
    public int FoodToConsume { get { return Residents / 4; } }

    public void AddFood(int num, int item) {
        Food[item] += num;
    }

    public int NumOfFoods() {

        int s = 0;

        for (int b = 0; b < Food.Length; b++)
            if (Food[b] > 0)
                s++;

        return s;

    }

    void ConsumeFood() {

        for(int a = 0; a < foodTypesNeeded; a++) {

            int consume = FoodToConsume / foodTypesNeeded;

            for (int b = 0; b < Food.Length && consume > 0; b++) {
                

                if (Food[b] >= consume) {
                    Food[b] -= consume;
                    consume = 0;
                }
                else {
                    consume -= Food[b];
                    Food[b] = 0;
                }

            }

        }
            
    }

    /*************************************
    GOODS STATS
    *************************************/

    public int[] Goods { get; set; }
    public int goodsTypesNeeded;
    public int goodsTypesWant;

    public int GoodsMax { get { return Residents * 4; } }
    public int GoodsNeeded(int item) { return GoodsMax - Goods[item]; }
    public int GoodsToConsume { get { return Residents / 4; } }

    public void AddGoods(int num, int item) {
        Goods[item] += num;
    }

    public int NumOfGoods() {

        int s = 0;

        for (int b = 0; b < Goods.Length; b++)
            if (Goods[b] > 0)
                s++;

        return s;

    }

    void ConsumeGoods() {

        for (int a = 0; a < goodsTypesNeeded; a++) {

            int consume = GoodsToConsume / goodsTypesNeeded;

            for (int b = 0; b < Goods.Length && consume > 0; b++) {


                if (Goods[b] >= consume) {
                    Goods[b] -= consume;
                    consume = 0;
                }
                else {
                    consume -= Goods[b];
                    Goods[b] = 0;
                }

            }

        }

    }

    /*************************************
    CULTURE STATS
    *************************************/

    public Dictionary<string, int> VenueAccess { get; set; }
    public int Culture { get; set; }
    public int cultureNeeded;
    public int cultureWant;

    public void SetCulture(string venue, int amount) {

        VenueAccess[venue] = amount;

    }

    public void AddCulture(string venue, int amount) {

        if (!VenueAccess.ContainsKey(venue))
            SetFaith(venue, amount);
        else
            VenueAccess[venue] += amount;

    }

    void ConsumeCulture() {

        Culture = 0;

        if (VenueAccess == null)
            VenueAccess = new Dictionary<string, int>();

        if (VenueAccess.Count == 0)
            return;

        foreach (string venue in VenueAccess.Keys) {

            GameObject go = GameObject.Find(venue);
            if (go == null) {

                VenueAccess.Remove(venue);
                continue;

            }

            CulturalVenue c = go.GetComponent<CulturalVenue>();

            VenueAccess[venue]--;
            Culture += c.culturePoints;

            if (VenueAccess[venue] <= 0)
                VenueAccess.Remove(venue);

        }

    }

    /*************************************
    RELIGION STATS
    *************************************/

    public Dictionary<string, int> Faith { get; set; }
    public int NumOfGods { get { return Faith.Count; } }
    public int godsNeeded;
    public int godsWant;

    public void SetFaith(string god, int amount) {

        Faith[god] = amount;

    }

    public void AddFaith(string god, int amount) {

        if (!Faith.ContainsKey(god))
            SetFaith(god, amount);
        else
            Faith[god] += amount;

    }

    void ConsumeFaith() {

        if (Faith == null)
            Faith = new Dictionary<string, int>();

        if (Faith.Count == 0)
            return;

        foreach (string god in Faith.Keys){

            Faith[god]--;

            if (Faith[god] <= 0)
                Faith.Remove(god);

        }

    }

}
