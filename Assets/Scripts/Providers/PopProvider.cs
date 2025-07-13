using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCase
{

    [RequireComponent(typeof(GravityProvider))]
    [RequireComponent(typeof(NeighbourUpdateProvider))]
    [RequireComponent(typeof(GridController))]
    public class PopProvider : Provider
    {
        public List<GameObject> toDestroy { get; private set; } = new List<GameObject>();
        private GravityProvider gravityProvider;

        void Start()
        {
            gravityProvider = GetComponent<GravityProvider>();
            grid = GetComponent<GridController>();
        }

        public override List<Action> Run(Action action)
        {
            List<Action> actions = new List<Action>();
            if (toDestroy.Count > 0)
            {
                actions.Add(new Action(grid, gravityProvider));
                foreach(GameObject block in toDestroy)
                {
                    Block b = block.GetComponent<Block>();
                    
                    b.Pop(action);
                    
                    grid.SetBlock(b.x, b.y, null);
                }
                NeighbourUpdateProvider nup = GetComponent<NeighbourUpdateProvider>();
                nup.AddDeletedBlocks(toDestroy);
                toDestroy.Clear();
                actions.Add(new Action(grid, nup));
            }

            if(action.workerCount == 0)
            {
                action.SilentFinish();
            }

            return actions;
        }

        public void AddDestroy(List<GameObject> newDestroys)
        {
            foreach (GameObject obj in newDestroys)
            {
                if (obj != null && !toDestroy.Contains(obj))
                {
                    toDestroy.Add(obj);
                }
            }
        }
    }

}
