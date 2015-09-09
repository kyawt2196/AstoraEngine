using UnityEngine;
using System.Collections;
[RequireComponent(typeof(UnitMovement))]
public class Unit : MonoBehaviour {
	private int health;

	//public Material shipMaterial;


	Material material;
	public bool selected;
	// Use this for initialization
	void Start () {
		selected = false;
		material = new Material(Shader.Find("Outlined/Silhouette Only"));
	}


	// Update is called once per frame
	void Update () {
		selectUnit ();




	}

	private void selectUnit(){
		//Drag select functionality
		if(Input.GetMouseButtonUp (0)){
			//this unprojects the 3d object into 2d space ie our GUI
			Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
			camPos.y = Screen.height - camPos.y;
			//Debug.Log (DragSelect.dragging);
			
			
			if(DragSelect.selection.Contains(camPos)){
				selected = true;
			}else if(!Input.GetKey(KeyCode.LeftShift)){
				selected = false;
			}
			
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
			{
				//Debug.Log ("test");
				if(hit.collider.gameObject.Equals(this.gameObject)){
					selected = true;
				}else if(!Input.GetKey(KeyCode.LeftShift)){
					selected = false;
					
				}
			}
			DragSelect.dragging = false;
		}
		//Material[] mats = GetComponent<MeshRenderer>().materials;
		if(selected){
			//mats[1] = material;
				
		}else{
			//mats[1] = null;
		}
		//GetComponent<MeshRenderer>().materials = mats;
	}

}
