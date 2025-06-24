using System;
using System.Collections;
using System.Collections.Generic;
using Pelax.Utils;
using UnityEngine;

namespace Pelax.Pooling
{
    public class PoolManager : Singleton<PoolManager>
    {
        #region MEMBER FIELDS

        public PoolContainerData[] poolContainers;

        [Serializable]
        public struct PoolContainerData
        {
            public Transform transform;
            public int sortingOrder;
        }

        public static Dictionary<string, PoolContainer> pools =
            new Dictionary<string, PoolContainer>();

        public const string PoolContainerName = "Pools";

        #endregion



        #region CREATE AND DESTROY STUFF

        public static GameObject Instantiate(
            GameObject prefab,
            Vector3 pos,
            Transform parentContainer = null,
            string objectReference = "",
            int maxInstances = -1,
            bool useLocalPosition = false
        )
        {
            GameObject objInstance = null;

            if (parentContainer == null)
                parentContainer = Instance.GetMainPoolContainer().transform;

            objectReference =
                parentContainer.name
                + "_"
                + ((objectReference != "") ? objectReference : prefab.name);

            var recycleGameObject = prefab.GetComponent<PoolObject>();
            if (recycleGameObject != null)
            {
                PoolContainer pool = GetPool(
                    recycleGameObject,
                    parentContainer,
                    objectReference,
                    maxInstances
                );
                PoolObject poolObject = pool.GetNextObject(pos, useLocalPosition);
                if (poolObject)
                    objInstance = poolObject.gameObject;
            }
            else
            {
                objInstance = UnityEngine.Object.Instantiate(prefab);
                objInstance.transform.position = pos;
            }

            return objInstance;
        }

        public static void Destroy(GameObject gameObject, float DestroyTime = 0)
        {
            PoolObject recycleGameObject = gameObject.GetComponent<PoolObject>();

            if (recycleGameObject != null)
            {
                gameObject.transform.SetParent(Instance.transform, false);
                recycleGameObject.Disable(DestroyTime);
            }
            else
                UnityEngine.Object.Destroy(gameObject, DestroyTime);
        }

        #endregion



        #region POOL STUFF

        public static void Clear()
        {
            foreach (KeyValuePair<string, PoolContainer> pool in pools)
                Destroy(pool.Value.gameObject);

            pools.Clear();
        }

        private static PoolContainer GetPool(
            PoolObject spawnObject,
            Transform parentContainer,
            string reference,
            int maxInstances
        )
        {
            PoolContainer pool;

            if (pools.ContainsKey(reference))
            {
                pool = pools[reference];
            }
            else
            {
                GameObject poolContainer = new GameObject(reference + PoolContainerName);

                if (parentContainer != null)
                {
                    poolContainer.transform.parent = parentContainer;
                    poolContainer.transform.localPosition = Vector2.zero;
                    poolContainer.transform.localScale = parentContainer.localScale;
                }

                pool = poolContainer.AddComponent<PoolContainer>();
                pool.prefab = spawnObject;
                pool.maxInstances = maxInstances;
                pools.Add(reference, pool);
            }

            return pool;
        }

        public PoolContainerData GetMainPoolContainer()
        {
            if (poolContainers.Length == 0)
            {
                poolContainers = new PoolContainerData[1];
                poolContainers[0] = new PoolContainerData();
                poolContainers[0].transform = new GameObject("MainPoolContainer").transform;
                poolContainers[0].transform.SetParent(transform);
            }

            return poolContainers[0];
        }

        #endregion
    }
}
