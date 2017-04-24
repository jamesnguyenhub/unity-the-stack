using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStack : MonoBehaviour {
	private const float BOUND_SIZE = 3.5f;
	private const float STACK_MOVING_SPEED = 5.0f;
	private const float ERROR_MARGIN = 0.1f;

	private GameObject[] theStack;
	private Vector2 stackBounds;
	private int stackIndex;
	private int scoreCount;
	private int combo;
	private float tileTransition;
	private float tileSpeed;
	private bool isMovingOnX;
	private float secondaryPosition;
	private Vector3 desiredPosition;
	private Vector3 lastTilePosition;

	void Start() {
		theStack = new GameObject[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) {
			theStack[i] = transform.GetChild(i).gameObject;
		}

		stackBounds = new Vector2(BOUND_SIZE, BOUND_SIZE);
		stackIndex = transform.childCount - 1;
		scoreCount = 0;
		combo = 0;
		tileTransition = 0.0f;
		tileSpeed = 2.5f;
		isMovingOnX = true;
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

		MoveTile();

		transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);
	}

	private void MoveTile() {
		tileTransition += Time.deltaTime * tileSpeed;

		if (isMovingOnX) {
			theStack[stackIndex].transform.localPosition = 
				new Vector3(Mathf.Sin(tileTransition) * BOUND_SIZE, scoreCount, secondaryPosition);
		} else {
			theStack[stackIndex].transform.localPosition = 
				new Vector3(secondaryPosition, scoreCount, Mathf.Sin(tileTransition) * BOUND_SIZE);
		}
	}

	private void SpawnTile() {
		lastTilePosition = theStack[stackIndex].transform.localPosition;
		stackIndex--;

		if (stackIndex < 0) {
			stackIndex = transform.childCount - 1;
		}

		desiredPosition = Vector3.down * scoreCount;
		theStack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
	}

	private bool PlaceTile() {
		Transform t = theStack[stackIndex].transform;

		if (isMovingOnX) {
			float deltaX = lastTilePosition.x - t.position.x;
			if (Mathf.Abs(deltaX) > ERROR_MARGIN) {
				combo = 0;
				stackBounds.x -= Mathf.Abs(deltaX);
				if (stackBounds.x <= 0) {
					return false;
				}

				float middle = lastTilePosition.x + t.localPosition.x / 2;
				t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
				t.localPosition = new Vector3(middle - lastTilePosition.x / 2, scoreCount, lastTilePosition.z);
			}
		} else {
			float deltaZ = lastTilePosition.z - t.position.z;
			if (Mathf.Abs(deltaZ) > ERROR_MARGIN) {
				combo = 0;
				stackBounds.y -= Mathf.Abs(deltaZ);
				if (stackBounds.y <= 0) {
					return false;
				}

				float middle = lastTilePosition.z + t.localPosition.z / 2;
				t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
				t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - lastTilePosition.z / 2);
			}
		}

		secondaryPosition = isMovingOnX ? t.localPosition.x : t.localPosition.z;

		isMovingOnX = !isMovingOnX;
		return true;
	}

	private void EndGame() {
		Debug.Log("LOSE");
	}
}
