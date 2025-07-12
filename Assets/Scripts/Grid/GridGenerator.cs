using System.Collections.Generic;
using UnityEngine;

namespace PCase
{

    public abstract class GridGenerator: ScriptableObject
    {
        public GameObject goal1;
        public GameObject goal2;

        public int goal1Count;
        public int goal2Count;

        public int maxMoveCount;

        public int xSize;
        public int ySize;

        public List<GameObject> prefabs = new List<GameObject>();

        [Header("Grid Prefabs (Used in Build)")]
        public List<PrefabRow> prefabGrid;

        public abstract void Build(GameGrid grid);

        public abstract GameObject Generate(GameGrid grid,  int x, int y, float cellWidth, float cellHeight);
    }

}