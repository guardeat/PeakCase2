using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCase
{

    public class BalloonBlock : CollectableBlock
    {
        public override List<Action> OnUpdate(Action action)
        {
            PopProvider pp = action.grid.GetComponent<PopProvider>();
            pp.AddDestroy(new List<GameObject> { gameObject });

            List<Action> actions = new List<Action>();
            if (!(action.grid.actions.Count > 0 && action.grid.actions.Peek().provider is PopProvider))
            {
                actions.Add(new Action(action.grid, pp));
            }

            return actions;
        }

        public override void Pop(Action action, float dur = 0.20f)
        {
            Vector2? goalPos = action.grid.GetGoalPos(gameObject);
            if (goalPos != null)
            {
                GoGoal(action, goalPos.Value);
            }
            else 
            {
                action.WorkStartCallBack();

                Sequence popSequence = DOTween.Sequence();

                float scaleUpFactor = 1.2f;
                float scaleUpDuration = dur * 0.6f;
                float vanishDuration = dur * 0.1f;

                popSequence.Append(transform.DOScale(Vector3.one * scaleUpFactor, scaleUpDuration)
                                            .SetEase(Ease.OutQuad));

                popSequence.Append(transform.DOScale(Vector3.zero, vanishDuration)
                                            .SetEase(Ease.InBack));

                popSequence.SetLink(gameObject);

                popSequence.OnComplete(() =>
                {
                    AudioManager.Instance.Play(popAudioName);
                    Destroy(gameObject);
                    action.WorkFinishCallBack();
                });
            }
        }

    }

}
