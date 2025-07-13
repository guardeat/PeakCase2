using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCase
{
    [RequireComponent(typeof(SelectProvider))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(GridController))]
    public class InputProvider : Provider
    {
        private RectTransform rectTransform;
        private Vector2Int? lastInput = null;

        [SerializeField] private float retryDelay = 0.2f;
        private float inputTimer = 0f;

        void Start()
        {
            grid = GetComponent<GridController>();
            rectTransform = GetComponent<RectTransform>();
        }

        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 screenPos = Input.mousePosition;
                CheckTouch(screenPos);
            }
#endif
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Vector2 screenPos = Input.GetTouch(0).position;
                CheckTouch(screenPos);
            }

            if (lastInput != null)
            {
                inputTimer += Time.deltaTime;

                if (inputTimer >= retryDelay && grid.actions.Count == 0)
                {
                    grid.PushAction(new Action(grid, this));
                    inputTimer = 0f;
                }
            }
        }

        public override List<Action> Run(Action action)
        {
            List<Action> list = new();

            if (lastInput != null)
            {
                SelectProvider sp = GetComponent<SelectProvider>();
                sp.Select(lastInput);
                list.Add(new Action(grid, sp));
                lastInput = null;
                inputTimer = 0f;
            }

            action.SilentFinish();
            return list;
        }

        private void CheckTouch(Vector2 screenPosition)
        {
            if (lastInput != null)
                return;

            Vector2 localPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                screenPosition,
                null,
                out localPos))
            {
                Vector2 size = rectTransform.rect.size;
                Vector2 bottomLeft = -size / 2f;
                Vector2 normalized = (localPos - bottomLeft);
                normalized.x /= size.x;
                normalized.y /= size.y;

                if (normalized.x < 0f || normalized.x >= 1f || normalized.y < 0f || normalized.y >= 1f)
                    return;

                int x = Mathf.FloorToInt(normalized.x * grid.xSize);
                int y = Mathf.FloorToInt(normalized.y * grid.ySize);
                lastInput = new Vector2Int(x, grid.ySize - y - 1);
                inputTimer = 0f;

                if (grid.actions.Count == 0)
                {
                    grid.PushAction(new Action(grid, this));
                }
            }
        }
    }
}
