using UnityEngine;
using System.Collections;

public class ZeroLayer: MonoBehaviour
{
	public 	GameObject 	otherCube;
	private Grid 		grid;

	void Start()
	{
		grid = FindObjectOfType<Grid>();
	}
	
	void Update()
	{
		if(!grid.isCalculating)
		{
			if(Input.GetKey(KeyCode.Mouse0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				
				if(Physics.Raycast (ray, out hit, 100))
				{
					if(otherCube)
					{
						otherCube.GetComponent<Collider>().enabled = false;
						otherCube.transform.position = FindClosestCell(hit.point) + new Vector3(0, 1, 0);//hit.point;
					}
				}
			}
			else if(Input.GetKeyUp(KeyCode.Mouse0))
			{
				if(otherCube)
				{
					otherCube.GetComponent<Collider>().enabled = true;
					otherCube = null;

					grid.CalculatePathExternal();
				}
			}
		}
	}

	private Vector3 FindClosestCell(Vector3 startPos)
	{
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = startPos;
		foreach (GameObject go in grid.allCells)
		{
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance)
			{
				closest = go;
				distance = curDistance;
			}
		}
		return closest.transform.position;
	}
}