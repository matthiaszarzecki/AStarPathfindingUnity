using UnityEngine;

public class InputHandler : MonoBehaviour {
  [HideInInspector] public GameObject grabbedCube;
  
  private Grid grid;
  private Camera gameCamera;

  private void Start() {
    grid = FindObjectOfType<Grid>();
    gameCamera = Camera.main;
  }

  private void Update() {
    if (grabbedCube && !grid.isCalculating) {
      if (Input.GetKey(KeyCode.Mouse0)) {
        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100)) {
          grabbedCube.transform.position = FindClosestCellPosition(hit.point) + new Vector3(0, 1, 0);
        }
      } else if (Input.GetKeyUp(KeyCode.Mouse0)) {
        grabbedCube = null;
        grid.CalculatePath();
      }
    }
  }

  private Vector3 FindClosestCellPosition(Vector3 startPosition) {
    Cell closestCell = null;
    float distance = Mathf.Infinity;

    foreach (Cell cell in grid.allCells) {
      Vector3 diff = cell.transform.position - startPosition;
      float curDistance = diff.sqrMagnitude;
      if (curDistance < distance) {
        closestCell = cell;
        distance = curDistance;
      }
    }
    return closestCell.transform.position;
  }
}