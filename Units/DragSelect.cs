using UnityEngine;
using System.Collections;

public class DragSelect : MonoBehaviour {
	public GUIStyle selectBox;
	public static Rect selection;
	public static bool dragging;

	private Vector2 startPos;
	
	// Use this for initialization
	void Start () {
		selectBox = new GUIStyle();
		selection = new Rect(0,0,0,0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){

		if(Input.GetMouseButtonDown(0)){
			startPos = Input.mousePosition;

		}
		if(Input.GetMouseButton(0)){
			//Debug.Log (Input.mousePosition.x-startPos.x);
			//Debug.Log (Input.mousePosition.y-startPos.y);
			if(Input.mousePosition.x-startPos.x != 0 || startPos.y-Input.mousePosition.y != 0){
				dragging = true;
			}
			if(dragging){
				selection = new Rect(startPos.x, Screen.height - startPos.y, Input.mousePosition.x-startPos.x, startPos.y-Input.mousePosition.y);
				GUI.Box (selection, "");
			}

		}
		if(Input.GetMouseButtonUp(0)){
		
			//since contains methods does not accept negatives we have to move our points;
			if(selection.width < 0){
				selection.x += selection.width;
				selection.width = -selection.width;
			}
			if(selection.height < 0){
				selection.y += selection.height;
				selection.height = -selection.height;
			}

		}

	}


}
