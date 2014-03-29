using UnityEngine;
using System.Collections.Generic;

public class gcm_wireframe : MonoBehaviour {
	
	struct wireframe_target {
		public readonly Mesh mesh;
		public readonly Color color;
		public readonly Matrix4x4 matrix;
		
		public wireframe_target(Mesh mesh, Matrix4x4 matrix, Color color) {
			this.mesh = mesh;
			this.matrix = matrix;
			this.color = color;
		}
	}
	
	static Queue<wireframe_target> targets = new Queue<wireframe_target>();
	
	public static void Draw(Mesh m, Vector3 position, Color color) {
		Matrix4x4 mx = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
		
		lock (targets) {
			targets.Enqueue(new wireframe_target(m, mx, color));
		}
	}
	
	[SerializeField]
	Shader shader;
	
	Material material;
	
	void Awake() {
		material = new Material(shader);
	}
	
	void OnPostRender() {
		GL.wireframe = true;
		
		while(targets.Count > 0) {
			wireframe_target t = targets.Dequeue();
			material.SetPass(0);
			material.SetColor("_Color", t.color);
			Graphics.DrawMeshNow(t.mesh, t.matrix);
		}
		
		GL.wireframe = false;
	}
}
