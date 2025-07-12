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
    [RequireComponent(typeof(GameGrid))]
    public class FireRocketProvider : Provider
    {
        public GameObject rocket { get; private set; } = null;

        private GravityProvider gravityProvider;

        void Start()
        {
            gravityProvider = GetComponent<GravityProvider>();
            grid = GetComponent<GameGrid>();
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
            int x = rocketBlock.x;
            int y = rocketBlock.y;

            action.grid.SetBlock(x, y, null);
            rocketBlock.Fly(action, 0.2f);

            List<RocketArrive> affected = new();
            List<GameObject> destroyed = new();
            int maxDistance = 0;

            if (rocketBlock.Direction == RocketDirection.HORIZONTAL)
            {
                for (int i = x - 1, d = 1; i >= 0; i--, d++)
                {
                    GameObject block = action.grid.GetBlock(i, y);
                    if (block != null)
                    {
                        affected.Add(new RocketArrive { gameObject = block, distance = d });
                        maxDistance = Mathf.Max(maxDistance, d);
                    }
                }

                for (int i = x + 1, d = 1; i < action.grid.xSize; i++, d++)
                {
                    GameObject block = action.grid.GetBlock(i, y);
                    if (block != null)
                    {
                        affected.Add(new RocketArrive { gameObject = block, distance = d });
                        maxDistance = Mathf.Max(maxDistance, d);
                    }
                }
            }
            else
            {
                for (int j = y - 1, d = 1; j >= 0; j--, d++)
                {
                    GameObject block = action.grid.GetBlock(x, j);
                    if (block != null)
                    {
                        affected.Add(new RocketArrive { gameObject = block, distance = d });
                        maxDistance = Mathf.Max(maxDistance, d);
                    }
                }

                for (int j = y + 1, d = 1; j < action.grid.ySize; j++, d++)
                {
                    GameObject block = action.grid.GetBlock(x, j);
                    if (block != null)
                    {
                        affected.Add(new RocketArrive { gameObject = block, distance = d });
                        maxDistance = Mathf.Max(maxDistance, d);
                    }
                }
            }

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
