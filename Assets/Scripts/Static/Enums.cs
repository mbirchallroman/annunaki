using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Terrain { Water, Lush, Grass, Mud, Sand, END }
public enum Month { March, April, May, June, July, August, September, October, November, December, January, February, END }
public enum Mode { Construct, Edit, Combat, END }
public enum DifficultyLevel { Easiest, Easy, Moderate, Hard, Hardest, END }
public enum LaborDivision { Peasantry, Industry, Health, Distribution, Administration, END }
public enum CultureValue { Heart, Mind, Soul, End }
public enum Season { Spring, Summer, Autumn, Winter, END}
public enum FoodType { Dates, Fish, Lettuce, Meat, Lentils, Pomegranates, Wheat, END }
public enum ResourceType { Asphalt, Barley, Bricks, Clay, Flax, Lumber, Ingots, Olives, Ore, Stone, Weapons, Wool, END }
public enum GoodType { Beer, Carpets, Furniture, Milk, Oil, Pottery, Tunics, END }
public enum ItemType { Food, Good, Resource, END }
public enum Quality { None, Poor, Average, Great, Luxurious, END }
public enum NotificationType { Event, Issue, GoodNews, Invasion, END }
public enum TradeDirection { Export, Import, END }
public enum BuildingType { Administration, Culture, Distribution, Health, Industry, Resources, Agriculture }

public class Enums {
    public static Dictionary<string, Terrain> terrainDict = new Dictionary<string, Terrain>();
    public static Dictionary<string, Node> foodDict = new Dictionary<string, Node>();
    public static Dictionary<string, Node> resourceDict = new Dictionary<string, Node>();
    public static Dictionary<string, Node> goodDict = new Dictionary<string, Node>();

    public static void LoadDictionaries() {

        for (int x = 0; x < (int)Terrain.END; x++)
            terrainDict[(Terrain)x + ""] = (Terrain)x;

        //for food/good/resources, X is item, Y is type
        for (int x = 0; x < (int)FoodType.END; x++)
            foodDict[(FoodType)x + ""] = new Node(x, (int)ItemType.Food);

        for (int x = 0; x < (int)ResourceType.END; x++)
            resourceDict[(ResourceType)x + ""] = new Node(x, (int)ItemType.Resource);

        for (int x = 0; x < (int)GoodType.END; x++)
            goodDict[(GoodType)x + ""] = new Node(x, (int)ItemType.Good);

    }

    public static string GetItemName(int index, ItemType type) {
        
        //if granary, display food label
        if (type == ItemType.Food)
            return (FoodType)index + "";

        //if warehouse, display goods label
        else if (type == ItemType.Good)
            return (GoodType)index + "";

        //if storage yard, display resource label
        else if (type == ItemType.Resource)
            return (ResourceType)index + "";

        return "???";

    }

}