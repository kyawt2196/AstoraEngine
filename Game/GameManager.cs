using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public Transform boundary1;
	public Transform boundary2;

	public int numOfBounday;
	public Vector2 worldSize;

	// Use this for initialization
	void Start () {
		for(int i = 0; i < numOfBounday; i++){
			Instantiate (boundary1, new Vector3(Random.Range(0,worldSize.x),0,Random.Range(0,worldSize.y)), boundary1.transform.rotation);
		}
		for(int i = 0; i < numOfBounday; i++){
			Instantiate (boundary2, new Vector3(Random.Range(0,worldSize.x),0,Random.Range(0,worldSize.y)), boundary2.transform.rotation);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
