using UnityEngine;
using System.Collections;
using System;

public class Heap<T> where T : IComparable<T>{
	public T[] items;
	public int Count;
	// Use this for initialization
	public Heap(int size){
		items = new T[size];
		Count = 0;
		
	}
	
	public void insert(T item){
		int current = Count;
		items[current] = item;
		if(Count != 0){
			int parent;
			while(items[current].CompareTo (items[parent = parentOf (current)]) < 0){
				swapItem (current, parent);
				current = parent;
			}
		}
		Count++;
	}
	
	public T extract(){
		T temp = items[0];
		items[0] = default(T);
		int current = 0;
		while(items[leftOf(current)] != null || items[rightOf(current)] != null){
			if(items[leftOf(current)] == null){
				swapItem(rightOf(current), current);
				current = rightOf(current);
			}else if(items[rightOf(current)] == null){
				swapItem(leftOf(current), current);
				current = leftOf(current);
			}else{
				if(items[leftOf(current)].CompareTo(items[rightOf(current)])<0){
					swapItem(leftOf(current), current);
					current = leftOf(current);
				}else{
					swapItem(rightOf(current), current);
					current = rightOf(current);
				}
			}
		}
		return temp;
	}
	
	public void Clear(){
		for(int i=0; i<Count; i++){
			items[i] = default(T);
		}
		Count = 0;
	}
	
	private void swapItem(int first, int second){
		T temp = items[first];
		items[first] = items[second];
		items[second] = temp;
	}
	
	private int leftOf(int current){
		return 2*current+1;
	}
	
	private int rightOf(int current){
		return 2*current+2;
	}
	
	private int parentOf(int current){
		return (current-1)/2;
	}
}
/*
 * public void insert(T item){
		int current = Count;
		items[current] = item;
		if(Count != 0){
			int parent;
			while(items[current].CompareTo (items[parent = parentOf (current)]) < 0){
				swapItem (current, parent);
				current = parent;
			}
		}
		Count++;
	}
 * 	public T extract(){
		T temp = items[0];
		items[0] = default(T);
		Count--;
		int current = 0;
		int left, right;
		do {
			left = leftOf(current);
			right = rightOf(current);
			if(items[left] == null) {
				if(items[right] == null) {
					return temp;
				} else {
					swapItem(right, current);
					current = right;
				}
			} else {
				if(items[right] == null || items[left].CompareTo (items[right]) < 0) {
					swapItem(left, current);
					current = left;
				} else {
					swapItem (right,current);
					current = right;
				}
			}
		} while(true);
	}
 */