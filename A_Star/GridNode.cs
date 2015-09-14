using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GridNode : IComparable<GridNode>{	
	public bool isBlock;
	public Vector3 worldPosition;
	
	public int score;
	public int gridPositionX;
	public int gridPositionY;

	public GridNode(bool isBlock, Vector3 worldPosition, int x, int y){
		this.isBlock = isBlock;
		this.worldPosition = worldPosition;
		this.gridPositionX = x;
		this.gridPositionY = y;
	}
	
	public bool Equals(GridNode other){
		return this.worldPosition == other.worldPosition;
	}
	
	public int CompareTo(GridNode other){
		if(other != null){
			if(this.score == other.score){
				return 0;
			}else if(this.score < other.score){
				return -1;
			}else{
				return 1;
			}
		}else{
			return -1;
		}
	}
}

