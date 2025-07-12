using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCase
{

    public class DuckBlock : CollectableBlock
    {
        public override List<Action> OnMove(Action action)
        {
            List<Action> actions = new List<Action>();
            if(y == action.grid.ySize - 1)
            {
                PopProvider pp = action.grid.GetComponent<PopProvider>();
                pp.AddDestroy(new List<GameObject> { gameObject });
                if (!(action.grid.actions.Count > 0 && action.grid.actions.Peek().provider is PopProvider))
                {
                    actions.Add(new Action(action.grid, pp));
                }
            }

            return actions;
        }

        public override List<Action> OnUpdate(Action action)
        {
            return OnMove(action);
        }

    }

}
