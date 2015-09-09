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

	private void findPath(GridNode start, GridNode end){
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

	private Vector3 nodeDifference(GridNode node1, GridNode node2){
		return node1.worldPosition - node2.worldPosition;
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
		List<GridNode> open = new List<GridNode>();
		Dictionary<GridNode,int> costSoFar = new Dictionary<GridNode,int>();
		Dictionary<GridNode,GridNode> cameFrom = new Dictionary<GridNode,GridNode>();
		open.Add (start);
		costSoFar.Add(start, 0);
		while(open.Count > 0){
			//finds node with the lowest score to look at
			GridNode current = findLowestScore(open);
			open.Remove(current);
			if(current.Equals(end)){
				retraceRoute(start,current,cameFrom);
				open.Clear ();
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
							open.Add (node);
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
 * 
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
List<GridNode> openSet = new List<GridNode>();
		HashSet<GridNode> closedSet = new HashSet<GridNode>();
		openSet.Add(start);
		
		while (openSet.Count > 0) {
			GridNode currentNode = openSet[0];
			for (int i = 1; i < openSet.Count; i ++) {
				if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
					currentNode = openSet[i];
				}
			}
			
			openSet.Remove(currentNode);
			closedSet.Add(currentNode);
			
			if (currentNode.Equals(end)) {
				RetracePath(start,end);
				return;
			}
			
			foreach (GridNode neighbour in gameGrid.getNeighbours(currentNode)) {
				if (neighbour.isBlock || closedSet.Contains(neighbour)) {
					continue;
				}
				
				int newMovementCostToNeighbour = currentNode.gCost + GetDistanceBetween(currentNode, neighbour);
				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistanceBetween(neighbour, end);
					neighbour.parent = currentNode;
					
					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}
		explored = new HashSet<GridNode>(openSet);

	private void RetracePath(GridNode startNode, GridNode endNode){
		route.Clear ();
		GridNode currentNode = endNode;
		while(currentNode != startNode){
			route.Push(currentNode);
			currentNode = currentNode.parent;
		}
	}
	private int GetDistanceBetween(GridNode nodeA, GridNode nodeB){
		int dstX = Mathf.Abs(nodeA.gridPositionX - nodeB.gridPositionY);
		int dstY = Mathf.Abs (nodeA.gridPositionY - nodeB.gridPositionY);
		if(dstX>dstY)
			return 14*dstY + 10*(dstX-dstY);
		return 14*dstX + 10*(dstY-dstX);
	}

 */
