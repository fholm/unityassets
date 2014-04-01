using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiratesOnlineHexagonWorld : MonoBehaviour
{
    #region Static

    static PiratesOnlineHexagonWorld instance = null;
    static Queue recycledGrids = new Queue(64);
    static ArrayList activeGrids = new ArrayList(64);
    static Dictionary<int, PiratesOnlineHexagonGrid> keyMap = new Dictionary<int, PiratesOnlineHexagonGrid>(64);

    public static Material GetAtlasMaterial(byte atlas)
    {
        return instance.materials[atlas];
    }

    public static bool GetGrid(int x, int z, out PiratesOnlineHexagonGrid grid)
    {
        return keyMap.TryGetValue((x << 16) | z, out grid);
    }

    public static void AddGrid(PiratesOnlineHexagonGrid grid)
    {
        keyMap[(grid.GridX << 16) | grid.GridZ] = grid;
        activeGrids.Add(grid);
    }

    public static bool HasGrid(int x, int z)
    {
        return keyMap.ContainsKey((x << 16) | z);
    }

    static PiratesOnlineHexagonGrid CreateGrid(int x, int z, Vector3 position)
    {
        if (x < 0 || z < 0)
        {
            return null;
        }

        if (x >= PiratesOnlineConstants.GridCount || z >= PiratesOnlineConstants.GridCount)
        {
            return null;
        }

        if (HasGrid(x, z))
        {
            return null;
        }

        PiratesOnlineHexagonGrid grid = null;

        if (recycledGrids.Count > 0)
        {
            grid = (PiratesOnlineHexagonGrid)recycledGrids.Dequeue();
            grid.gameObject.SetActiveRecursively(true);
        }
        else
        {
            // Create new grid object
            GameObject go = (GameObject)GameObject.Instantiate(instance.prefab, Vector3.zero, Quaternion.identity);

            // Grab new component
            grid = (PiratesOnlineHexagonGrid)go.GetComponent(typeof(PiratesOnlineHexagonGrid));

            // Init
            grid.InitMesh();
        }

        grid.GridX = x;
        grid.GridZ = z;
        grid.transform.position = position;

        AddGrid(grid);

        return grid;
    }

    static PiratesOnlineHexagonGrid CreateGrid(int x, int z, Vector3 position, PiratesOnlineNode node)
    {
        PiratesOnlineHexagonGrid grid = CreateGrid(x, z, position);

        if (grid != null)
        {
            grid.UpdateMesh(node);
        }

        return grid;
    }

    static void RecycleGrid(PiratesOnlineHexagonGrid grid)
    {
        // Remove from world
        keyMap.Remove((grid.GridX << 16) | grid.GridZ);

        // Remove from active grids
        activeGrids.Remove(grid);

        // Deactivate
        grid.gameObject.SetActiveRecursively(false);

        // Recycle
        recycledGrids.Enqueue(grid);

        //GameObject.Destroy(grid.gameObject);
    }

    #endregion

    [SerializeField]
    GameObject prefab;

    [SerializeField]
    float scrollSpeed = 10f;

    [SerializeField]
    int startX = 1023;

    [SerializeField]
    int startZ = 1023;

    [SerializeField]
    Material[] materials;
    
    //int moveFramesCounter = 0;
    //float touchStartTime;
    Vector2 touchStartPosition;
    PiratesOnlineNode testNode = null;
    PiratesOnlineHexagonGrid closestGrid = null;

    void Start()
    {
        instance = this;

        // Init test node
        InitTestNode();
        
        // Init grid
        SpawnAround(CreateGrid(startX, startZ, Vector3.zero, testNode));
    }

    void Update()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        TouchMove();
#else
        KeyboardMove();
#endif
    }

    void KeyboardMove()
    {
        // Default to no movement
        Vector3 m = Vector3.zero;

        // Vertical
        if (Input.GetKey(KeyCode.UpArrow))
        {
            m += Vector3.back;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            m += Vector3.forward;
        }

        // Horizontal
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            m += Vector3.right;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            m += Vector3.left;
        }

        // Do move!
        Move(m);
    }

    void TouchMove()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    //touchStartTime = Time.time;
                    touchStartPosition = touch.position;
                    break;

                case TouchPhase.Ended:
                    //touchStartTime = 0;
                    break;

                case TouchPhase.Moved:
                    // Total delta
                    Vector2 delta = -(touch.position - touchStartPosition);

                    // Convert Y to Z
                    Vector3 movement = new Vector3(delta.x, 0, delta.y);

                    // Apply movement
                    Move(movement.normalized * (movement.magnitude / 100));

                    break;
            }
        }
    }

    void Move(Vector3 m)
    {
        // If theres no movement, ignore
        if (m == Vector3.zero)
        {
            return;
        }

        // Never move further then 1.0 units per frame (really fast)
        m = Quaternion.Euler(0, 50, 0) * m;
        m = Vector3.ClampMagnitude(m * Time.deltaTime * scrollSpeed, 1f);

        // Squared distance, just start at max possible value
        float sqrDistance = float.MaxValue;

        //PiratesOnlineHexagonGrid previousGrid = closestGrid;

        for (int i = 0; i < activeGrids.Count; ++i)
        {
            // Grab grid
            PiratesOnlineHexagonGrid grid = (PiratesOnlineHexagonGrid)activeGrids[i];
            
            // Move!
            grid.transform.position += m;

            // Check if this grid is the closest one
            float sqrMagnitude = grid.transform.position.sqrMagnitude;

            // Check if this is the closest one so far
            if (sqrMagnitude < sqrDistance)
            {
                closestGrid = grid;
                sqrDistance = sqrMagnitude;
            }
        }

        // Spawn around closest grid
        SpawnAround(closestGrid);
    }

    void SpawnAround(PiratesOnlineHexagonGrid grid)
    {
        // Only update if we got a new closest grid
        if (grid != null)
        {
            Vector3 pos = grid.transform.position;

            for (int x = -PiratesOnlineConstants.SpawnDistance; x <= PiratesOnlineConstants.SpawnDistance; ++x)
            {
                for (int z = -PiratesOnlineConstants.SpawnDistance; z <= PiratesOnlineConstants.SpawnDistance; ++z)
                {
                    Vector3 p = pos;

                    // 
                    p.x += (x * PiratesOnlineConstants.GridWidth);
                    p.z += (z * PiratesOnlineConstants.GridHeight);

                    // Make sure we have this grid
                    CreateGrid(grid.GridX + x, grid.GridZ + z, p, testNode);
                }
            }

            // Find out-dated grids
            for (int i = 0; i < activeGrids.Count; ++i)
            {
                // Grab grid
                PiratesOnlineHexagonGrid neighbor = (PiratesOnlineHexagonGrid)activeGrids[i];

                // Calculate distances
                int xDist = grid.GridX - neighbor.GridX;
                int zDist = grid.GridZ - neighbor.GridZ;

                // Check x distance
                if (xDist > PiratesOnlineConstants.SpawnDistance || xDist < -PiratesOnlineConstants.SpawnDistance)
                {
                    RecycleGrid(neighbor);
                    --i;
                }

                // Check z distance
                else if (zDist > PiratesOnlineConstants.SpawnDistance || zDist < -PiratesOnlineConstants.SpawnDistance)
                {
                    RecycleGrid(neighbor);
                    --i;
                }
            }
        }
    }

    void InitTestNode()
    {
        // Setup node
        testNode = new PiratesOnlineNode((byte)materials.Length);

        for (int i = 0; i < materials.Length; ++i)
        {
            testNode.Atlases[i].Tiles = (byte)(PiratesOnlineConstants.GridTiles / materials.Length);
            testNode.Atlases[i].Atlas = (byte)i;
        }

        for (int i = 0; i < PiratesOnlineConstants.GridTiles; ++i)
        {
            testNode.Tiles[i].SubAtlas = (byte)(i % materials.Length);
            testNode.Tiles[i].Icon = (byte)i;
        }
    }
}
