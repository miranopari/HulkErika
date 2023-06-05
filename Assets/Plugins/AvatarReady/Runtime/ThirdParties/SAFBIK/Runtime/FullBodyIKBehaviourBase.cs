// Copyright (c) 2016 Nora
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

using UnityEngine;

namespace SA
{
    public abstract class FullBodyIKBehaviourBase : MonoBehaviour
    {
        [System.NonSerialized]
        FullBodyIK _cache_fullBodyIK; // instance cache

        public abstract FullBodyIK fullBodyIK
        {
            get;
        }

        private void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (_cache_fullBodyIK == null)
            {
                _cache_fullBodyIK = fullBodyIK;
            }

            if (_cache_fullBodyIK != null)
            {
                _cache_fullBodyIK.Awake(transform);
            }
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (_cache_fullBodyIK == null)
            {
                _cache_fullBodyIK = fullBodyIK;
            }
            if (_cache_fullBodyIK != null)
            {
                _cache_fullBodyIK.Update();
            }
        }

        private void OnDestroy()
        {
            if (_cache_fullBodyIK == null)
            {
                _cache_fullBodyIK = fullBodyIK;
            }
            if (_cache_fullBodyIK != null)
            {
                _cache_fullBodyIK.Destroy();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_cache_fullBodyIK == null)
            {
                _cache_fullBodyIK = fullBodyIK;
            }
            if (_cache_fullBodyIK != null)
            {
                _cache_fullBodyIK.DrawGizmos();
            }
        }
#endif

        // Excecutable in Inspector.
        public void Prefix()
        {
            if (_cache_fullBodyIK == null)
            {
                _cache_fullBodyIK = fullBodyIK;
            }
            if (_cache_fullBodyIK != null)
            {
                _cache_fullBodyIK.Prefix(transform);
            }
        }
    }
}