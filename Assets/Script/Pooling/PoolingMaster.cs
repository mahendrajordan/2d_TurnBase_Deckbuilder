using System;
using System.Collections.Generic;
using UnityEngine;


public class PoolingMaster : MonoBehaviour
{
    public static PoolingMaster ins;

    [SerializeField] Transform poolParent;

    Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        if(ins != null && ins != this)
        {
            Destroy(gameObject);
            return;
        }
        ins = this;
    }

    //membuat object pool
    public void CreateObjectPool(GameObject poolObjPrefab, int amount = 5)
    {
        if(poolObjPrefab==null) return;
        if (pools.ContainsKey(poolObjPrefab)) return;

        Queue<GameObject> q = new Queue<GameObject>();

        for(int i = 0; i < amount; i++)
        {
            GameObject _obj = CreateNewObject(poolObjPrefab);
            q.Enqueue(_obj);
        }

        pools.Add(poolObjPrefab, q);
    }

    private GameObject CreateNewObject(GameObject poolObjPrefab)
    {
        GameObject obj = Instantiate(poolObjPrefab, this.transform);

        //create PoolObjectInfo untuk menyimpan data prefab nya
        PoolObjectInfo po = obj.GetComponent<PoolObjectInfo>();
        if (po == null) po = obj.AddComponent<PoolObjectInfo>();
        po.originalPrefab = poolObjPrefab;

        Transform parent = poolParent == null ? this.transform : poolParent;
        po.transform.SetParent(parent);
        obj.SetActive(false);

        return obj;
    }

    //mendapat kan object pool yg tersedia
    public GameObject GetPoolObject(GameObject poolObjPrefab)
    {
        //jika pools not found (Dictionary pools tidak di temukan) auto create pool 
        if (!pools.TryGetValue(poolObjPrefab, out Queue<GameObject> queue))
        {
            CreateObjectPool(poolObjPrefab, 1);
            queue = pools[poolObjPrefab];
        }

        //expans pool
        if(queue.Count <= 0)
        {
            GameObject _obj = CreateNewObject(poolObjPrefab);
            queue.Enqueue(_obj);
        }

        GameObject obj = queue.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    // mengembalikan object pool yg sudah di pakai
    public void ReturnPoolObject(GameObject poolObj)
    {
        PoolObjectInfo po = poolObj.GetComponent<PoolObjectInfo>();
        if(po == null) 
        {
            Destroy(poolObj);
            return;
        }

        //pools not found (Dictionary pools tidak di temukan)
        if (!pools.TryGetValue(po.originalPrefab, out Queue<GameObject> queue))
        {
            Destroy(poolObj);
            return;
        }
        
        queue.Enqueue(poolObj);
        poolObj.SetActive(false);
    } 

    // untuk remove/destroy object pool yg sudah di panggil
    public void RemovePoolObject(GameObject poolObjPrefab, int amount = 5)
    {

        //pools not found (Dictionary pools tidak di temukan)
        if (!pools.TryGetValue(poolObjPrefab, out Queue<GameObject> queue)) return;

        int removeCount = Mathf.Min(amount, queue.Count);
        for (int i = 0; i< removeCount; i++)
        {
            GameObject obj = queue.Dequeue();
            Destroy(obj);
        }
    }
}
