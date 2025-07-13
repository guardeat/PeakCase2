using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCase
{
    class RocketArrive
    {
        public GameObject gameObject;
        public int distance;
    }

    [RequireComponent(typeof(GravityProvider))]
    [RequireComponent(typeof(NeighbourUpdateProvider))]
    [RequireComponent(typeof(GridController))]
    public class FireRocketProvider : Provider
    {
        public GameObject rocket { get; private set; } = null;

        private GravityProvider gravityProvider;

        void Start()
        {
            gravityProvider = GetComponent<GravityProvider>();
            grid = GetComponent<GridController>();
        }

        public override List<Action> Run(Action action)
        {
            List<Action> actions = new List<Action>();

            if (rocket != null)
            {
                actions.Add(new Action(grid, gravityProvider));

                RocketBlock rocketBlock = rocket.GetComponent<RocketBlock>();
                NeighbourUpdateProvider nup = GetComponent<NeighbourUpdateProvider>();

                nup.AddDeletedBlocks(new List<GameObject> { rocket });

                if (rocketBlock != null)
                {
                    nup.AddDeletedBlocks(Fire(action, rocketBlock));
                }

                rocket = null;
                actions.Add(new Action(grid, nup));
            }

            if (action.workerCount == 0)
            {
                action.SilentFinish();
            }

            return actions;
        }

        public void SetRocket(GameObject spawner)
        {
            rocket = spawner;
        }

        private List<GameObject> Fire(Action action, RocketBlock rocketBlock)
        {
            HandleRocketLaunch(action, rocketBlock);

            List<RocketArrive> affected = CollectAffectedBlocks(action, rocketBlock);
            return DestroyAffectedBlocks(action, affected);
        }

        private void HandleRocketLaunch(Action action, RocketBlock rocketBlock)
        {
            int x = rocketBlock.x;
            int y = rocketBlock.y;

            action.grid.SetBlock(x, y, null);
            rocketBlock.Fly(action, 0.2f);
        }

        private List<RocketArrive> CollectAffectedBlocks(Action action, RocketBlock rocketBlock)
        {
            List<RocketArrive> affected = new();
            int x = rocketBlock.x;
            int y = rocketBlock.y;

            if (rocketBlock.Direction == RocketDirection.HORIZONTAL)
            {
                CollectInDirection(action.grid, affected, x, y, -1, 0); // Left
                CollectInDirection(action.grid, affected, x, y, 1, 0);  // Right
            }
            else
            {
                CollectInDirection(action.grid, affected, x, y, 0, -1); // Down
                CollectInDirection(action.grid, affected, x, y, 0, 1);  // Up
            }

            return affected;
        }

        private void CollectInDirection(GridController grid, List<RocketArrive> affected, int startX, int startY, int dx, int dy)
        {
            int x = startX + dx;
            int y = startY + dy;
            int distance = 1;

            while (x >= 0 && x < grid.xSize && y >= 0 && y < grid.ySize)
            {
                GameObject block = grid.GetBlock(x, y);
                if (block != null)
                {
                    affected.Add(new RocketArrive { gameObject = block, distance = distance });
                }

                x += dx;
                y += dy;
                distance++;
            }
        }

        private List<GameObject> DestroyAffectedBlocks(Action action, List<RocketArrive> affected)
        {
            List<GameObject> destroyed = new();
            int maxDistance = 0;

            foreach (var ra in affected)
                maxDistance = Mathf.Max(maxDistance, ra.distance);

            foreach (RocketArrive ra in affected)
            {
                Block b = ra.gameObject.GetComponent<Block>();
                if (b != null)
                {
                    float t = (float)ra.distance / maxDistance;
                    float duration = Mathf.Lerp(0.05f, 0.2f, t);
                    action.grid.SetBlock(b.x, b.y, null);
                    b.Pop(action, duration);
                    destroyed.Add(ra.gameObject);
                }
            }

            return destroyed;
        }
    }
}
