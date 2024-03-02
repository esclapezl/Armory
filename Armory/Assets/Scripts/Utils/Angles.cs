using UnityEngine;

namespace Utils
{
    public class Angles
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
        
        public static bool AngleIsInAGivenRange(float angle, float range, float direction)
        {
            if (angle < 0)
            {
                angle += 360;
            }

            float start = direction - range/2;
            float end = direction + range/2;
            if(end > 360 || start < 0)
            {
                if(end > 360)
                {
                    end -= 360;
                }
                else
                {
                    start += 360;
                }
                return (angle > start || angle < end);
            }
            return (angle > start && angle < end);
        }
    }
}