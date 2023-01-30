using UnityEngine;

public class Obstacle : MonoBehaviour {
  private ZeroLayer zeroLayer;

  private void Start() {
    zeroLayer = FindObjectOfType<ZeroLayer>();
  }

  void OnMouseOver() {
    if (Input.GetKey(KeyCode.Mouse0))
      zeroLayer.otherCube = gameObject;
  }
}