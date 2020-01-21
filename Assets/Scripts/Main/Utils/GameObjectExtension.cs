using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameKit.Util.Extension
{
    public static class GameObjectExtension
    {
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren) {
                child.gameObject.layer = layer;
            }
        }

        public static T GetOrCreateComponent<T>(this GameObject gameObject)
            where T : Component
        {
            return (T) GetOrCreateComponent(gameObject, typeof(T));
        }

        public static Component GetOrCreateComponent(this GameObject gameObject, Type concreteType)
        {
            Component result = gameObject.GetComponent(concreteType);
            if (result == null) {
                result = gameObject.AddComponent(concreteType);
            }
            return result;
        }

        public static List<GameObject> GetChildren(this GameObject gameObject)
        {
            List<GameObject> result = new List<GameObject>();
            for (int i = 0; i < gameObject.transform.childCount; i++) {
                Transform t = gameObject.transform.GetChild(i);
                result.Add(t.gameObject);
            }

            return result;
        }

        public static void RemoveChildren(this GameObject gameObject)
        {
            List<GameObject> children = GetChildren(gameObject);

            foreach (GameObject child in children) {
                if (child == null) {
                    continue;
                }
                child.transform.SetParent(null);
                if (Application.isEditor) {
                    Object.DestroyImmediate(child);
                } else {
                    Object.Destroy(child);
                }
            }
        }

        public static GameObject RequireChildRecursive(this GameObject gameObject, string name, bool includeNotActive = false)
        {
            GameObject go = GetChildRecursive(gameObject, name, includeNotActive);
            if (go == null) {
                throw new NullReferenceException("child '" + name + "' not found");
            }
            return go;
        }

        [CanBeNull]
        public static GameObject GetChildRecursive(this GameObject gameObject, string name, bool includeNotActive = false)
        {
            Transform[] childComponents = gameObject.gameObject.GetComponentsInChildren<Transform>(includeNotActive);
            Transform t = childComponents.FirstOrDefault(childComponent => childComponent.gameObject.name == name);
            return t == null ? null : t.gameObject;
        }
    }
}