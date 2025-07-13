using System.Collections.Generic;
using UnityEngine;

namespace PCase
{
    [System.Serializable]
    public class PrefabRow
    {
        public List<GameObject> row;
    }

    [CreateAssetMenu(fileName = "RandomGridGenerator", menuName = "ScriptableObjects/GridGenerators/RandomGridGenerator")]
    public class RandomGridGenerator : GridGenerator
    {
        public override void Build(GridController grid)
        {
            List<List<GameObject>> blocks = new();

            RectTransform gridRect = grid.GetComponent<RectTransform>();
            float cellWidth = gridRect.rect.width / Mathf.Max(GetMaxRowLength(), 1);
            float cellHeight = gridRect.rect.height / Mathf.Max(prefabGrid.Count, 1);

            for (int y = 0; y < prefabGrid.Count; y++)
            {
                List<GameObject> rowList = new();

                if (prefabGrid[y] == null || prefabGrid[y].row == null)
                {
                    blocks.Add(rowList);
                    continue;
                }

                for (int x = 0; x < prefabGrid[y].row.Count; x++)
                {
                    GameObject prefab = GetPrefabAt(x, y);
                    if (prefab == null) continue;

                    GameObject blockGO = Instantiate(prefab, grid.transform);
                    blockGO.name = $"Block_{x}_{y}";

                    RectTransform blockRT = blockGO.GetComponent<RectTransform>();
                    blockRT.anchorMin = new Vector2(0, 1);
                    blockRT.anchorMax = new Vector2(0, 1);
                    blockRT.pivot = new Vector2(0, 1);
                    blockRT.sizeDelta = new Vector2(cellWidth, cellHeight);
                    blockRT.anchoredPosition = new Vector2(x * cellWidth, -y * cellHeight);

                    Block block = blockGO.GetComponent<Block>();
                    if (block != null)
                    {
                        block.x = x;
                        block.y = y;
                    }

                    rowList.Add(blockGO);
                }

                blocks.Add(rowList);
            }

            grid.CreateGrid(blocks);
        }

        public override GameObject Generate(GridController grid, int x, int y, float cellWidth, float cellHeight)
        {
            if (prefabs == null || prefabs.Count == 0)
                return null;

            GameObject randomPrefab = GameObject.Instantiate(prefabs[Random.Range(0, prefabs.Count)], grid.transform);
            randomPrefab.name = $"Block_{x}_{y}";

            RectTransform blockRT = randomPrefab.GetComponent<RectTransform>();
            blockRT.anchorMin = new Vector2(0, 1);
            blockRT.anchorMax = new Vector2(0, 1);
            blockRT.pivot = new Vector2(0, 1);
            blockRT.sizeDelta = new Vector2(cellWidth, cellHeight);
            blockRT.anchoredPosition = new Vector2(x * cellWidth, -y * cellHeight);

            Block block = randomPrefab.GetComponent<Block>();
            if (block != null)
            {
                block.x = x;
                block.y = y;
            }

            return randomPrefab;
        }

        private GameObject GetPrefabAt(int x, int y)
        {
            if (y < 0 || y >= prefabGrid.Count) return null;
            var row = prefabGrid[y];
            if (row == null || row.row == null || x < 0 || x >= row.row.Count) return null;
            return row.row[x];
        }

        private int GetMaxRowLength()
        {
            int max = 0;
            foreach (var row in prefabGrid)
            {
                if (row != null && row.row != null && row.row.Count > max)
                {
                    max = row.row.Count;
                }
            }
            return max;
        }
    }
}
