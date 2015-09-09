using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityGridNodeQueue {
	private Dictionary<int, Queue<GridNode>> dictionary;
	public int minScore;
	public int maxScore;

	public int Count;
	// Use this for initialization
	public PriorityGridNodeQueue () {
		dictionary = new Dictionary<int, Queue<GridNode>>();
		Count = 0;
	}
	
	public void Enqueue(GridNode node,int score){
		if(Count == 0){
			minScore = score;
			maxScore = score;
		}else{
			if(minScore > score){
				minScore = score;
			}
			if(maxScore < score){
				maxScore = score;
			}
		}
		Count++;
		if(!dictionary.ContainsKey(score)){
			Queue<GridNode> queue = new Queue<GridNode>();
			queue.Enqueue(node);
			dictionary.Add(score, queue);
		}else{
			dictionary[score].Enqueue(node);
		}
	}
	
	public GridNode Dequeue(){
		//Debug.Log ("dequeued!");
		GridNode temp = null;
		if(Count > 0){
			temp = dictionary[minScore].Dequeue();
			Count--;
			if(dictionary[minScore].Count == 0){
				for(int i=minScore+1; i<=maxScore; i++){
				//	Debug.Log ("finding min");
					if(dictionary.ContainsKey(i) && dictionary[i].Count>0){
						minScore = i;
						break;
					}
				}
			}
		}
		return temp;
	}

	public void Clear(){
		dictionary.Clear();
	}

	public bool Contains(GridNode node){
		return dictionary[node.score].Contains(node);
	}
}
