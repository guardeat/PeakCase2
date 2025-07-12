using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCase
{

    [RequireComponent(typeof(GravityProvider))]
    [RequireComponent(typeof(NeighbourUpdateProvider))]
    [RequireComponent(typeof(GameGrid))]
    public class SpawnRocketProvider : Provider
    {
        public List<GameObject> rocketSpawners { get; private set; } = new List<GameObject>();
        public GameObject mainSpawner { get; private set; } = null;

        private GravityProvider gravityProvider;

        void Start()
        {
            gravityProvider = GetComponent<GravityProvider>();
            grid = GetComponent<GameGrid>();
        }

        public override List<Action> Run(Action action)
        {
            List<Action> actions = new List<Action>();
            if (mainSpawner != null)
            {
                actions.Add(new Action(grid, gravityProvider));

                CandyBlock candyBlock = mainSpawner.GetComponent<CandyBlock>();

                if(candyBlock == null)
                {
                    return actions;
                }

                Vector2 candyPos = candyBlock.GetComponent<RectTransform>().position;

                foreach (GameObject block in rocketSpawners)
                {
                    CandyBlock b = block.GetComponent<CandyBlock>();

                    b.GoAndPop(action, candyPos);

                    grid.SetBlock(b.x, b.y, null);
                }

                GameObject rocket = candyBlock.SpawnRocket(action);

                if (rocket != null)
                {
                    Block rocketBlock = rocket.GetComponent<Block>();
                    grid.SetBlock(rocketBlock.x, rocketBlock.y, rocket);
                }
                
                NeighbourUpdateProvider nup = GetComponent<NeighbourUpdateProvider>();
                nup.AddDeletedBlocks(rocketSpawners);
                rocketSpawners.Clear();
                mainSpawner = null;
                actions.Add(new Action(grid, nup));
            }

            if (action.workerCount == 0)
            {
                action.SilentFinish();
            }

            return actions;
        }

        public void SetSpawners(List<GameObject> newSpawners)
        {
            rocketSpawners = newSpawners;
        }

        public void SetMainSpawner(GameObject spawner)
        {
            mainSpawner = spawner;
        }
    }

}
