using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridNode{	
	public bool isBlock;
	public Vector3 worldPosition;
	
	public int score;
	public int gridPositionX;
	public int gridPositionY;
	public GridNode parent;
	public int gCost;
	public int hCost;

	public GridNode(bool isBlock, Vector3 worldPosition, int x, int y){
		this.isBlock = isBlock;
		this.worldPosition = worldPosition;
		this.gridPositionX = x;
		this.gridPositionY = y;
	}

	public bool Equals(GridNode other){
		return this.worldPosition == other.worldPosition;
	}

	public int fCost{
		get{
			return gCost+hCost;
		}
	}
}


