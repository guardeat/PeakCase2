using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PCase
{

    public class GameGrid : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI movesPanel;

        [SerializeField]
        TextMeshProUGUI goal1Panel;
        [SerializeField]
        TextMeshProUGUI goal2Panel;

        [SerializeField]
        Image image1Panel;
        [SerializeField]
        Image image2Panel;

        private int goal1Count;
        private int goal2Count;

        private GameObject goal1;
        private GameObject goal2;

        public int xSize { get; private set; } = 9;
        public int ySize { get; private set; } = 9;

        private int moves;

        public List<List<GameObject>> data { get; private set; } = new();

        public Stack<Action> actions { get; private set; } = new Stack<Action>();
        private List<Action> newActions;

        public GridGenerator generator;

        private bool finished = false;

        void Start()
        {
            SetupGrid();
        }

        public void PushAction(Action action)
        {
            if(finished) 
                return;

            if (actions.Count == 0)
            {
                actions.Push(action);
                PopAction();
            }
        }

        private void PopAction()
        {
            Action action = actions.Peek();

            action.OnFinished += OnActionFinished;

            newActions = action.Invoke();

            if(action.status == ActionStatus.COMPLETE)
            {
                OnActionFinished(action);
            }
        }

        private void OnActionFinished(Action action)
        {
            action.OnFinished -= OnActionFinished;

            actions.Pop();

            foreach(Action ac in newActions)
            {
                actions.Push(ac);
            }

            newActions = null;

            if (actions.Count > 0)
            {
                PopAction();
            }
            else if ((goal1Count <= 0 && goal2Count <= 0) || moves == 0)
            {
                FinishGame(goal1Count <= 0 && goal2Count <= 0);
            }
        }

        public void SetBlock(int x, int y, GameObject value)
        {
            if (x >= 0 && x < xSize && y >= 0 && y < ySize)
            {
                data[y][x] = value;
            }
        }

        public GameObject GetBlock(int x, int y)
        {
            if (x >= 0 && x < xSize && y >= 0 && y < ySize)
            {
                return data[y][x];
            }

            return null;
        }

        public void CreateGrid(List<List<GameObject>> newData)
        {
            if (newData == null || newData.Count == 0 || newData[0].Count == 0)
            {
                return;
            }

            data = newData;
            xSize = newData[0].Count;
            ySize = newData.Count;
        }

        public void DecreaseMove()
        {
            --moves;
            movesPanel.text = moves.ToString();
        }

        private void FinishGame(bool win)
        {
            if (win) 
            {
                Debug.Log("win");
            }
            else
            {
                Debug.Log("Lose");
            }
            finished = true;
        }

        public void SetupGrid()
        {
            if (generator == null)
            {
                return;
            }

            xSize = generator.xSize;
            ySize = generator.ySize;
            moves = generator.maxMoveCount;

            RectTransform rt = transform.parent.GetComponent<RectTransform>();
            if (rt != null && xSize != 0)
            {
                float scaleY = (float)ySize / xSize;
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, scaleY * rt.sizeDelta.x);
            }

            generator.Build(this);

            movesPanel.text = moves.ToString();

            SelectGoal();
        }

        private void SelectGoal()
        {
            goal1 = Instantiate(generator.goal1);
            goal2 = Instantiate(generator.goal2);
            goal1Count = generator.goal1Count;
            goal2Count = generator.goal2Count;

            Sprite sprite1 = goal1.GetComponent<Image>()?.sprite;
            Sprite sprite2 = goal2.GetComponent<Image>()?.sprite;

            if (sprite1 != null) image1Panel.sprite = sprite1;
            if (sprite2 != null) image2Panel.sprite = sprite2;
        }

        public GameObject GetGoal(GameObject check)
        {
            if (!check) return null;

            Block b = check.GetComponent<Block>();
            if (b == null) return null;

            if (goal1.GetComponent<Block>().IsSameBlock(b) && goal1Count > 0)
            {
                return goal1;
            }
            if (goal2.GetComponent<Block>().IsSameBlock(b) && goal2Count > 0)
            {
                return goal2;
            }

            return null;
        }

        public Vector2? GetGoalPos(GameObject check)
        {
            if (!check) return null;

            Block b = check.GetComponent<Block>();
            if (b == null) return null;

            if (goal1.GetComponent<Block>().IsSameBlock(b) && goal1Count > 0)
            {
                return image1Panel.GetComponent<RectTransform>().position;
            }
            if (goal2.GetComponent<Block>().IsSameBlock(b) && goal2Count > 0)
            {
                return image2Panel.GetComponent<RectTransform>().position;
            }

            return null;
        }

        public void UpdateGoal(GameObject block)
        {
            if (!block) return;

            Block b = block.GetComponent<Block>();
            if (b == null) return;

            if (goal1.GetComponent<Block>().IsSameBlock(b) && goal1Count > 0)
            {
                goal1Count--;
            }
            else if (goal2.GetComponent<Block>().IsSameBlock(b) && goal2Count > 0)
            {
                goal2Count--;
            }

            goal1Panel.text = goal1Count.ToString();
            goal2Panel.text = goal2Count.ToString();
        }

    }

}
