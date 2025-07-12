using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PCase
{

    public enum CandyColor
    {
        YELLOW,
        RED,
        BLUE,
        GREEN,
        PURPLE,
    }

    public class CandyBlock : CollectableBlock
    {
        [SerializeField]
        private CandyColor candyColor;

        public CandyColor CandyColor => candyColor;

        [SerializeField]
        private List<GameObject> particlePrefabs;

        [SerializeField]
        private int minNeighbors = 3;

        [SerializeField]
        private int minNeighborsForBomb = 5;

        [SerializeField]
        private List<GameObject> rocketPrefabs;

        public override List<Action> OnSelect(Action action)
        {
            List<Action> actions = new();

            var connected = GetConnectedNeighbors(action.grid);

            if(connected.Count >= minNeighborsForBomb)
            {
                connected.Remove(gameObject);
                SpawnRocketProvider srp = action.grid.GetComponent<SpawnRocketProvider>();
                srp.SetSpawners(connected);
                srp.SetMainSpawner(gameObject);

                actions.Add(new Action(action.grid, srp));
            }
            else if(connected.Count >= minNeighbors)
            {
                PopProvider pp = action.grid.GetComponent<PopProvider>();

                pp.AddDestroy(connected);

                actions.Add(new Action(action.grid, pp));
            }
            else
            {
                Shake(action);
            }

            return actions;
        }

        public override void Pop(Action action, float duration = 0.20f)
        {
            SpawnParticles();
            base.Pop(action, duration);
        }

        public void GoAndPop(Action action, Vector2 pos, float dur = 0.15f)
        {
            Vector2? goalPos = action.grid.GetGoalPos(gameObject);
            if (goalPos != null)
            {
                GoGoal(action, goalPos.Value);
            }
            else
            {
                RectTransform rt = GetComponent<RectTransform>();
                if (rt == null) return;

                action.WorkStartCallBack();

                if (popAudioName != null)
                {
                    AudioManager.Instance.Play(popAudioName);
                }

                Sequence seq = DOTween.Sequence();
                seq.SetLink(gameObject);

                seq.Join(transform.DOMove(pos, dur).SetEase(Ease.InOutQuad));
                seq.Join(transform.DOScale(Vector3.zero, dur).SetEase(Ease.InBack));

                seq.OnKill(() =>
                {
                    Destroy(gameObject);
                    action.WorkFinishCallBack();
                });
            }
        }

        private List<GameObject> GetConnectedNeighbors(GameGrid grid)
        {
            List<GameObject> connected = new List<GameObject>();
            HashSet<(int, int)> visited = new HashSet<(int, int)>();
            Queue<(int, int)> toVisit = new Queue<(int, int)>();

            toVisit.Enqueue((this.x, this.y));

            while (toVisit.Count > 0)
            {
                var (cx, cy) = toVisit.Dequeue();

                if (visited.Contains((cx, cy)))
                    continue;

                GameObject blockObj = grid.GetBlock(cx, cy);
                if (blockObj == null)
                    continue;

                CandyBlock cb = blockObj.GetComponent<CandyBlock>();
                if (cb == null || cb.candyColor != this.candyColor)
                    continue;

                visited.Add((cx, cy));
                connected.Add(blockObj);

                toVisit.Enqueue((cx + 1, cy));
                toVisit.Enqueue((cx - 1, cy));
                toVisit.Enqueue((cx, cy + 1));
                toVisit.Enqueue((cx, cy - 1));
            }

            return connected;
        }

        public void SpawnParticles()
        {
            if (particlePrefabs == null || particlePrefabs.Count == 0)
                return;

            int particleCount = Random.Range(5, 10);

            for (int i = 0; i < particleCount; i++)
            {
                GameObject particlePrefab = particlePrefabs[Random.Range(0, particlePrefabs.Count)];

                if (particlePrefab != null)
                {
                    GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity, gameObject.transform.parent);

                    particle.GetComponent<Image>().color = GetColorByCandy(candyColor);

                    float radius = 0.3f;
                    Vector2 offset = Random.insideUnitCircle * radius;
                    particle.transform.position += new Vector3(offset.x, offset.y, 0f);
                    particle.transform.position = new Vector3(
                        particle.transform.position.x,
                        particle.transform.position.y,
                        -2f
                    );
                }
            }
        }

        private Color GetColorByCandy(CandyColor color)
        {
            switch (color)
            {
                case CandyColor.YELLOW:
                    return Color.yellow;
                case CandyColor.RED:
                    return Color.red;
                case CandyColor.BLUE:
                    return Color.blue;
                case CandyColor.GREEN:
                    return Color.green;
                case CandyColor.PURPLE:
                    return new Color(0.6f, 0f, 0.8f);
                default:
                    return Color.white;
            }
        }

        public GameObject SpawnRocket(Action action, float duration = 0.20f)
        {
            if (action.grid.GetGoalPos(gameObject) != null)
            {
                action.grid.UpdateGoal(gameObject);
            }

            GameObject rocket = CreateRocketObject();

            action.WorkStartCallBack();

            transform.DOScale(Vector3.zero, duration / 2f)
                .SetEase(Ease.InBack)
                .SetLink(gameObject)
                .OnKill(() =>
                {
                    AnimateRocketIn(rocket, action, duration / 2f);
                    Destroy(gameObject);
                    action.WorkFinishCallBack();
                });

            return rocket;
        }

        private GameObject CreateRocketObject()
        {
            if (rocketPrefabs == null || rocketPrefabs.Count == 0)
                return null;

            GameObject prefab = rocketPrefabs[Random.Range(0, rocketPrefabs.Count)];
            if (prefab == null)
                return null;

            GameObject rocket = Instantiate(prefab, transform.parent);
            RectTransform rocketRect = rocket.GetComponent<RectTransform>();
            RectTransform sourceRect = GetComponent<RectTransform>();

            rocketRect.anchoredPosition = sourceRect.anchoredPosition;
            rocketRect.localRotation = sourceRect.localRotation;
            rocketRect.localScale = Vector3.zero;
            rocketRect.sizeDelta = sourceRect.sizeDelta;
            rocketRect.anchorMin = sourceRect.anchorMin;
            rocketRect.anchorMax = sourceRect.anchorMax;
            rocketRect.pivot = sourceRect.pivot;

            Block rocketBlock = rocket.GetComponent<Block>();
            rocketBlock.x = x;
            rocketBlock.y = y;

            return rocket;
        }

        private void AnimateRocketIn(GameObject rocket, Action action, float duration)
        {
            if (rocket == null)
                return;

            RectTransform rocketRect = rocket.GetComponent<RectTransform>();

            action.WorkStartCallBack();

            rocketRect.DOScale(1f, duration)
                .SetEase(Ease.OutBounce)
                .SetLink(rocket)
                .OnKill(() => action.WorkFinishCallBack());
        }

        public override bool IsSameBlock(Block other)
        {
            if(base.IsSameBlock(other))
            {
                CandyBlock candyBlock = other as CandyBlock;
                if(candyBlock != null)
                {
                    return candyBlock.candyColor == candyColor;
                }
            }

            return false;
        }

    }

}
