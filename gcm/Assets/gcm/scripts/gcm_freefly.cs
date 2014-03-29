using UnityEngine;
using System.Collections;

public class gcm_freefly : MonoBehaviour {
	float x_rot = 0f;
	float y_rot = 0f;
	float normalSpeed = 20f;
	float fastSpeed = 40f;
	float rotSpeed = 2.5f;
	
	[SerializeField]
	bool lock_cursor = false;
	
	void Update () {
		if (lock_cursor) {
			Screen.lockCursor = true;
			Screen.showCursor = false;
		}
		
		x_rot += -Input.GetAxis("Mouse Y") * rotSpeed;
		y_rot += Input.GetAxis("Mouse X") * rotSpeed;
		transform.rotation = Quaternion.Euler(x_rot, y_rot, 0);
		
		Vector3 movement = Vector3.zero;
		float speed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : normalSpeed;
		
		if (Input.GetKey(KeyCode.W)) movement.z += 1;
		if (Input.GetKey(KeyCode.S)) movement.z -= 1;
		if (Input.GetKey(KeyCode.A)) movement.x -= 1;
		if (Input.GetKey(KeyCode.D)) movement.x += 1;
		if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Space)) movement.y += 1;
		if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.LeftControl)) movement.y -= 1;
		
		transform.Translate(movement.normalized * speed * Time.deltaTime);
		
	}
}