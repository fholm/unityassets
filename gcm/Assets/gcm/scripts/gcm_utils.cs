using UnityEngine;
using System.Collections.Generic;

public static class gcm_utils {
	public static GameObject mesh_temp_display (Mesh m) {
		GameObject go = new GameObject("_temp_display_" + m.name);
		go.transform.position = new Vector3();
		go.transform.rotation = Quaternion.identity;
		go.AddComponent<MeshFilter>().sharedMesh = m;
		go.AddComponent<MeshRenderer>();
		go.hideFlags = HideFlags.DontSave;
		return go;
	}
	
	public static Mesh create_trim (int2 tb_origin, int2 lr_origin, int length) {
		Mesh m = new Mesh();
		List<Vector3> vs = new List<Vector3>();
		List<int> tr = new List<int>();
		
		for (int x = 0; x < length; ++x) {
			vs.Add(new Vector3(tb_origin.x + x, 0, tb_origin.y));
			vs.Add(new Vector3(tb_origin.x + x, 0, tb_origin.y + 1));
			
			if (x > 0) {
				tr.Add(vs.Count - 1);
				tr.Add(vs.Count - 2);
				tr.Add(vs.Count - 3);
				tr.Add(vs.Count - 2);
				tr.Add(vs.Count - 4);
				tr.Add(vs.Count - 3);
			}
		}
		
		for (int y = 0; y < length - 1; ++y) {
			vs.Add(new Vector3(lr_origin.x, 0, lr_origin.y + y));
			vs.Add(new Vector3(lr_origin.x + 1, 0, lr_origin.y + y));
			
			if (y > 0) {
				tr.Add(vs.Count - 2);
				tr.Add(vs.Count - 1);
				tr.Add(vs.Count - 3);
				tr.Add(vs.Count - 4);
				tr.Add(vs.Count - 2);
				tr.Add(vs.Count - 3);
			}
		}
		
		m.vertices = vs.ToArray();
		m.triangles = tr.ToArray();
		m.name = string.Format("trim");
		m.bounds = new Bounds(Vector3.zero, new Vector3(4096, 4096, 4096));
		return m;
	}
	
	public static int[] create_grid_indices (int rows, int cols) {
		int[] tr = new int[(rows - 1) * (cols - 1) * 6];
		
        int t = 0;
        for (int z = 0; z < rows - 1; z++)
        {
            for (int x = 0; x < cols - 1; x++)
            {
                tr[t + 0] = (z * cols) + x;
                tr[t + 1] = ((z + 1) * cols) + x;
                tr[t + 2] = (z * cols) + x + 1;
                tr[t + 3] = ((z + 1) * cols) + x;
                tr[t + 4] = ((z + 1) * cols) + x + 1;
                tr[t + 5] = (z * cols) + x + 1;
				
                t += 6;
            }
        }
		
		return tr;
	}
	
	public static Mesh create_grid (int rows, int cols) {
		Mesh m = new Mesh();
		Vector3[] vs = new Vector3[rows * cols];
		int[] tr = new int[(rows - 1) * (cols - 1) * 6];
		
		int v = 0;
		for (int z = 0; z < rows; z++) {
			for (int x = 0; x < cols; x++) {
				vs[v++] = new Vector3(x, 0, z);
			}
		}

        int t = 0;
        for (int z = 0; z < rows - 1; z++)
        {
            for (int x = 0; x < cols - 1; x++)
            {
                tr[t + 0] = (z * cols) + x;
                tr[t + 1] = ((z + 1) * cols) + x;
                tr[t + 2] = (z * cols) + x + 1;
                tr[t + 3] = ((z + 1) * cols) + x;
                tr[t + 4] = ((z + 1) * cols) + x + 1;
                tr[t + 5] = (z * cols) + x + 1;
				
                t += 6;
            }
        }
		
		m.vertices = vs;
		m.triangles = tr;
		m.name = string.Format("grid_{0}x{1}", rows, cols);
		m.bounds = new Bounds(Vector3.zero, new Vector3(4096, 4096, 4096));
		
		return m;
	}
}
