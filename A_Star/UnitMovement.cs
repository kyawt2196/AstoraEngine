using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class UnitMovement : MonoBehaviour {
	public float movementSpeed;
	public float RotationSpeed;

	private Quaternion lookRotation;
	private Vector3 direction;
	private Ray ray;
	private Vector3 destination;
	private Vector3 currentDestination;

	private GridNode nodeEnd;
	private GridNode nodeStart;
	private Grid gameGrid;

	private Stack<GridNode> route;
	//for debugging
	private HashSet<GridNode> explored;

	// Use this for initialization
	void Start () {
		explored = new HashSet<GridNode>();
		route = new Stack<GridNode>();
		gameGrid = GameObject.Find ("GameManager").GetComponent<Grid>();
		destination = this.transform.position;
		currentDestination = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<Unit>().selected){
			if(Input.GetMouseButtonDown(1)){
				//this gets the location;
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Plane plane = new Plane(Vector3.up, Vector3.zero);
				float rayDistance;
				if(plane.Raycast(ray, out rayDistance)){
					destination = ray.GetPoint(rayDistance);
				}
				nodeEnd = gameGrid.getNodeFromPosition(destination);
				nodeStart = gameGrid.getNodeFromPosition(transform.position);
				route.Clear ();
				findPath(nodeStart, gameGrid.findNearestUnblockedNode(nodeEnd,transform.position));
				
			}
			if(Input.GetMouseButtonDown(0)){
				Heap<GridNode> heap = new Heap<GridNode>(100);
				gameGrid.grid[0,0].score = 10;
				gameGrid.grid[1,0].score = 2;
				gameGrid.grid[2,0].score = 1;
				gameGrid.grid[3,0].score = 5;
				gameGrid.grid[4,0].score = 6;
				heap.insert (gameGrid.grid[0,0]);
				heap.insert (gameGrid.grid[1,0]);
				heap.insert (gameGrid.grid[2,0]);
				heap.insert (gameGrid.grid[3,0]);
				heap.insert (gameGrid.grid[4,0]);
				for(int i=0; i<5; i++){
					//print (heap.items[i].score);
				}
				for(int i=0; i<5; i++){
					heap.extract();
				}

			}
		}
		followPath ();

		//S stop
		if(Input.GetKeyDown(KeyCode.S)){
			route.Clear ();
			destination = transform.position;
		}
	}

	void followPath(){
		if(route.Count == 0 && !gameGrid.getNodeFromPosition(destination).isBlock){
			currentDestination = destination;
		}else if(gameGrid.getNodeFromPosition(transform.position).Equals (
				gameGrid.getNodeFromPosition(currentDestination))){
			if(route.Count > 0){
				currentDestination = route.Pop().worldPosition;
			}
		}
		moveTowards (currentDestination);
	}
	
	void moveTowards(Vector3 goal){  
		//find the vector pointing from our position to the target
		direction = (goal - transform.position).normalized;
		//create the rotation we need to be in to look at the target
		if(direction != Vector3.zero){	
			lookRotation = Quaternion.LookRotation(direction);
			//rotate us over time according to speed until we are in the required rotation
			transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
		}
		transform.position =  Vector3.MoveTowards (transform.position, goal, movementSpeed * Time.deltaTime);
	}

	//uses A* to find path
	private void findPath(GridNode start, GridNode end){
		Stopwatch watch = new Stopwatch();
		watch.Start();
		Heap<GridNode> heap = new Heap<GridNode>(gameGrid.gridHeight*gameGrid.gridLength);
		Dictionary<GridNode,int> costSoFar = new Dictionary<GridNode,int>();
		Dictionary<GridNode,GridNode> cameFrom = new Dictionary<GridNode,GridNode>();
		HashSet<GridNode> closed = new HashSet<GridNode>();
		heap.insert (start);
		costSoFar.Add(start, 0);
		while(heap.Count > 0){
			GridNode current = heap.extract();
			closed.Add (current);
			if(current.Equals(end)){
				retraceRoute(start,current,cameFrom);
				heap.Clear ();
				watch.Stop();
				print (watch.ElapsedMilliseconds);
			}else{
				List<GridNode> list = gameGrid.getNeighbours(current);
				foreach(GridNode node in list){
					if(!node.isBlock && !closed.Contains(node)){
						int cost = costSoFar[current] + gameGrid.getDistanceBetween(current,node);
						if(!costSoFar.ContainsKey (node) || cost < costSoFar[node]){
							costSoFar.Remove (node);
							costSoFar.Add (node,cost);
							node.score = cost + gameGrid.getHeuristic(node, end);
							heap.insert (node);
							cameFrom.Remove (node);
							cameFrom.Add (node, current);
						}
					}
				}
			}
		}	
		explored = new HashSet<GridNode>(closed);
	}

	private GridNode findLowestScore(List<GridNode> open){
		GridNode current = open[0];
		for(int i=1; i<open.Count; i++){
			if(current.score > open[i].score){
				current = open[i];
			}
		}
		return current;
	}

	private void retraceRoute(GridNode start, GridNode end, Dictionary<GridNode,GridNode> cameFrom){
		route.Clear();
		GridNode current = end;
		while(!current.Equals(start)){
			route.Push (current);
			current = cameFrom[current];
		}
	}

	void OnDrawGizmos(){
		if(gameGrid != null){
			foreach(GridNode path in explored){
				Vector3 center = new Vector3(path.worldPosition.x,0, path.worldPosition.z);
				Vector3 size = new Vector3(gameGrid.nodeDiameter,0,gameGrid.nodeDiameter);
				Gizmos.color = Color.white;
				Gizmos.DrawCube(center, size);
			}
			foreach(GridNode path in route){
				Vector3 center = new Vector3(path.worldPosition.x,0, path.worldPosition.z);
				Vector3 size = new Vector3(gameGrid.nodeDiameter,0,gameGrid.nodeDiameter);
				Gizmos.color = Color.green;
				Gizmos.DrawCube(center, size);
			}
		}
		
	}


}

 /* 
Stopwatch watch = new Stopwatch();
		watch.Start();
		PriorityGridNodeQueue queue = new PriorityGridNodeQueue();
		Dictionary<GridNode,int> costSoFar = new Dictionary<GridNode,int>();
		Dictionary<GridNode,GridNode> cameFrom = new Dictionary<GridNode,GridNode>();
		queue.Enqueue (start,0);
		costSoFar.Add(start, 0);
		while(queue.Count > 0){
			GridNode current = queue.Dequeue();
			if(current.Equals(end)){
				retraceRoute(start,current,cameFrom);
				queue.Clear ();
				watch.Stop();
				print (watch.ElapsedMilliseconds);
			}else{
				List<GridNode> list = gameGrid.getNeighbours(current);
				foreach(GridNode node in list){
					if(!node.isBlock){
						int cost = costSoFar[current] + gameGrid.getDistanceBetween(current,node);
						if(!costSoFar.ContainsKey (node) || cost < costSoFar[node]){
							if(costSoFar.ContainsKey (node)){
								costSoFar.Remove (node);
							}
							costSoFar.Add (node,cost);
							node.score = cost + gameGrid.getHeuristic(node, end);
							queue.Enqueue (node, node.score);
							if(cameFrom.ContainsKey (node)){
								cameFrom.Remove (node);
							}
							cameFrom.Add (node, current);
						}
					}
				}
				
			}
			
			explored = new HashSet<GridNode>(costSoFar.Keys);
		}
	Stopwatch watch = new Stopwatch();
		watch.Start();
		PriorityGridNodeQueue queue = new PriorityGridNodeQueue();
		Dictionary<GridNode,int> costSoFar = new Dictionary<GridNode,int>();
		Dictionary<GridNode,GridNode> cameFrom = new Dictionary<GridNode,GridNode>();
		queue.Enqueue (start,0);
		costSoFar.Add(start, 0);
		while(queue.Count > 0){
			GridNode current = queue.Dequeue();
			if(current.Equals(end)){
				retraceRoute(start,current,cameFrom);
				queue.Clear ();
				watch.Stop();
				print (watch.ElapsedMilliseconds);
			}else{
				List<GridNode> list = gameGrid.getNeighbours(current);
				foreach(GridNode node in list){
					if(!node.isBlock){
						int cost = costSoFar[current] + gameGrid.getDistanceBetween(current,node);
						if(!costSoFar.ContainsKey (node) || cost < costSoFar[node]){
							if(costSoFar.ContainsKey (node)){
								costSoFar.Remove (node);
							}
							costSoFar.Add (node,cost);
							node.score = cost + gameGrid.getHeuristic(node, end);
							queue.Enqueue (node, node.score);
							if(cameFrom.ContainsKey (node)){
								cameFrom.Remove (node);
							}
							cameFrom.Add (node, current);
						}
					}
				}
				
			}
			
			explored = new HashSet<GridNode>(costSoFar.Keys);
		}

Stopwatch watch = new Stopwatch();
		watch.Start();
		Heap<GridNode> heap = new Heap<GridNode>(gameGrid.gridHeight*gameGrid.gridLength);
		Dictionary<GridNode,int> costSoFar = new Dictionary<GridNode,int>();
		Dictionary<GridNode,GridNode> cameFrom = new Dictionary<GridNode,GridNode>();
		heap.insert (start);
		costSoFar.Add(start, 0);
		while(heap.Count > 0){
			GridNode current = heap.extract();
			if(current.Equals(end)){
				retraceRoute(start,current,cameFrom);
				heap.Clear ();
				watch.Stop();
				print (watch.ElapsedMilliseconds);
			}else{
				List<GridNode> list = gameGrid.getNeighbours(current);
				foreach(GridNode node in list){
					if(!node.isBlock){
						int cost = costSoFar[current] + gameGrid.getDistanceBetween(current,node);
						if(!costSoFar.ContainsKey (node) || cost < costSoFar[node]){
							if(costSoFar.ContainsKey (node)){
								costSoFar.Remove (node);
							}
							costSoFar.Add (node,cost);
							node.score = cost + gameGrid.getHeuristic(node, end);
							heap.insert (node);
							if(cameFrom.ContainsKey (node)){
								cameFrom.Remove (node);
							}
							cameFrom.Add (node, current);
						}
					}
				}
				
			}
			
			explored = new HashSet<GridNode>(costSoFar.Keys);
		}
	Stopwatch watch = new Stopwatch();
		watch.Start();
		Heap<GridNode> open = new Heap<GridNode>(gameGrid.gridHeight*gameGrid.gridLength);
		HashSet<GridNode> closed = new HashSet<GridNode>();
		Dictionary<GridNode,GridNode> parents = new Dictionary<GridNode, GridNode>();
		Dictionary<GridNode, int> costOf = new Dictionary<GridNode, int>();
		open.insert(start);
		costOf.Add(start, 0);
		while(open.Count > 0){
			GridNode current = open.extract ();
			closed.Add(current);
			if(current.Equals(end)){

			}else{
				List<GridNode> list = gameGrid.getNeighbours(current);
				foreach(GridNode neighbour in list){
					if(!neighbour.isBlock || !closed.Contains (neighbour)){
						int cost = gameGrid.getHeuristic(end,neighbour)+gameGrid.getDistanceBetween(start, neighbour);
						if(!costOf.ContainsKey (neighbour) || cost<costOf[neighbour]){
							
						}
					}
				}
			}
		}
 */
