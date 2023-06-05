using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    [Serializable]
    public class CalibrationProfile
    {
        public string Name;

        [SerializeField]
        private List<CalibrationData> Data = new List<CalibrationData>();

        public bool ContainsData(string key)
        {
            return Data.Exists(d => d.Key == key);
        }

        public float Get(string key)
        {
            if (ContainsData(key))
                return Data.Find(d => d.Key == key).Value;
            else
                throw new KeyNotFoundException(key);
        }

        public void Set(string key, float value)
        {
            if (ContainsData(key))
                Data.Find(d => d.Key == key).Value = value;
            else
                Data.Add(new CalibrationData() { Key = key, Value = value });
        }

        public List<CalibrationData> AllData()
        {
            return Data;
        }

        [Serializable]
        public class CalibrationData
        {
            public string Key;
            public float Value;
        }
    }


}
