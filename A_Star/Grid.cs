//Kyaw Thant
//Grid for use by A star pathfinding algorithim

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Grid : MonoBehaviour {
	public bool showGrid;
	public Vector2 gridWorldSize;
	public float nodeDiameter;
	public LayerMask SnorlaxMask;

	public GridNode[,] grid;
	public int gridLength;
	public int gridHeight;

	void Start(){
		transform.position = Vector3.zero;
		createGrid ();
	}

	//updates the grid everyframe
	void Update(){
		for(int i=0; i<gridLength; i++){
			for(int n=0; n<gridHeight; n++){
				grid[i,n].isBlock = Physics.CheckSphere(grid[i,n].worldPosition,nodeDiameter, SnorlaxMask);
			}
		}
	}

	//Creates grid with the size of Vector2 gridWorldSize
	private void createGrid(){
		gridLength = getArrayPosition(gridWorldSize.x);
		gridHeight = getArrayPosition(gridWorldSize.y);
		grid = new GridNode[gridLength,gridHeight];
		for(int i=0; i<gridLength; i++){
			for(int n=0; n<gridHeight; n++){
				Vector3 nodePosition = new Vector3(i*nodeDiameter+nodeDiameter/2, 0 ,n*nodeDiameter+nodeDiameter/2);
				bool isBlocked = false;
				if(n != 0 || n != gridHeight -1 || i != 0 || i != gridLength -1){
					isBlocked = Physics.CheckSphere(nodePosition,nodeDiameter, SnorlaxMask);
				}
				grid[i,n] = new GridNode(isBlocked,nodePosition,i,n);
			}
		}
	}

	//returns a node from a legal vector3 position
	public GridNode getNodeFromPosition(Vector3 position){
		int x = getArrayPosition(position.x);
		int y = getArrayPosition(position.z);
		GridNode temp = null;
		if(!withinArrayBounds (x,y)){
			if(x<0){
				x = 0;
			}else if(x >= gridLength){
				x = gridLength - 1;
			}
			if(y<0){
				y = 0; 
			}else if(y >= gridHeight){
				y = gridHeight - 1;
			}
		}
		temp = grid[x,y];
		
		return temp;
	}

	//returns a list of surrounding nodes of a home node
	public List<GridNode> getNeighbours(GridNode home){
		List<GridNode> list = new List<GridNode>();
		int x = home.gridPositionX;
		int y = home.gridPositionY;
		for(int h = -1; h <= 1; h++){
			for(int v = -1; v <= 1; v++){
				if(withinArrayBounds(x+h, y+v)){
					list.Add(grid[x+h,y+v]);
				}
			}
		}
	
		return list;
	}

	//returns a node that is not blocked towards the player
	//used for when player clicks on blocked node
	public GridNode findNearestUnblockedNode(GridNode node, Vector3 playerPosition){
		GridNode current = node;
		//if(current != null){
			while(current.isBlock){
				if(current.worldPosition != playerPosition){
					if(current.worldPosition.x < playerPosition.x){
						current = grid[current.gridPositionX+1, current.gridPositionY];
					}else{
						current = grid[current.gridPositionX-1, current.gridPositionY];
					}
					if(current.worldPosition.z < playerPosition.z){
						current = grid[current.gridPositionX, current.gridPositionY+1];
					}else{
						current = grid[current.gridPositionX, current.gridPositionY-1];
					}
				}
			}
		//}
		return current;
	}

	//checks whether the two ints are within the bounds of an array
	public bool withinArrayBounds(int x, int y){
		return (x >= 0 && x < gridLength && y >=0 && y < gridHeight);
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.black;
		Vector3 centerFrame = new Vector3(gridWorldSize.x/2,0,gridWorldSize.x/2);
		Gizmos.DrawWireCube(centerFrame, new Vector3(gridWorldSize.x,1,gridWorldSize.y));
		if(grid != null && showGrid){
			for(int i=0; i<gridLength; i++){
				for(int n=0; n<gridHeight; n++){
					Vector3 center = new Vector3(grid[i,n].worldPosition.x,0, grid[i,n].worldPosition.z);
					Vector3 size = new Vector3(nodeDiameter-.4f,0,nodeDiameter-.4f);
					if(grid[i,n].isBlock){
						Gizmos.color = Color.red;
					}else{
						Gizmos.color = Color.cyan;
					}
					Gizmos.DrawWireCube(center, size);
				}
			}
		}
	}

	private int getArrayPosition(float position){
		return Mathf.FloorToInt(position/nodeDiameter);
	}
	
	public int getDistanceBetween(GridNode current, GridNode goal){
		int dx = Math.Abs(goal.gridPositionX - current.gridPositionX);
		int dy = Math.Abs(goal.gridPositionY - current.gridPositionY);
		int h = (int)(10*Math.Sqrt(dx*dx + dy*dy));
		return h;
	}
	
	public int getHeuristic(GridNode current, GridNode goal){
		int dx = Math.Abs(goal.gridPositionX - current.gridPositionX);
		int dy = Math.Abs(goal.gridPositionY - current.gridPositionY);
		int h = (int)(10*(dx+dy)+(14-2*10)+Math.Min(dx,dy));
		return h;
	}
	
	public int getManhattanHeuristic(GridNode current, GridNode goal){
		int dx = Math.Abs(goal.gridPositionX - current.gridPositionX);
		int dy = Math.Abs(goal.gridPositionY - current.gridPositionY);
		int h = (int)(dx + dy);
		return h;
	}
}

