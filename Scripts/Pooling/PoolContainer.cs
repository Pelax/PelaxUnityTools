using System.Collections;
using System.Collections.Generic;
using Pelax.Utils;
using UnityEngine;

namespace Pelax.Pooling
{
    public class PoolContainer : MonoBehaviour
    {
        #region MEMBER FIELDS

        public PoolObject prefab;
        public int maxInstances;
        public List<PoolObject> poolInstances = new List<PoolObject>();

        #endregion


        #region POOL STUFF

        private PoolObject CreatePooledObject(Vector3 pos, bool useLocalPosition)
        {
            PoolObject clone =
                (useLocalPosition)
                    ? GameObject.Instantiate(prefab)
                    : (PoolObject)GameObject.Instantiate(prefab, pos, Quaternion.identity);

            clone.transform.SetParent(transform, false);
            clone.name = prefab.name;
            if (useLocalPosition)
                clone.transform.localPosition = pos;

            poolInstances.Add(clone);
            return clone;
        }

        public PoolObject GetNextObject(Vector3 pos, bool useLocalPosition)
        {
            PoolObject instance = null;

            for (int i = poolInstances.Count - 1; i >= 0; i--)
            {
                if (null == poolInstances[i])
                {
                    Logit.Error($"PoolContainer {name} instance is null, this should be prevented");
                    poolInstances.RemoveAt(i);
                    continue;
                }
                if (!poolInstances[i].gameObject.activeSelf)
                {
                    instance = poolInstances[i];
                    instance.transform.position = pos;
                    break;
                }
            }

            if (instance == null && (poolInstances.Count < maxInstances || maxInstances == -1))
                instance = CreatePooledObject(pos, useLocalPosition);
            instance.Reset();
            return instance;
        }

        #endregion
    }
}
