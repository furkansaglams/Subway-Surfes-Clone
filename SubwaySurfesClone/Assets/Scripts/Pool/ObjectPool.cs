using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectPool : MonoBehaviour
{
   [Serializable]
   public struct Pool
   {
      public Queue<GameObject> pooledObjects;
      public GameObject objectrefab;
      public int poolSize;
   }

   public static ObjectPool instance;
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
   [SerializeField] public Pool[] pools = null;

   private void Start()
   {
      for (int i = 0; i < pools.Length; i++)
      {
         pools[i].pooledObjects = new Queue<GameObject>();
         for (int j = 0; j < pools[i].poolSize; j++)
         {

            GameObject obj = RandomTile();
            obj.SetActive(false);
            pools[i].pooledObjects.Enqueue(obj);
         }
      }
   }

   public GameObject GetPooObject(int objectType)
   {
      if (objectType >= pools.Length)
      {
         return null;
      }
      if (pools[objectType].pooledObjects.Count == 0)
      {
         AddSizePool(5f, objectType);
      }
      GameObject obj = pools[objectType].pooledObjects.Dequeue();
      obj.SetActive(true);
      return obj;

   }

   public void SetPoolObject(GameObject pooledObject, int objectType)
   {
      if (objectType >= pools.Length)
      {
         return;
      }
      else
      {
         pools[objectType].pooledObjects.Enqueue(pooledObject);
         pooledObject.SetActive(false);
      }

   }

   public void AddSizePool(float amount, int objectType)
   {
      for (int i = 0; i < amount; i++)
      {
         GameObject obj = RandomTile();
         obj.SetActive(false);
         pools[objectType].pooledObjects.Enqueue(obj);
      }
   }

   public GameObject RandomTile()
   {
      int tileIndex = TileSpawnManager.instance.RandomPrefabNumber();
      GameObject obj = Instantiate(TileSpawnManager.instance.tilePrefabs[tileIndex]);
      return obj;
   }
}
