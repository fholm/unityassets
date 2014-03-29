using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public struct int2 {
	public int x;
	public int y;
	
	public Vector2 vector {
		get { return new Vector2(x, y); }
	}
	
	public Vector3 v3_xz {
		get { return new Vector3(x, 0, y); }
	}
	
	public int2 (Vector2 v) {
		x = Mathf.FloorToInt(v.x);
		y = Mathf.FloorToInt(v.y);
	}
	
	public int2 (int _x, int _y) {
		x = _x;
		y = _y;
	}
	
	public int2 (float _x, float _y) {
		x = (int) _x;
		y = (int) _y;
	}
	
	public override string ToString () {
		return string.Format ("[int2: x={0}, y={1}]", x.ToString(), y.ToString());
	}
	
	public static int2 xz(Vector3 v) {
		return new int2(v.x, v.z);
	}
	
	public static int2 operator -(int2 l, int2 r) {
		return new int2(l.x - r.x, l.y - r.y);
	}
	
	public static int2 operator +(int2 l, int2 r) {
		return new int2(l.x + r.x, l.y + r.y);
	}
}

struct gcm_collider {
	public Mesh m;
	public int2 origin;
}

[System.Serializable]
class gcm_level {
	public int scale;
	public int2 origin;
	public Material material;
	public RenderTexture texture;
	
	public int size {
		get { return (scale * gcm_terrain.W * 4) + (scale *2); }
	}
	
	public int2 min {
		get { return origin; }
	}
	
	public int2 max {
		get { return origin + new int2(size, size); }
	}
	
	public int2 min_inner {
		get {  
			int v = gcm_terrain.W * scale;
			return origin + new int2(v, v);
		}
	}
	
	public int2 max_inner {
		get {
			int v = (scale * gcm_terrain.W * 3) + (scale *2);
			return origin + new int2(v, v);
		}
	}
}

public class gcm_terrain : MonoBehaviour {
	public const int SIZE = 63;
	public const int SIZE_PW2 = SIZE + 1;
	public const float SIZE_F = SIZE;
	public const float SIZE_FPW2 = SIZE + 1;
	public const int M = (SIZE + 1) / 4;
	public const int M_1 = M - 1;
	public const int W = M - 1;
	public const int M2 = M * 2;
	public const int L = 7;
	public const int COLLIDER_SIZE = 128;
	
	const int L_XMIN_YMIN = 0;
	const int L_XMIN_YMAX = 1;
	const int L_XMAX_YMIN = 2;
	const int L_XMAX_YMAX = 3;
	
	Mesh grid_MxM;
	Mesh grid_Mx3;
	Mesh grid_3xM;
	Mesh[] grid_L;
	bool initialized = false;
	int2 movement = new int2();
	int2 current_pos = new int2();
	Material level_map_material;
	MaterialPropertyBlock level_property_block;
	
	[SerializeField]
	Vector2 terrain_offset = new Vector2(0, 0);
	
	[SerializeField]
	Shader level_shader;
	
	[SerializeField]
	Texture2D heightmap;
	
	[SerializeField]
	gcm_level[] levels;
	
	[SerializeField]
	Shader level_map_shader;
	
	[SerializeField]
	Transform target;
	
	
	MeshFilter mesh_filter {
		get { return GetComponent<MeshFilter>(); }
	}
	
	MeshRenderer mesh_renderer {
		get { return GetComponent<MeshRenderer>(); }
	}
	
	void create_textures () {
		levels = new gcm_level[L];
		level_property_block = new MaterialPropertyBlock();
		int2 origin = new int2();
		origin.x -= M;
		origin.y -= M;
		
		for (int i = 0; i < L; ++i) {
			origin.x -= W * (1 << i);
			origin.y -= W * (1 << i);
			
			levels[i] = new gcm_level();
			levels[i].texture = new RenderTexture(SIZE + 1, SIZE + 1, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
			//levels[i].texture.filterMode = FilterMode.Point;
			levels[i].scale = 1 << i;
			levels[i].origin = origin;
			levels[i].material = new Material(level_shader);
			levels[i].material.name = "gcm_level_" + i;
			levels[i].material.SetFloat("_tex_width", SIZE + 1);
			levels[i].material.SetTexture("_MainTex", levels[i].texture);
		}
	}
	
	void Start () {
		grid_MxM = gcm_utils.create_grid(M, M);
		grid_Mx3 = gcm_utils.create_grid(M, 3);
		grid_3xM = gcm_utils.create_grid(3, M);
		
		grid_L = new Mesh[4];
		grid_L[L_XMIN_YMIN] = gcm_utils.create_trim(new int2(), new int2(0, 1), W + W + 3);
		grid_L[L_XMAX_YMIN] = gcm_utils.create_trim(new int2(), new int2(W + W + 1, 1), W + W + 3);
		grid_L[L_XMIN_YMAX] = gcm_utils.create_trim(new int2(0, W + W + 1), new int2(0, 0), W + W + 3);
		grid_L[L_XMAX_YMAX] = gcm_utils.create_trim(new int2(0, W + W + 1), new int2(W + W + 1, 0), W + W + 3);
		
		level_map_material = new Material(level_map_shader);
		collider_rt =  new RenderTexture(COLLIDER_SIZE, COLLIDER_SIZE, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
		collider_rt.Create();
		
		create_textures();
		StartCoroutine(init());
		
		//read_collision_data(2048, 2048);
		
		m = gcm_utils.create_grid(128, 128);
		Vector3[] vs = m.vertices;
		
		for (int y = 0; y < 128; ++y) {
			for (int x = 0; x < 128; ++x) {
				vs[(y * 128) + x].y = (heightmap.GetPixel(2048 + x, 2048 + y).r * 64);
			}
		}
		
		m.vertices = vs;
		m.RecalculateBounds();
		m.RecalculateNormals();
		
		go = new GameObject();
		var mf = go.AddComponent<MeshFilter>();
		var mr = go.AddComponent<MeshRenderer>();
		var mc = go.AddComponent<MeshCollider>();
		mf.sharedMesh = m;
		mc.sharedMesh = m;
		mr.enabled = true;
		go.transform.position = new Vector3(0, 0, 0);
	}
	
	GameObject go;
	Mesh m;
	
	[SerializeField]
	Material go_mat;
	
	void update_movement (Vector3 position) {
		int2 new_pos = int2.xz(transform.InverseTransformPoint(position));
		movement += current_pos - new_pos;
		current_pos = new_pos;
	}
	
	void Update () {
		update_movement(target.position);
		
		for (int i = 0; i < L; ++i) {
			draw_level(i);
		}
	}
	
	System.Collections.IEnumerator init() {
		yield return null;
		yield return null;
		yield return null;
			
		if (initialized == false) {
			for (int i = 0; i < L; ++i) {
				update_clip_map(levels[i]);
			}
			
			initialized = true;
		}
	}
	
	MaterialPropertyBlock get_properties (int scale, float x, float y, float tex_x, float tex_y) {
		level_property_block.Clear();
		level_property_block.AddVector("_slab_scale", new Vector4(scale, scale));
		level_property_block.AddVector("_slab_origin", new Vector4(x, y));
		level_property_block.AddVector("_slab_tex_origin", new Vector4(tex_x, tex_y));
		return level_property_block;
	}
	
	void draw_mesh(gcm_level l, Mesh m, float pos_x, float pos_y, float tex_x, float tex_y) {
		Graphics.DrawMesh(m, new Vector4(pos_x, 0, pos_y), Quaternion.identity, l.material, 0, null, 0, get_properties(l.scale, pos_x, pos_y, tex_x, tex_y));
	}
	
	void draw_center (gcm_level lv) {
		float t = W / SIZE_FPW2;
		float texel = 1f / SIZE_FPW2;
		
		// 2 x trim around center
		draw_mesh(lv, grid_L[L_XMIN_YMIN], lv.origin.x + W, lv.origin.y + W, t, t);
		draw_mesh(lv, grid_L[L_XMAX_YMAX], lv.origin.x + W, lv.origin.y + W, t, t);
		
		// 4 x MxM blocks in center
		draw_mesh(lv, grid_MxM, lv.origin.x + M, lv.origin.y + M, t + texel, t + texel);
		draw_mesh(lv, grid_MxM, lv.origin.x + M + W, lv.origin.y + M, t + texel + t, t + texel);
		draw_mesh(lv, grid_MxM, lv.origin.x + M, lv.origin.y + M + W, t + texel, t + texel + t);
		draw_mesh(lv, grid_MxM, lv.origin.x + M + W, lv.origin.y + M + W, t + texel + t, t + texel + t);
	}
	
	void draw_level (int level) {
		gcm_level lv = levels[level];
		bool update = false;
		
		if (level > 0) {
			var min = levels[level - 1].min;
			var max = levels[level - 1].max;
			var min_inner = lv.min_inner;
			var max_inner = lv.max_inner;
			
			while (min_inner.x > min.x) {
				lv.origin.x -= lv.scale * 2;
				min = levels[level - 1].min;
				max = levels[level - 1].max;
				min_inner = lv.min_inner;
				max_inner = lv.max_inner;
				update = true;
			}
			
			while (min_inner.y > min.y) {
				lv.origin.y -= lv.scale * 2;
				min = levels[level - 1].min;
				max = levels[level - 1].max;
				min_inner = lv.min_inner;
				max_inner = lv.max_inner;
				update = true;
			}
			
			while (max_inner.x < max.x) {
				lv.origin.x += lv.scale * 2;
				min = levels[level - 1].min;
				max = levels[level - 1].max;
				min_inner = lv.min_inner;
				max_inner = lv.max_inner;
				update = true;
			}
			
			while (max_inner.y < max.y) {
				lv.origin.y += lv.scale * 2;
				min = levels[level - 1].min;
				max = levels[level - 1].max;
				min_inner = lv.min_inner;
				max_inner = lv.max_inner;
				update = true;
			}
		} else {
			while (movement.x >= 2) {
				lv.origin.x -= 2;
				movement.x -= 2;
				update = true;
			}
			
			while (movement.x <= -2) {
				lv.origin.x += 2;
				movement.x += 2;
				update = true;
			}
			
			while (movement.y >= 2) {
				lv.origin.y -= 2;
				movement.y -= 2;
				update = true;
			}
			
			while (movement.y <= -2) {
				lv.origin.y += 2;
				movement.y += 2;
				update = true;
			}
			
		}
		
		int w = W * lv.scale;
		int small = 2 * lv.scale;
		float t = W / SIZE_FPW2;
		float small_t = 2f / SIZE_FPW2;
		float right = (SIZE - 1) * lv.scale;
		float top = (SIZE - 1) * lv.scale;
		float tex_edge = t + t + t + small_t;
		
		if (update) {
			update_clip_map(lv);
		}
		
		if (level == 0) {
			draw_center(lv);
		} else {
			var min = levels[level - 1].min;
			var max = levels[level - 1].max;
			var min_inner = lv.min_inner;
			var max_inner = lv.max_inner;
			
			if (min.x == min_inner.x) {
				if (min.y == min_inner.y) {
					draw_mesh(lv, grid_L[L_XMAX_YMAX], lv.origin.x + w, lv.origin.y + w, t, t);
				} else {
					draw_mesh(lv, grid_L[L_XMAX_YMIN], lv.origin.x + w, lv.origin.y + w, t, t);
				}
			} else {
				if (min.y == min_inner.y) {
					draw_mesh(lv, grid_L[L_XMIN_YMAX], lv.origin.x + w, lv.origin.y + w, t, t);
				} else {
					draw_mesh(lv, grid_L[L_XMIN_YMIN], lv.origin.x + w, lv.origin.y + w, t, t);
				}
			}
		}
		
		// Bottom
		draw_mesh(lv, grid_MxM, lv.origin.x, lv.origin.y, 0, 0);
		draw_mesh(lv, grid_MxM, lv.origin.x + w, lv.origin.y, t, 0);
		draw_mesh(lv, grid_Mx3, lv.origin.x + w + w, lv.origin.y, t + t, 0);
		draw_mesh(lv, grid_MxM, lv.origin.x + w + w + small, lv.origin.y, t + t + small_t, 0);
		draw_mesh(lv, grid_MxM, lv.origin.x + w + w + small + w, lv.origin.y, t + t + small_t + t, 0);
		
		// Left
		draw_mesh(lv, grid_MxM, lv.origin.x, lv.origin.y + w, 0, t);
		draw_mesh(lv, grid_3xM, lv.origin.x, lv.origin.y + w + w, 0, t + t);
		draw_mesh(lv, grid_MxM, lv.origin.x, lv.origin.y + w + w + small, 0, t + t + small_t);
		
		// Right
		draw_mesh(lv, grid_MxM, lv.origin.x + right - w, lv.origin.y + w, tex_edge, t);
		draw_mesh(lv, grid_3xM, lv.origin.x + right - w, lv.origin.y + w + w, tex_edge, t + t);
		draw_mesh(lv, grid_MxM, lv.origin.x + right - w, lv.origin.y + w + w + small, tex_edge, t + t + small_t);
		
		// Top
		draw_mesh(lv, grid_MxM, lv.origin.x, lv.origin.y + top - w, 0, tex_edge);
		draw_mesh(lv, grid_MxM, lv.origin.x + w, lv.origin.y + top - w, t, tex_edge);
		draw_mesh(lv, grid_Mx3, lv.origin.x + w + w, lv.origin.y + top - w, t + t, tex_edge);
		draw_mesh(lv, grid_MxM, lv.origin.x + w + w + small, lv.origin.y + top - w, t + t + small_t, tex_edge);
		draw_mesh(lv, grid_MxM, lv.origin.x + w + w + small + w, lv.origin.y + top - w, t + t + small_t + t, tex_edge);
	}
	
	void OnDrawGizmos() {
		if (levels != null) {
			for (int i = 0; i < L && i < levels.Length; ++i) {
				gcm_level lv = levels[i];
				
				//Gizmos.color = Color.white;
				//Gizmos.DrawWireSphere(lv.origin.v3_xz, 0.25f);
				//Bounds b = new Bounds();
				//b.SetMinMax(lv.min_inner.v3_xz, lv.max_inner.v3_xz);
				//Gizmos.color = Color.red;
				//Gizmos.DrawWireCube(b.center, b.size);
				//Gizmos.DrawLine(b.min - new Vector3(0, 32, 0), b.min + new Vector3(0, 32, 0));
				//Gizmos.DrawLine(b.max - new Vector3(0, 32, 0), b.max + new Vector3(0, 32, 0));
				//Gizmos.DrawLine(new Vector3(b.min.x, 0, b.max.z) - new Vector3(0, 32, 0), new Vector3(b.min.x, 0, b.max.z) + new Vector3(0, 32, 0));
				//Gizmos.DrawLine(new Vector3(b.max.x, 0, b.min.z) - new Vector3(0, 32, 0), new Vector3(b.max.x, 0, b.min.z) + new Vector3(0, 32, 0));
			}
		}
	}
	
	void update_clip_map (gcm_level lv) {
		int2 pos = new int2(terrain_offset) + lv.origin;
		float start_x = (float)pos.x / (float)heightmap.width;
		float start_y = (float)pos.y / (float)heightmap.width;
		float texture_width = ((float)SIZE_FPW2 / (float)heightmap.width) * lv.scale;
		float texel_width = 1f / (float)heightmap.width;
		
		level_map_material.SetTexture("_MainTex", heightmap);
		level_map_material.SetVector("_Input", new Vector4(start_x, start_y, texture_width, texel_width));
		
		Graphics.Blit(heightmap, lv.texture, level_map_material);
	}
	
	void read_collision_data (int x, int y) {
		float start_x = (float)x / (float)heightmap.width;
		float start_y = (float)y / (float)heightmap.width;
		float texture_width = COLLIDER_SIZE / (float)heightmap.width;
		float texel_width = 1f / (float)heightmap.width;
		
		level_map_material.SetTexture("_MainTex", heightmap);
		level_map_material.SetVector("_Input", new Vector4(start_x, start_y, texture_width, texel_width));
		
		Graphics.Blit(heightmap, collider_rt, level_map_material);
	}
	
	RenderTexture collider_rt;
	Texture2D collider_tex;
	
	void OnGUI() {
		int preview_size = 128;
		for (int i = 0; i < L; ++i) {
			GUI.DrawTexture(new Rect(i * preview_size,0, preview_size, preview_size), levels[i].texture);
		}
	}
}