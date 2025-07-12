using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCase
{
    [RequireComponent(typeof(GameGrid))]
    public class Provider : MonoBehaviour
    {
        protected GameGrid grid;

        void Start()
        {
            grid = GetComponent<GameGrid>();
        }

        public virtual List<Action> Run(Action action)
        {
            action.SilentFinish();
            return new();
        }
    }
}
