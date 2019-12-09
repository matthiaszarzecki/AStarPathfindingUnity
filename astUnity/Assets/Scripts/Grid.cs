using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
  public GameObject cellPrefab;
  public int numberOfCells = 100;
  public int cellsPerRow = 10;
  public int startID = 12;
  public int targetID = 87;
  public bool isCalculating = false;

  public ArrayList allCells;
  private ArrayList openList;
  private ArrayList closedList;
  private Cell startCell;
  private Cell targetCell;

  void Start() {
    CreateCells();
  }

  private void CreateCells() {
    allCells = new ArrayList();

    int zOffset = 0;
    int xOffset = 0;
    int counter = 0;

    for (int i = 0; i < numberOfCells; i++) {
      counter += 1;
      xOffset += 1;
      GameObject newCell = (GameObject)Instantiate(cellPrefab, new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z + zOffset), transform.rotation);
      newCell.GetComponent<Cell>().id = i;
      allCells.Add(newCell);

      if (counter >= cellsPerRow) {
        counter = 0;
        xOffset = 0;
        zOffset += 1;
      }
    }
  }

  public void CalculatePathExternal() {
    // Reset all cells
    foreach (GameObject gameObject in allCells) {
      gameObject.GetComponent<Cell>().Reset();
    }

    StartCoroutine(CalculatePath());
  }

  private IEnumerator CalculatePath() {
    isCalculating = true;

    CreateStart(startID);
    CreateTarget(targetID);

    openList = new ArrayList();
    closedList = new ArrayList();

    Cell currentCell = startCell;
    AddCellToClosedList(currentCell);

    float cycleDelay = 0.0f;
    int cycleCounter = 0;
    while (currentCell.id != targetCell.id) {
      yield return new WaitForSeconds(cycleDelay);

      //safety-abort in case of endless loop
      cycleCounter++;
      if (cycleCounter >= numberOfCells) {
        Debug.Log("No Path Found");
        break;
      }

      //add all cells adjacent to currentCell to openList
      foreach (Cell cell in GetAdjacentCells(currentCell)) {
        float tentativeG = currentCell.G + Vector3.Distance(currentCell.transform.position, cell.transform.position);
        //if cell is on closed list skip to next cycle
        if (cell.onClosedList && tentativeG > cell.G) {
          continue;
        }

        if (!cell.onOpenList || tentativeG < cell.G) {
          cell.CalculateH(targetCell);
          cell.G = tentativeG;
          cell.F = cell.G + cell.H;
          cell.parent = currentCell;

          if (!cell.onClosedList)
            AddCellToOpenList(cell);
        }
      }

      yield return new WaitForSeconds(cycleDelay);

      //get cell with lowest F value from openList, set it to currentCell
      float lowestFValue = 99999.9f;
      foreach (Cell cell in openList) {
        if (cell.F < lowestFValue) {
          lowestFValue = cell.F;
          currentCell = cell;
        }
      }

      //remove currentCell from openList, add to closedList
      openList.Remove(currentCell);
      AddCellToClosedList(currentCell);
    }

    //get path
    ArrayList path = new ArrayList();
    currentCell = targetCell;
    while (currentCell.id != startCell.id) {
      path.Add(currentCell);
      currentCell = currentCell.parent;
    }
    path.Add(currentCell);
    path.Reverse();

    //draw path
    LineRenderer lineRenderer = GetComponent<LineRenderer>();
    lineRenderer.SetVertexCount(path.Count);
    int i = 0;
    foreach (Cell cell in path) {
      lineRenderer.SetPosition(i, cell.transform.position + new Vector3(0, 1, 0));
      i++;
    }

    isCalculating = false;
  }

  private ArrayList GetAdjacentCells(Cell currentCell) {
    return currentCell.GetAdjacentCells(allCells, cellsPerRow);
  }

  private void AddCellToClosedList(Cell currentCell) {
    closedList.Add(currentCell);
    currentCell.SetToClosedList();
  }

  private void AddCellToOpenList(Cell currentCell) {
    openList.Add(currentCell);
    currentCell.SetToOpenList();
  }

  private void CreateStart(int id) {
    GameObject tempStartCell = (GameObject)allCells[id];
    startCell = tempStartCell.GetComponent<Cell>();
    startCell.SetToStart();
  }

  private void CreateTarget(int id) {
    GameObject tempTargetCell = (GameObject)allCells[id];
    targetCell = tempTargetCell.GetComponent<Cell>();
    targetCell.SetToTarget();
  }
}