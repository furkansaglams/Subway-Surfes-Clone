using UnityEngine;

public class TileSpawnManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    public float zSpawn;
    public float tileLength;

    public static TileSpawnManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int RandomPrefabNumber()
    {
        int randomPrefab = Random.Range(0, tilePrefabs.Length);
        return randomPrefab;
    }

    public GameObject SpawnTile()
    {

        GameObject obj = ObjectPool.instance.GetPooObject(0);
        obj.transform.position = new Vector3(0, 0, tileLength) + new Vector3(-25, 0, 0) + transform.forward * zSpawn;
        zSpawn += tileLength;
        return obj;

    }
}