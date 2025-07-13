using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCase
{

    [RequireComponent(typeof(GridController))]
    public class MoveUpdateProvider : Provider
    {
        public List<GameObject> movedBlocks { get; private set; } = new List<GameObject>();

        void Start()
        {
            grid = GetComponent<GridController>();
        }

        public override List<Action> Run(Action action)
        {
            List<Action> actions = new List<Action>();

            foreach (GameObject blockObj in movedBlocks)
            {
                if (blockObj == null) continue;

                Block block = blockObj.GetComponent<Block>();
                if (block != null)
                {
                    actions.AddRange(block.OnMove(action));
                }
            }

            movedBlocks.Clear();

            if (action.workerCount == 0)
            {
                action.SilentFinish();
            }

            return actions;
        }

        public void AddMovedBlocks(List<GameObject> blocks)
        {
            foreach (GameObject obj in blocks)
            {
                if (obj != null && !movedBlocks.Contains(obj))
                {
                    movedBlocks.Add(obj);
                }
            }
        }
    }


}
