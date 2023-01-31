using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Cell : MonoBehaviour {
  public int id;
  public bool isValid = true;
  public bool onOpenList;
  public bool onClosedList;
  public Cell parent;
  public float F;
  public float G;
  public float H;
  public TextMesh displayF;
  public TextMesh displayG;
  public TextMesh displayH;
  public TextMesh displayID;
  public GameObject indicator;
  public GameObject indicatorDisplay;

  public Material materialInvalid;
  public Material materialStart;
  public Material materialOnClosedList;
  public Material materialOnOpenList;
  
  private Material materialValid;
  private Renderer currentRenderer;
  private Grid grid;

  private void Awake() {
    currentRenderer = GetComponent<Renderer>();
  }

  private void Start() {
    materialValid = currentRenderer.material;
    grid = FindObjectOfType<Grid>();
  }

  public void Reset() {
    parent = null;

    F = 0f;
    G = 0f;
    H = 0f;

    onOpenList = false;
    onClosedList = false;

    currentRenderer.material = isValid ? materialValid : materialInvalid;
  }

  private void FixedUpdate() {
    // Display Cell Information
    if (isValid) {
      if (F == 0f) {
        displayF.text = "";
      } else {
        displayF.text = "F: " + F.ToString("F2");
      }

      if (G == 0f) {
        displayG.text = "";
      } else {
        displayG.text = "G: " + G.ToString("F2");
      }

      if (H == 0f) {
        displayH.text = "";
      } else {
        displayH.text = "H: " + H.ToString("F2");
      }
    } else {
      displayF.text = "";
      displayG.text = "";
      displayH.text = "";
    }

    displayID.text = "" + id;

    Renderer indicatorRenderer = indicatorDisplay.GetComponent<Renderer>();
    if (parent != null) {
      indicatorRenderer.enabled = true;
      indicator.transform.LookAt(parent.transform.position);
    } else
      indicatorRenderer.enabled = false;
  }

  private void OnTriggerStay(Collider other) {
    if (other.CompareTag($"Obstacle")) {
      if (isValid) {
        isValid = false;
        currentRenderer.material = materialInvalid;
      }
    } else if (other.CompareTag($"Start")) {
      grid.startCellID = id;
    } else if (other.CompareTag($"Target")) {
      grid.targetCellID = id;
    }
  }

  private void OnTriggerExit(Collider other) {
    if (other.CompareTag($"Obstacle")) {
      if (!isValid) {
        isValid = true;
        currentRenderer.material = materialValid;
      }
    }
  }

  public void SetToClosedList() {
    onClosedList = true;
    onOpenList = false;
    currentRenderer.material = materialOnClosedList;
  }

  public void SetToOpenList() {
    onOpenList = true;
    currentRenderer.material = materialOnOpenList;
  }

  public void CalculateH(Cell targetCell) {
    Vector3 targetPosition = targetCell.transform.position;
    Vector3 ownPosition = transform.position;
    float manhattanDistance = Mathf.Abs(targetPosition.x - ownPosition.x) + Mathf.Abs(targetPosition.z - ownPosition.z);
    H = manhattanDistance;
  }

  public void SetToStart() {
    currentRenderer.material = materialStart;
  }

  // Gets each neighbor by calculating its position, and checking if a cell exists at those coordinates.
  // Could be replaced with a kernel-check over a two-dimensional array.
  public List<Cell> GetAdjacentCells(List<Cell> allCells, int cellsPerRow) {
    List<Cell> adjacentCells = new ();

    Cell neighbourUpperLeft = null;
    Cell neighbourUpper = null;
    Cell neighbourUpperRight = null;
    Cell neighbourLeft = null;
    Cell neighbourRight = null;
    Cell neighbourLowerLeft = null;
    Cell neighbourLower = null;
    Cell neighbourLowerRight = null;

    // Check each neighbour
    if (id % cellsPerRow != 0 && IsInBounds(id + cellsPerRow - 1, allCells))
      neighbourUpperLeft = (allCells[id + cellsPerRow - 1]).GetComponent<Cell>();

    if (IsInBounds(id + cellsPerRow, allCells))
      neighbourUpper = (allCells[id + cellsPerRow]).GetComponent<Cell>();

    if ((id + 1) % cellsPerRow != 0 && IsInBounds(id + cellsPerRow + 1, allCells))
      neighbourUpperRight = (allCells[id + cellsPerRow + 1]).GetComponent<Cell>();

    if (id % cellsPerRow != 0)
      neighbourLeft = (allCells[id - 1]).GetComponent<Cell>();

    if ((id + 1) % cellsPerRow != 0)
      neighbourRight = (allCells[id + 1]).GetComponent<Cell>();

    if (id % cellsPerRow != 0 && IsInBounds(id - cellsPerRow - 1, allCells))
      neighbourLowerLeft = (allCells[id - cellsPerRow - 1]).GetComponent<Cell>();

    if (IsInBounds(id - cellsPerRow, allCells))
      neighbourLower = (allCells[id - cellsPerRow]).GetComponent<Cell>();

    if ((id + 1) % cellsPerRow != 0 && IsInBounds(id - cellsPerRow + 1, allCells))
      neighbourLowerRight = (allCells[id - cellsPerRow + 1]).GetComponent<Cell>();
    
    // If neighbor exists and is valid, add to neighbour-array
    if (IsCellValid(neighbourUpperLeft))
      adjacentCells.Add(neighbourUpperLeft);

    if (IsCellValid(neighbourUpper))
      adjacentCells.Add(neighbourUpper);

    if (IsCellValid(neighbourUpperRight))
      adjacentCells.Add(neighbourUpperRight);

    if (IsCellValid(neighbourLeft))
      adjacentCells.Add(neighbourLeft);

    if (IsCellValid(neighbourRight))
      adjacentCells.Add(neighbourRight);

    if (IsCellValid(neighbourLowerLeft))
      adjacentCells.Add(neighbourLowerLeft);

    if (IsCellValid(neighbourLower))
      adjacentCells.Add(neighbourLower);

    if (IsCellValid(neighbourLowerRight))
      adjacentCells.Add(neighbourLowerRight);

    // Diagonal edge detection. If at an edge, remove from neighbour-array
    if (IsCellInvalid(neighbourRight)) {
      adjacentCells.Remove(neighbourUpperRight);
      adjacentCells.Remove(neighbourLowerRight);
    }

    if (IsCellInvalid(neighbourLeft)) {
      adjacentCells.Remove(neighbourUpperLeft);
      adjacentCells.Remove(neighbourLowerLeft);
    }

    if (IsCellInvalid(neighbourUpper)) {
      adjacentCells.Remove(neighbourUpperRight);
      adjacentCells.Remove(neighbourUpperLeft);
    }

    if (IsCellInvalid(neighbourLower)) {
      adjacentCells.Remove(neighbourLowerRight);
      adjacentCells.Remove(neighbourLowerLeft);
    }

    return adjacentCells;
  }

  private static bool IsCellInvalid(Cell inputCell) {
    return (inputCell && !inputCell.isValid);
  }

  private static bool IsCellValid(Cell inputCell) {
    return (inputCell && inputCell.isValid);
  }

  private static bool IsInBounds(int i, ICollection cells) {
    return (i >= 0 && i < cells.Count);
  }
}