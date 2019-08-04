using UnityEngine;
using System.Collections;

public class TileMouseOver : MonoBehaviour {
	
	public Color highlightColor;
	Color normalColor; 
	
	void Start() {
		normalColor = GetComponent<MeshRenderer>().material.color;
	}
	
	// Update is called once per frame
	void Update () {
		
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hitInfo;
		
		if( GetComponent<MeshCollider>().Raycast( ray, out hitInfo, Mathf.Infinity ) ) {
            GetComponent<MeshRenderer>().material.color = highlightColor;
		}
		else {
            GetComponent<MeshRenderer>().material.color = normalColor;
		}
		
	}
	
}
