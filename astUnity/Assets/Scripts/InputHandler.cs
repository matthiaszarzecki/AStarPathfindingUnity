using UnityEngine;

public class InputHandler : MonoBehaviour {
  public GameObject otherCube;
  
  private Grid grid;
  private Camera gameCamera;

  private void Start() {
    grid = FindObjectOfType<Grid>();
    gameCamera = Camera.main;
  }

  private void Update() {
    if (otherCube && !grid.isCalculating) {
      if (Input.GetKey(KeyCode.Mouse0)) {
        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100)) {
          otherCube.GetComponent<Collider>().enabled = false;
          otherCube.transform.position = FindClosestCellPosition(hit.point) + new Vector3(0, 1, 0);
        }
      } else if (Input.GetKeyUp(KeyCode.Mouse0)) {
        otherCube.GetComponent<Collider>().enabled = true;
        otherCube = null;

        grid.CalculatePathExternal();
      }
    }
  }

  private Vector3 FindClosestCellPosition(Vector3 startPosition) {
    GameObject closest = null;
    float distance = Mathf.Infinity;
    Vector3 position = startPosition;
    
    foreach (GameObject cell in grid.allCells) {
      Vector3 diff = cell.transform.position - position;
      float curDistance = diff.sqrMagnitude;
      if (curDistance < distance) {
        closest = cell;
        distance = curDistance;
      }
    }
    return closest.transform.position;
  }
}