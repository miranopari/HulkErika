using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Inria.Avatar.AvatarReady
{
    public static class ReflectionUtils
    {
        private static Type[] cachedTypes;
        private static Dictionary<Type, Type[]> cachedResults;

        public static Type[] GetSubclassesOf(Type parent, bool IsAbstract = false)
        {
            if (cachedTypes == null)
                cachedTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).ToArray();

            if (cachedResults == null)
                cachedResults = new Dictionary<Type, Type[]>();

            if (!cachedResults.ContainsKey(parent))
                cachedResults[parent] = cachedTypes.Where(t => t != parent && parent.IsAssignableFrom(t)).ToArray();

            return cachedResults[parent].Where(t => t.IsAbstract == IsAbstract).ToArray();
        }

        public static List<T> GetInstancesOf<T>(T parent)
        {
            return GetSubclassesOf(typeof(T)).Where(t => !t.IsAbstract).Select(t => Activator.CreateInstance(t)).Cast<T>().ToList();
        }

        public static string[] GetAvatarReadyNames(Type[] types)
        {
            return types.Select(t => GetAvatarReadyName(t)).ToArray();
        }

        public static string GetAvatarReadyName(Type type)
        {
            if (Attribute.IsDefined(type, typeof(AvatarReadyNameAttribute)))
                return (Attribute.GetCustomAttribute(type, typeof(AvatarReadyNameAttribute)) as AvatarReadyNameAttribute).displayName;
            else
            {
#if UNITY_EDITOR
                return ObjectNames.NicifyVariableName(type.Name);
#else
                return type.Name;
#endif
            }
        }
    }
}