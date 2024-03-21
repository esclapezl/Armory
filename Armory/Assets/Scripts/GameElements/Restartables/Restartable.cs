using System;
using UnityEngine;

namespace GameElements.Restartables
{
    public abstract class Restartable : MonoBehaviour
    {
        public virtual void Restart()
        {
            throw new NotImplementedException();
        }

        public virtual void Exit()
        {
            throw new NotImplementedException();
        }
    }
}