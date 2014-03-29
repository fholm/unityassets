using UnityEngine;
using System.Collections;

public class gcm_map_tester : MonoBehaviour {
	[SerializeField]
	int tex_size = 4096;
	
	[SerializeField]
	int map_size = 256;
	
	[SerializeField]
	int map_level = 0;
	
	[SerializeField]
	Vector2 position = new Vector2(2048, 2048);
	
	void Update () {
		renderer.material.SetVector("_Pos", position);	
		renderer.material.SetVector("_Input", new Vector4(1 << map_level, tex_size, map_size, 0));
	}
}
