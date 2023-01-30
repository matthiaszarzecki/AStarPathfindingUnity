using UnityEngine;

public class Obstacle : MonoBehaviour {
  private InputHandler inputHandler;

  private void Start() {
    inputHandler = FindObjectOfType<InputHandler>();
  }

  private void OnMouseOver() {
    if (Input.GetKey(KeyCode.Mouse0))
      inputHandler.otherCube = gameObject;
  }
}