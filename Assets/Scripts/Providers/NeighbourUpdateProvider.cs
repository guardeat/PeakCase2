using System.Collections.Generic;
using UnityEngine;

namespace PCase
{
    [RequireComponent(typeof(GameGrid))]
    public class NeighbourUpdateProvider : Provider
    {
        public List<GameObject> deletedBlocks { get; private set; } = new List<GameObject>();

        void Start()
        {
            grid = GetComponent<GameGrid>();
        }

        public override List<Action> Run(Action action)
        {
            List<Action> actions = new List<Action>();
            HashSet<GameObject> deletedSet = new HashSet<GameObject>(deletedBlocks);
            HashSet<GameObject> updatedSet = new HashSet<GameObject>();

            foreach (GameObject deleted in deletedBlocks)
            {
                if (deleted == null) continue;

                List<GameObject> neighbours = GetDirectNeighbours(deleted);

                foreach (GameObject neighbour in neighbours)
                {
                    if (neighbour == null || deletedSet.Contains(neighbour) || updatedSet.Contains(neighbour))
                        continue;

                    updatedSet.Add(neighbour);

                    Block block = neighbour.GetComponent<Block>();
                    if (block != null)
                    {
                        actions.AddRange(block.OnUpdate(action));
                    }
                }
            }

            deletedBlocks.Clear();

            if (action.workerCount == 0)
            {
                action.SilentFinish();
            }

            return actions;
        }

        public void AddDeletedBlocks(List<GameObject> blocks)
        {
            foreach (GameObject obj in blocks)
            {
                if (obj != null && !deletedBlocks.Contains(obj))
                {
                    deletedBlocks.Add(obj);
                }
            }
        }

        private List<GameObject> GetDirectNeighbours(GameObject blockObj)
        {
            List<GameObject> result = new List<GameObject>();
            Block block = blockObj.GetComponent<Block>();
            if (block == null) return result;

            int[,] offsets = new int[,]
            {
                { 0, 1 },  // up
                { 0, -1 }, // down
                { -1, 0 }, // left
                { 1, 0 }   // right
            };

            for (int i = 0; i < offsets.GetLength(0); i++)
            {
                int nx = block.x + offsets[i, 0];
                int ny = block.y + offsets[i, 1];

                GameObject neighbour = grid.GetBlock(nx, ny);
                if (neighbour != null)
                {
                    result.Add(neighbour);
                }
            }

            return result;
        }
    }
}
