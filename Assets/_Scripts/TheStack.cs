using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStack : MonoBehaviour {
	private const float BOUND_SIZE = 3.5f;

	private GameObject[] theStack;
	private int stackIndex;
	private int scoreCount;

	void Start() {
		theStack = new GameObject[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) {
			theStack[i] = transform.GetChild(i).gameObject;
		}

		stackIndex = transform.childCount - 1;
		scoreCount = 0;
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			if (PlaceTile()) {
				scoreCount++;
				SpawnTile();
			} else {
				EndGame();
			}
		}
	}

	private void SpawnTile() {
		stackIndex--;

		if (stackIndex < 0) {
			stackIndex = transform.childCount - 1;
		}

		theStack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
	}

	private bool PlaceTile() {
		return true;
	}

	private void EndGame() {
		
	}
}
