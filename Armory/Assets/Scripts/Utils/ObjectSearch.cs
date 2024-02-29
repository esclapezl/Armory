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
        public static Transform FindChild(Transform parent, string pattern)
        {
            Regex regex = new Regex(pattern);
            foreach (Transform child in parent)
            {
                if (regex.Match(child.name).Success)
                {
                    return child;
                }
                
                if(child.childCount > 0)
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
                if(child.childCount > 0)
                {
                    children.AddRange(FindChildren(child, pattern));
                }
            }
            return children;
        }
    }
}
