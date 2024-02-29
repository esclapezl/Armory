using UnityEngine;

namespace DefaultNamespace
{
    public class Utils
    {
        public static string AngleToDirection(float angle, float angleTolerance)
        {
            if (angle < 0)
            {
                angle += 360;
            }
            
            if(angle < 0 + angleTolerance || angle > 360 - angleTolerance)
            {
                return "right";
            }
            
            if(angle < 90 + angleTolerance && angle > 90 - angleTolerance)
            {
                return "up";
            }
            
            if(angle < 180 + angleTolerance && angle > 180 - angleTolerance)
            {
                return "left";
            }
            
            if(angle < 270 + angleTolerance && angle > 270 - angleTolerance)
            {
                return "down";
            }
            
            return "none";
        }
        
        public static Transform FindChild(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                {
                    return child;
                }
                
                if(child.childCount > 0)
                {
                    Transform result = FindChild(child, name);
                    if (result != null)
                    {
                        return result;
                    }
                }
                
            }
            return null;
        }
    }
}