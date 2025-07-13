using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCase
{
    [RequireComponent(typeof(GridController))]
    public class Provider : MonoBehaviour
    {
        protected GridController grid;

        void Start()
        {
            grid = GetComponent<GridController>();
        }

        public virtual List<Action> Run(Action action)
        {
            action.SilentFinish();
            return new();
        }
    }
}
