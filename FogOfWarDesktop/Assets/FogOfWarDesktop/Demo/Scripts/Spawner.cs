using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject Prefab;

    void Start()
    {
        for (int i = 0; i < 256; ++i)
        {
            Vector3 p = new Vector3(Random.Range(-512f, 512f), 1f, Random.Range(-512f, 512f));
            GameObject.Instantiate(Prefab, p, Quaternion.identity);
        }
    }
}