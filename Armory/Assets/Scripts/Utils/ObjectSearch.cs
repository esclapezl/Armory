using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class ObjectSearch : MonoBehaviour
    {
        public static Transform FindRoot(string pattern)
        {
            Regex regex = new Regex(pattern);
            GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (GameObject obj in rootObjects)
            {
                if (regex.Match(obj.name).Success)
                {
                    return obj.transform;
                }
            }

            return null;
        }

        public static List<Transform> FindAllRoots(string pattern)
        {
            Regex regex = new Regex(pattern);
            GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            List<Transform> matchingTransforms = new List<Transform>();

            foreach (GameObject obj in rootObjects)
            {
                if (regex.Match(obj.name).Success)
                {
                    matchingTransforms.Add(obj.transform);
                }
            }

            return matchingTransforms;
        }

        public static Transform FindChild(Transform parent, string pattern)
        {
            Regex regex = new Regex(pattern);
            foreach (Transform child in parent)
            {
                if (regex.Match(child.name).Success)
                {
                    return child;
                }

                if (child.childCount > 0)
                {
                    Transform result = FindChild(child, pattern);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }
        
        public static T FindChildWithScript<T>(Transform parent) where T : Component
        {
            T[] components = parent.GetComponentsInChildren<T>();

            if (components.Length > 0)
            {
                return components[0];
            }

            return null;
        }

        public static List<Transform> FindChildren(Transform parent, string pattern)
        {
            List<Transform> children = new List<Transform>();
            Regex regex = new Regex(pattern);
            foreach (Transform child in parent)
            {
                if (regex.Match(child.name).Success)
                {
                    children.Add(child);
                }

                if (child.childCount > 0)
                {
                    children.AddRange(FindChildren(child, pattern));
                }
            }

            return children;
        }

        public static List<T> FindChildrenWithScript<T>(Transform parent) where T : Component
        {
            List<T> childrenWithScript = new List<T>();
            T[] components = parent.GetComponentsInChildren<T>();

            foreach (T component in components)
            {
                childrenWithScript.Add(component);
            }

            return childrenWithScript;
        }

        public static Transform FindParent(Transform child, string pattern)
        {
            Regex regex = new Regex(pattern);
            Transform parent = child.parent;

            while (parent != null)
            {
                if (regex.Match(parent.name).Success)
                {
                    return parent;
                }

                parent = parent.parent;
            }

            return null;
        }

        public static List<Transform> FindParents(Transform child, string pattern)
        {
            List<Transform> parents = new List<Transform>();
            Regex regex = new Regex(pattern);
            Transform parent = child.parent;

            while (parent != null)
            {
                if (regex.Match(parent.name).Success)
                {
                    parents.Add(parent);
                }

                parent = parent.parent;
            }

            return parents;
        }

        public static T FindParentWithScript<T>(Transform child) where T : Component
        {
            Transform parent = child.parent;

            while (parent != null)
            {
                T component = parent.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }

                parent = parent.parent;
            }

            return null;
        }
    }
}