using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStack : MonoBehaviour {
	public Color32[] gameColors = new Color32[4];

	private const float BOUND_SIZE = 3.5f;
	private const float STACK_MOVING_SPEED = 5.0f;
	private const float ERROR_MARGIN = 0.1f;
	private const float STACK_BOUNDS_GAIN = 0.25f;
	private const int COMBO_START_GAIN = 5; 

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
	private bool gameOver;

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
		gameOver = false;
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
		if (gameOver) {
			return;
		}

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
		theStack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

		ColorMesh(theStack[stackIndex].GetComponent<MeshFilter>().mesh);
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
			} else {
				if (combo > COMBO_START_GAIN) {
					stackBounds.x += STACK_BOUNDS_GAIN;
					float middle = lastTilePosition.x + t.localPosition.x / 2;
					t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
					t.localPosition = new Vector3(middle - lastTilePosition.x / 2, scoreCount, lastTilePosition.z);
				}

				combo++;
				t.localPosition = lastTilePosition + Vector3.up;
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
			} else {
				if (combo > COMBO_START_GAIN) {
					stackBounds.y += STACK_BOUNDS_GAIN;
					float middle = lastTilePosition.z + t.localPosition.z / 2;
					t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
					t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - lastTilePosition.z / 2);
				}

				combo++;
				t.localPosition = lastTilePosition + Vector3.up;
			}
		}

		secondaryPosition = isMovingOnX ? t.localPosition.x : t.localPosition.z;

		isMovingOnX = !isMovingOnX;
		return true;
	}

	private void ColorMesh(Mesh mesh) {
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = new Color32[vertices.Length];
		float f = Mathf.Sin(scoreCount * 0.25f);

		for (int i = 0; i < vertices.Length; i++) {
			colors[i] = Lerp4(gameColors[0], gameColors[1], gameColors[2], gameColors[3], f);
		}

		mesh.colors32 = colors;
	}

	private Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d, float t) {
		if (t < 0.33f) {
			return Color32.Lerp(a, b, t / 0.33f);
		} else if (t < 0.66f) {
			return Color32.Lerp(b, c, (t - 0.33f) / 0.33f);
		} else {
			return Color32.Lerp(c, d, (t - 0.66f) / 0.66f);
		}
	}

	private void EndGame() {
		gameOver = true;
		theStack[stackIndex].AddComponent<Rigidbody>();
	}
}
