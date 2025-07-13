using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCase
{

    [RequireComponent(typeof(GridController))]
    public class SelectProvider : Provider
    {
        private Vector2Int? lastSelection = null;

        void Start()
        {
            grid = GetComponent<GridController>();
        }

        public override List<Action> Run(Action action)
        {
            List<Action> actions = new List<Action>();
            if (lastSelection != null) {
                Vector2Int pos = lastSelection.Value;
                var b = grid.GetBlock(pos.x, pos.y);
                if (b)
                {
                    actions = b.GetComponent<Block>().OnSelect(action);
                }
                lastSelection = null;
            }

            if(action.workerCount == 0)
            {
                action.SilentFinish();
            }

            if(actions.Count > 0)
            {
                action.grid.DecreaseMove();
            }

            return actions;
        }

        public void Select(Vector2Int? selection)
        {
            lastSelection = selection;
        }
    }

}
