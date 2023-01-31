using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Grid : MonoBehaviour {
  public Cell cellPrefab;
  public int cellsPerRow = 10;
  public int startCellID = 12;
  public int targetCellID = 87;
  
  [HideInInspector] public bool isCalculating;
  public List<Cell> allCells;
  
  private int numberOfCells;
  private List<Cell> openList;
  private List<Cell> closedList;
  private Cell startCell;
  private Cell targetCell;

  private void Start() {
    CreateCells();
  }

  private void CreateCells() {
    numberOfCells = cellsPerRow * cellsPerRow;
    allCells = new List<Cell>();

    int zOffset = 0;
    int xOffset = 0;
    int counter = 0;

    for (int i = 0; i < numberOfCells; i++) {
      counter += 1;
      xOffset += 1;

      Vector3 newPosition = transform.position + new Vector3(xOffset, 0, zOffset);
      Cell newCell = Instantiate(cellPrefab, newPosition, Quaternion.identity);
      newCell.id = i;
      allCells.Add(newCell);

      if (counter >= cellsPerRow) {
        counter = 0;
        xOffset = 0;
        zOffset += 1;
      }
    }
  }

  public void CalculatePath() {
    ResetAllCells();
    StartCoroutine(CalculatePathRoutine());
  }

  private void ResetAllCells() {
    foreach (Cell cell in allCells) {
      cell.Reset();
    }
  }

  private IEnumerator CalculatePathRoutine() {
    isCalculating = true;

    CreateStart(startCellID);
    CreateTarget(targetCellID);

    openList = new List<Cell>();
    closedList = new List<Cell>();

    Cell currentCell = startCell;
    AddCellToClosedList(currentCell);

    const float cycleDelay = 0f;
    int cycleCounter = 0;
    while (currentCell.id != targetCell.id) {
      yield return new WaitForSeconds(cycleDelay);

      // Safety-abort in case of endless loop
      cycleCounter++;
      if (cycleCounter >= numberOfCells) {
        Debug.Log("No Path Found");
        break;
      }

      // Add all cells adjacent to currentCell to openList
      foreach (Cell cell in GetAdjacentCells(currentCell)) {
        float tentativeG = currentCell.G + Vector3.Distance(currentCell.transform.position, cell.transform.position);
        // If cell is on closed list skip to next cycle
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

      // Get cell with lowest F value from openList, set it to currentCell
      float lowestFValue = 99999.9f;
      foreach (Cell cell in openList.Where(cell => cell.F < lowestFValue)) {
        lowestFValue = cell.F;
        currentCell = cell;
      }

      // Remove currentCell from openList, add to closedList
      openList.Remove(currentCell);
      AddCellToClosedList(currentCell);
    }

    // Get path
    List<Cell> path = new ();
    currentCell = targetCell;
    
    while (currentCell.id != startCell.id) {
      path.Add(currentCell);
      currentCell = currentCell.parent;
    }
    path.Add(currentCell);
    path.Reverse();

    // Draw path
    LineRenderer lineRenderer = GetComponent<LineRenderer>();
    lineRenderer.positionCount = path.Count;

    for (int i = 0; i < path.Count; i++) {
      lineRenderer.SetPosition(i, path[i].transform.position + new Vector3(0, 1, 0));
    }

    isCalculating = false;
  }
  
  private List<Cell> GetAdjacentCells(Cell currentCell) {
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
    startCell = allCells[id];
    startCell.SetToStart();
  }

  private void CreateTarget(int id) {
    targetCell = allCells[id];
  }
}