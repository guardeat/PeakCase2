using System.Collections.Generic;
using UnityEngine;

namespace PCase
{
    [RequireComponent(typeof(MoveUpdateProvider))]
    [RequireComponent(typeof(GridController))]
    public class GravityProvider : Provider
    {
        private MoveUpdateProvider moveProvider;

        void Start()
        {
            grid = GetComponent<GridController>();
            moveProvider = GetComponent<MoveUpdateProvider>();
        }

        public override List<Action> Run(Action action)
        {
            List<Action> actions = new List<Action>();
            List<GameObject> movedOrGenerated = new List<GameObject>();

            ApplyGravity(action, movedOrGenerated);
            GenerateBlocks(action, movedOrGenerated);

            if (movedOrGenerated.Count > 0)
            {
                moveProvider.AddMovedBlocks(movedOrGenerated);
                actions.Add(new Action(grid,moveProvider));
            }

            if (action.workerCount == 0)
            {
                action.SilentFinish();
            }

            return actions;
        }

        private void ApplyGravity(Action action, List<GameObject> movedList)
        {
            int width = grid.xSize;
            int height = grid.ySize;

            for (int x = 0; x < width; x++)
            {
                ApplyGravityToColumn(x, height, action, movedList);
            }
        }

        private void ApplyGravityToColumn(int x, int height, Action action, List<GameObject> movedList)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                if (grid.GetBlock(x, y) == null)
                {
                    MoveFirstNonEmptyAbove(x, y, action, movedList);
                }
            }
        }

        private void MoveFirstNonEmptyAbove(int x, int y, Action action, List<GameObject> movedList)
        {
            for (int k = y - 1; k >= 0; k--)
            {
                GameObject above = grid.GetBlock(x, k);
                if (above != null)
                {
                    RectTransform gridRect = grid.GetComponent<RectTransform>();
                    RectTransform blockRT = above.GetComponent<RectTransform>();
                    if (gridRect == null || blockRT == null) return;

                    float cellWidth = gridRect.rect.width / grid.xSize;
                    float cellHeight = gridRect.rect.height / grid.ySize;

                    Vector2 start = new Vector2(x * cellWidth, -k * cellHeight);
                    Vector2 end = new Vector2(x * cellWidth, -y * cellHeight);

                    blockRT.anchoredPosition = start;

                    Block blockComp = above.GetComponent<Block>();
                    if (blockComp != null)
                    {
                        blockComp.GoDown(action, start, end);
                        blockComp.x = x;
                        blockComp.y = y;
                    }

                    grid.SetBlock(x, y, above);
                    grid.SetBlock(x, k, null);

                    movedList.Add(above);
                    break;
                }
            }
        }

        private void GenerateBlocks(Action action, List<GameObject> movedList)
        {
            int width = grid.xSize;
            int height = grid.ySize;

            RectTransform gridRect = grid.GetComponent<RectTransform>();
            if (gridRect == null) return;

            float cellWidth = gridRect.rect.width / width;
            float cellHeight = gridRect.rect.height / height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid.GetBlock(x, y) == null)
                    {
                        GameObject newBlock = InstantiateNewBlock(x, y, cellWidth, cellHeight);
                        if (newBlock == null) continue;

                        RectTransform blockRT = newBlock.GetComponent<RectTransform>();
                        if (blockRT == null) continue;

                        Vector2 start = new Vector2(x * cellWidth, cellHeight * height);
                        Vector2 end = new Vector2(x * cellWidth, -y * cellHeight);

                        blockRT.anchoredPosition = start;

                        Block blockComp = newBlock.GetComponent<Block>();
                        if (blockComp != null)
                        {
                            blockComp.GoDown(action, start, end);
                            blockComp.x = x;
                            blockComp.y = y;
                        }

                        grid.SetBlock(x, y, newBlock);
                        movedList.Add(newBlock);
                    }
                }
            }
        }

        private GameObject InstantiateNewBlock(int x, int y, float cellWidth, float cellHeight)
        {
            return grid.generator.Generate(grid, x, y, cellWidth, cellHeight);
        }
    }
}
