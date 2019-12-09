using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour {
  public int id = 0;
  public bool isValid = true;
  public bool onOpenList = false;
  public bool onClosedList = false;
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
  public Material materialTarget;
  public Material materialOnClosedList;
  public Material materialOnOpenList;
  private Material materialValid;
  private Renderer currentRenderer;
  private Grid grid;

  void Awake() {
    currentRenderer = GetComponent<Renderer>();
  }

  void Start() {
    materialValid = currentRenderer.material;
    grid = FindObjectOfType<Grid>();
  }

  public void Reset() {
    parent = null;

    F = 0.0f;
    G = 0.0f;
    H = 0.0f;

    onOpenList = false;
    onClosedList = false;

    if (isValid) {
      currentRenderer.material = materialValid;
    } else {
      currentRenderer.material = materialInvalid;
    }
  }

  void FixedUpdate() {
    // Display Cell Information
    if (isValid) {
      if (F == 0.0f) {
        displayF.text = "";
      } else {
        displayF.text = "F: " + F.ToString("F2");
      }

      if (G == 0.0f) {
        displayG.text = "";
      } else {
        displayG.text = "G: " + G.ToString("F2");
      }

      if (H == 0.0f) {
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

  void OnTriggerStay(Collider other) {
    if (other.tag == "Obstacle") {
      if (isValid) {
        isValid = false;
        currentRenderer.material = materialInvalid;
      }
    } else if (other.tag == "Start")
      grid.startID = this.id;
    else if (other.tag == "Target")
      grid.targetID = this.id;
  }

  void OnTriggerExit(Collider other) {
    if (other.tag == "Obstacle") {
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
    H = Mathf.Abs(targetCell.transform.position.x - transform.position.x) + Mathf.Abs(targetCell.transform.position.z - transform.position.z);
  }

  public void SetToStart() {
    currentRenderer.material = materialStart;
  }

  public void SetToTarget() {
    //currentRenderer.material = materialTarget;
  }

  // Gets each neighbor by calculating its position, and checking if a cell exists
  // at those coordinates.
  // Could be replaced with a kernel-check over a twodimensional array.
  public ArrayList GetAdjacentCells(ArrayList allCells, int cellsPerRow) {
    ArrayList adjacentCells = new ArrayList();

    Cell neighbourUpperLeft = null;
    Cell neighbourUpper = null;
    Cell neighbourUpperRight = null;
    Cell neighbourLeft = null;
    Cell neighbourRight = null;
    Cell neighbourLowerLeft = null;
    Cell neighbourLower = null;
    Cell neighbourLowerRight = null;

    //check each neighbour
    if (id % cellsPerRow != 0 && IsInBounds(id + cellsPerRow - 1, allCells))
      neighbourUpperLeft = ((GameObject)allCells[id + cellsPerRow - 1]).GetComponent<Cell>();

    if (IsInBounds(id + cellsPerRow, allCells))
      neighbourUpper = ((GameObject)allCells[id + cellsPerRow]).GetComponent<Cell>();

    if ((id + 1) % cellsPerRow != 0 && IsInBounds(id + cellsPerRow + 1, allCells))
      neighbourUpperRight = ((GameObject)allCells[id + cellsPerRow + 1]).GetComponent<Cell>();

    if (id % cellsPerRow != 0)
      neighbourLeft = ((GameObject)allCells[id - 1]).GetComponent<Cell>();

    if ((id + 1) % cellsPerRow != 0)
      neighbourRight = ((GameObject)allCells[id + 1]).GetComponent<Cell>();

    if (id % cellsPerRow != 0 && IsInBounds(id - cellsPerRow - 1, allCells))
      neighbourLowerLeft = ((GameObject)allCells[id - cellsPerRow - 1]).GetComponent<Cell>();

    if (IsInBounds(id - cellsPerRow, allCells))
      neighbourLower = ((GameObject)allCells[id - cellsPerRow]).GetComponent<Cell>();

    if ((id + 1) % cellsPerRow != 0 && IsInBounds(id - cellsPerRow + 1, allCells))
      neighbourLowerRight = ((GameObject)allCells[id - cellsPerRow + 1]).GetComponent<Cell>();

    //if neighbor exists and is valid, add to neighbour-array
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

    //diagonal-edge-detection. if at an edge, remove from neighbour-array
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

  private bool IsCellInvalid(Cell inputCell) {
    return (inputCell != null && !inputCell.isValid);
  }

  private bool IsCellValid(Cell inputCell) {
    return (inputCell != null && inputCell.isValid);
  }

  private bool IsInBounds(int i, ArrayList array) {
    return (i >= 0 && i < array.Count);
  }
}