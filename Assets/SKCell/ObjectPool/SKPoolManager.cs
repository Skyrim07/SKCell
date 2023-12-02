using System;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
	[AddComponentMenu("SKCell/Misc/SKPoolManager")]
	public class SKPoolManager : SKMonoSingleton<SKPoolManager>
	{
		public bool logStatus;
		public Transform root;

		private Dictionary<GameObject, SKObjectPool<GameObject>> prefabLookup;
		private Dictionary<GameObject, SKObjectPool<GameObject>> instanceLookup;

		private bool dirty = false;

		protected override void Awake()
		{
            base.Awake();
			prefabLookup = new Dictionary<GameObject, SKObjectPool<GameObject>>();
			instanceLookup = new Dictionary<GameObject, SKObjectPool<GameObject>>();
		}

		void Update()
		{
			if (logStatus && dirty)
			{
				PrintStatus();
				dirty = false;
			}
		}
		public void warmPool(GameObject prefab, int size, Transform parent = null)
		{
			if (prefabLookup.ContainsKey(prefab))
			{
				throw new Exception("Pool for prefab " + prefab.name + " has already been created");
			}

			var pool = new SKObjectPool<GameObject>(() => { return InstantiatePrefab(prefab, parent); }, size);
			prefabLookup[prefab] = pool;

			dirty = true;
		}

		public GameObject spawnObject(GameObject prefab)
		{
			return spawnObject(prefab, Vector3.zero, Quaternion.identity);
		}

		public GameObject spawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			if(!prefab)
				return null;

			if (!prefabLookup.ContainsKey(prefab))
			{
				WarmPool(prefab, 1);
			}

			var pool = prefabLookup[prefab];

			var clone = pool.GetItem();
			if (clone == null)
				return null;

			clone.transform.position = position;
			clone.transform.rotation = rotation;
			clone.SetActive(true);

			instanceLookup.Add(clone, pool);
			dirty = true;
			return clone;
		}

		public void releaseObject(GameObject clone)
		{
			if (!clone)
				return;

			clone.SetActive(false);

			if (instanceLookup.ContainsKey(clone))
			{
				instanceLookup[clone].ReleaseItem(clone);
				instanceLookup.Remove(clone);
				dirty = true;
			}
			else
			{
                SKUtils.EditorLogWarning("No pool contains the object: " + clone.name);
			}
		}


		private GameObject InstantiatePrefab(GameObject prefab, Transform parent = null)
		{
			var go =GameObject.Instantiate(prefab, parent) as GameObject;
			if (root != null) go.transform.SetParent(root,true);
			return go;
		}

		public void PrintStatus()
		{
			foreach (KeyValuePair<GameObject, SKObjectPool<GameObject>> keyVal in prefabLookup)
			{
				SKUtils.EditorLogWarning(string.Format("Object Pool for Prefab: {0} In Use: {1} Total {2}", keyVal.Key.name,
					keyVal.Value.CountUsedItems, keyVal.Value.Count));
			}
		}

		#region Static API
        /// <summary>
        /// Pre-construct a pool
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="size"></param>
        /// <param name="parent"></param>
		public static void WarmPool(GameObject prefab, int size, Transform parent = null)
		{
			instance.warmPool(prefab, size, parent);
		}
        /// <summary>
        /// Instantiate a prefab from the pool
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
		public static GameObject SpawnObject(GameObject prefab)
		{
			return instance.spawnObject(prefab);
		}

		public static GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			return instance.spawnObject(prefab, position, rotation);
		}
        /// <summary>
        /// Release a instantiated clone from the pool
        /// </summary>
        /// <param name="clone"></param>
		public static void ReleaseObject(GameObject clone)
		{
			instance.releaseObject(clone);
		}

		#endregion
	}

}
