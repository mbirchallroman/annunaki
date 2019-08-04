using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UsefulThings;

public class WorldController : MonoBehaviour {

    public WorldGenerator mapGenerator;
    public GameObject structures;
    public GameObject walkers;

    [Header("Controllers")]
    public ActionSelecterController actionSelecter;
    public CameraController cameraController;
    public DiplomacyController diplomacy;
    public ImmigrationController immigration;
    public InteractionController interaction;
    public LaborController labor;
    public ModeController modeController;
    public MoneyController money;
    public NotificationController notifications;
    public ScenarioController scenario;
    public TileMap tilemap;
    public TimeController timeController;
    public TradeController trade;
    public UIObjectDatabase uiObjectDatabase;

    public World Map { get; set; }
    public ObjWindow objWindow { get; set; }

    public bool PlaceOnRoads { get; set; }

    private void Start() {
        Enums.LoadDictionaries();

        BasicWorldSave save = SaveLoadMap.GetGameData();

        //load world if it exists
        if (save != null) {

            if (save is WorldProgressSave)
                LoadSavedGame((WorldProgressSave)save);
            else
                LoadScenario(save);

        }
        else
            CreateWorld();

        GenerateWorld();
    }

    public void CreateWorld() {

        Node size = SaveLoadMap.newWorldSize;
        int szx = (int)size.X;
        int szy = (int)size.Y;
        
        Map = new World(size);
        Map.terrain = mapGenerator.GetRandomTerrain(size);

        SpawnStructure("MapEntrance", 0, 0, 0);
        SpawnStructure("MapExit", szx - 1, szy - 1, 0);

        money.Sheqels = 1000;
        labor.InstantiateLabor();
        
        actionSelecter.FreshActions();

        if (notifications != null)
            notifications.FreshEvents();

    }

    public void GenerateWorld() {

        GameObject worldObj = new GameObject();
        worldObj.name = "WorldMap";

        //for (int a = 0; a < Map.size.X; a++) {

        //    GameObject row = new GameObject();
        //    row.transform.parent = worldObj.transform;
        //    row.name = "Row_" + a;

        //    for (int b = 0; b < Map.size.Y; b++) {

        //        GenerateTile(a, b);

        //    }

        //}
        
        tilemap.BuildMesh(Map);

        actionSelecter.LoadActionButtons();
        actionSelecter.LoadActionEnablers();

    }

    public void GenerateTile(int x, int y) {

        //destroy old tile if it exists
        GameObject old = GameObject.Find("Tile_" + x + "_" + y);
        if (old != null)
            Destroy(old);

        //create new tile
        GameObject tile = Instantiate(Resources.Load<GameObject>("Tiles/" + (Terrain)Map.terrain[x,y]));
        float height = tile.transform.localScale.y;
        tile.transform.parent = GameObject.Find("Row_" + x).transform;
        tile.transform.position = new Vector3(x, -.6f, y);
        tile.name = "Tile_" + x + "_" + y;

    }

    public void LoadScenario(BasicWorldSave w) {

        Map = w.world;
        money.Load(w.money);
        actionSelecter.Load(w.actionSelecter);
        diplomacy.Load(w.diplomacy);
        scenario.Load(w.scenario);

        //GO THROUGH LISTS OF OBJECTS AND ACTIVATE THEM USING THE LOADMAPOBJECT() FUNCTION
        //structures
        foreach (ObjSave save in w.structures)
            LoadMapObject(save).transform.parent = structures.transform;

        labor.InstantiateLabor();
        if (notifications != null)
            notifications.FreshEvents();

    }

    public void LoadSavedGame(WorldProgressSave w) {

        Map = w.world;
        timeController.Load(w.time);
        labor.Load(w.labor);
        money.Load(w.money);
        cameraController.Load(w.camera);
        actionSelecter.Load(w.actionSelecter);
        immigration.Load(w.immigration);
        trade.Load(w.trade);
        diplomacy.Load(w.diplomacy);
        scenario.Load(w.scenario);

        //GO THROUGH LISTS OF OBJECTS AND ACTIVATE THEM USING THE LOADMAPOBJECT() FUNCTION
        //structures
        foreach (ObjSave save in w.structures)
            LoadMapObject(save).transform.parent = structures.transform;
        foreach (ObjSave save in w.workplaces)
            LoadMapObject(save).transform.parent = structures.transform;
        foreach (ObjSave save in w.storagebuildings)
            LoadMapObject(save).transform.parent = structures.transform;
        foreach (ObjSave save in w.generators)
            LoadMapObject(save).transform.parent = structures.transform;
        foreach (ObjSave save in w.factories)
            LoadMapObject(save).transform.parent = structures.transform;
        foreach (ObjSave save in w.stables)
            LoadMapObject(save).transform.parent = structures.transform;
        foreach (ObjSave save in w.houses)
            LoadMapObject(save).transform.parent = structures.transform;
        foreach (ObjSave save in w.markets)
            LoadMapObject(save).transform.parent = structures.transform;

        //walkers
        foreach (ObjSave save in w.animals)
            LoadMapObject(save).transform.parent = walkers.transform;
        foreach (ObjSave save in w.walkers)
            LoadMapObject(save).transform.parent = walkers.transform;
        foreach (ObjSave save in w.venders)
            LoadMapObject(save).transform.parent = walkers.transform;

    }

    public GameObject LoadMapObject(ObjSave save) {

        //load object
        GameObject go = Instantiate(Resources.Load<GameObject>(save.resourcePath));

        //activate object
        Obj o = go.GetComponent<Obj>();
        o.world = this;
        o.Load(save);

        return go;
    }


    public bool CanSpawnStructure(string type, int x, int y, float buildingRotation) {

        StructureData data = StructureDatabase.structureData[type];

        int sizex = data.Sizex;
        int sizey = data.Sizey;

        //switch size dimensions if building is rotated
        if (buildingRotation % 180 != 0) {
            int tempszx = sizex;
            int tempszy = sizey;
            sizex = tempszy;
            sizey = tempszx;
        }

        for (int a = x; a < x + sizex; a++)
            for (int b = y; b < y + sizey; b++) {

                //coordinates within array
                int m = a - x;
                int n = b - y;

                if (Map.OutOfBounds(a, b))
                    return false;

                //if building has road tiles beneath
                else if (data.HasRoadTiles) {

                    int[,] roads = ArrayFunctions.RotatedArray(data.RoadTiles, buildingRotation);

                    //if there should be a road at this tile
                    if(roads[m, n] == 1) {

                        //if no structure at all, return false
                        if (string.IsNullOrEmpty(Map.structures[a, b]))
                            return false;

                        //else if there's not a road here or it does not contain "Road", return false
                        else if (Map.roads[a, b] != 2 || !Map.GetBuildingNameAt(a, b).Contains("Road_"))
                            return false;

                    }

                }

                //else if there's a structure on this tile when there shouldn't be
                else if(!string.IsNullOrEmpty(Map.structures[a, b]))
                    return false;

                if (data.HasWaterTiles) {

                    int[,] water = ArrayFunctions.RotatedArray(data.WaterTiles, buildingRotation);

                    if (Map.terrain[a, b] != (int)Terrain.Water && water[m, n] == 0 || Map.terrain[a, b] == (int)Terrain.Water && water[m, n] != 0)
                        return false;

                }

                //else return false if there's water here and there shouldn't be
                else if (Map.terrain[a, b] == (int)Terrain.Water)
                    return false;

            }

        return true;
    }

    //place a building
    public void SpawnStructure(string type, int x, int y, float buildingRotation) {

        //don't do if not possible
        if (!CanSpawnStructure(type, x, y, buildingRotation))
            return;

        //retrieve data about structure
        StructureData data = StructureDatabase.structureData[type];

        int sizex = data.Sizex;
        int sizey = data.Sizey;
        float alignx = data.Alignx;
        float aligny = data.Aligny;

        //if placing a mapentrance or mapexit, remove existing one
        if (data.BuiltOnce) {

            //find object
            GameObject g = GameObject.Find(type);

            //only try to delete it if it exists
            if (g != null) {

                Structure m = g.GetComponent<Structure>();
                Demolish(m.X, m.Y);

            }

        }

        //destroy roads beneath area if they're there
        else if (data.HasRoadTiles) {

            for (int a = x; a < sizex + x; a++) {
                for (int b = y; b < sizey + y; b++) {
                    PlaceOnRoads = true;
                    Demolish(x, y);
                }
            }

        }

        //switch size dimensions if building is rotated
        if (buildingRotation % 180 != 0) {
            int tempszx = sizex;
            int tempszy = sizey;
            sizex = tempszy;
            sizey = tempszx;

            float tempalignx = alignx;
            float tempaligny = aligny;
            alignx = tempaligny;
            aligny = tempalignx;

        }
        
        GameObject go = Instantiate(Resources.Load<GameObject>("Structures/" + type), new Vector3(x, 0, y), Quaternion.Euler(new Vector3(0, buildingRotation, 0)));
        go.transform.position += new Vector3(alignx, 0, aligny);

        //unless object is a mapentrance or mapexit, put its coords in its name
        go.name = type + "_" + x + "_" + y;
        if (data.BuiltOnce)
            go.name = type;

        //rename the area it takes up to its name
        Map.RenameArea(go.name, x, y, sizex, sizey);

        Structure s = go.GetComponent<Structure>();
        if (s == null)
            return;
        s.X = x;
        s.Y = y;
        s.Sizex = sizex;
        s.Sizey = sizey;
        s.DisplayName = data.DisplayName;
        s.world = this;
        s.Activate();
    }

    public void Demolish(int x, int y) {

        GameObject go = Map.GetBuildingAt(x, y);
        if (go == null)
            return;

        Structure s = go.GetComponent<Structure>();

        if (!s.canBeDemolished && modeController.currentMode != Mode.Edit)
            return;

        int sizex = s.Sizex;
        int sizey = s.Sizey;
        x = s.X;
        y = s.Y;

        Map.RenameArea(null, x, y, sizex, sizey);
        Destroy(go);
    }

    //check if a tile can be painted
    public bool CanPaintTerrain(string type, int x, int y) {

        //if out of range, don't try it
        if (Map.OutOfBounds(x, y))
            return false;

        //if terrain is already this, don't change
        if (Map.terrain[x, y] == (int)Enums.terrainDict[type])
            return false;

        if (Enums.terrainDict[type] == Terrain.Water && Map.IsBuildingAt(x, y))
            return false;

        return true;
    }

    //paint a tile in the world map
    public void PaintTerrain(string type, int x, int y) {

        if (!CanPaintTerrain(type, x, y))
            return;

        Map.terrain[x, y] = (int)Enums.terrainDict[type];
        tilemap.PaintTile(x, y);

    }

    public bool MapCornersConnected() {

        Node a = new Node(GameObject.Find("MapEntrance").GetComponent<Structure>());
        Node b = new Node(GameObject.Find("MapExit").GetComponent<Structure>());
        List<Node> path = Map.pathfinder.FindPath(a, b);

        return path.Count != 0;

    }

    public bool HasGood(int num, int item, ItemType type) {

        GameObject[] objs = GameObject.FindGameObjectsWithTag("StorageBuilding");

        if (objs.Length == 0 && num != 0)
            return false;
        else if (objs.Length == 0)
            return true;
        
        int sum = 0;

        foreach (GameObject go in objs) {

            StorageBuilding strg = go.GetComponent<StorageBuilding>();

            //if storage building does not store that type of item, continue
            if (strg.typeStored != type)
                continue;

            sum += strg.Inventory[item];

        }

        Debug.Log(sum + " " + num);

        return sum >= num;

    }

    public bool HasGood(ItemOrder io) { return HasGood(io.amount, io.item, io.type); }

}
