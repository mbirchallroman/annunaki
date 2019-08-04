using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TileMap))]
public class TileMapMouse : MonoBehaviour {
	
	TileMap _tileMap;
	
	Vector3 currentTileCoord;
	
	void Start() {
		_tileMap = GetComponent<TileMap>();
	}

	// Update is called once per frame
	void Update () {

        Vector3 pos = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(pos);
		RaycastHit hitInfo;
		
		if(GetComponent<MeshCollider>().Raycast( ray, out hitInfo, Mathf.Infinity ) ) {
			int x = Mathf.FloorToInt( hitInfo.point.x + .5f);
			int z = Mathf.FloorToInt( hitInfo.point.z + .5f);
			
			currentTileCoord.x = x;
			currentTileCoord.z = z;
			
		}

	}
}
