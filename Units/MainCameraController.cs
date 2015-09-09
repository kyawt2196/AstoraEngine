using UnityEngine;
using System.Collections;

public class MainCameraController : MonoBehaviour {
	public int cameraSpeed;
	public bool lockCamera;
	public static Plane gamePlane;
	public float mapWidth;
	public float mapHeight;

	private float maxZoom;
	private float minZoom;
	private float zoomRate;


	// Use this for initialization
	void Start () {

		cameraSpeed = 50;
		maxZoom = 150;
		minZoom = 10;
		zoomRate = 5000;
	

	}



	// Update is called once per frame
	void Update () {
		Camera camera = GetComponent<Camera>(); 
		camera.fieldOfView -= Input.GetAxis ("Mouse ScrollWheel")*zoomRate * Time.deltaTime;
		if(camera.fieldOfView < minZoom){
			camera.fieldOfView = minZoom;
		}
		if(camera.fieldOfView > maxZoom){
			camera.fieldOfView = maxZoom;
		}

		if(!lockCamera){
			//Mouse camera position
			if(Input.mousePosition.x <= 0){
				//Debug.Log ("ScreenLeft");
				transform.Translate (Vector3.left * cameraSpeed * Time.deltaTime,Space.World);
			}else if(Input.mousePosition.x >= Screen.width){
				//Debug.Log ("ScreenRight");
				transform.Translate (Vector3.right * cameraSpeed * Time.deltaTime,Space.World);
			}

			if(Input.mousePosition.y <= 0){
				//Debug.Log ("ScreenTop");
				transform.Translate (Vector3.back * cameraSpeed * Time.deltaTime,Space.World);
			}else if(Input.mousePosition.y >= Screen.height){
				//Debug.Log ("ScreenRight");
				transform.Translate (Vector3.forward * cameraSpeed * Time.deltaTime,Space.World);
			}
		}
	}
}
