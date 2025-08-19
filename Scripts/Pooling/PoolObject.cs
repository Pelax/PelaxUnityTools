using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pelax.Pooling
{
    public class PoolObject : MonoBehaviour
    {
        #region MEMBER FIELDS

        public bool offScreenDestroy;
        public Vector2 offScreenOffsets = new(1.5f, 1.5f);

        public delegate void OnPoolObjectSpawnedDelegate(PoolObject poolObject);
        public event OnPoolObjectSpawnedDelegate OnPoolObjectSpawnedEvent;

        public delegate void OnPoolObjectDestroyedDelegate(PoolObject poolObject, bool offScreen);
        public event OnPoolObjectDestroyedDelegate OnPoolObjectDestroyedEvent;

        [Serializable]
        public class SingleAnimation
        {
            public bool enabled;
            public string stateName = "Default";
        }

        #endregion



        #region INIT STUFF

        protected virtual void Awake() { }

        protected virtual void Start() { }

        #endregion



        #region DESTROY STUFF



        private void TriggerOnDestroyEvent(bool offScreen)
        {
            OnPoolObjectDestroyedEvent?.Invoke(this, offScreen);
        }

        public virtual void Reset()
        {
            gameObject.SetActive(true);
            OnPoolObjectSpawnedEvent?.Invoke(this);
        }

        public virtual void Disable(float DestroyTime)
        {
            if (!gameObject.activeSelf) //if already deactivated dont do anything
                return;

            TriggerOnDestroyEvent(false);

            if (DestroyTime > 0)
                Invoke(nameof(DelayedDisable), DestroyTime);
            else
                gameObject.SetActive(false);
        }

        public void DelayedDisable()
        {
            gameObject.SetActive(false);
        }

        #endregion
    }
}
