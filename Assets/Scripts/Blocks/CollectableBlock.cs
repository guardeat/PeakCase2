using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace PCase
{

    public class CollectableBlock : Block
    {
        protected void GoGoal(Action action, Vector2 pos, float dur = 0.30f)
        {
            RectTransform rt = GetComponent<RectTransform>();
            if (rt == null) return;

            //action.WorkStartCallBack();

            Sequence seq = DOTween.Sequence();
            seq.SetLink(gameObject);

            seq.Join(transform.DOMove(pos, dur).SetEase(Ease.InOutQuad));
            seq.Join(transform.DOScale(Vector3.zero, dur).SetEase(Ease.InBack));

            seq.OnKill(() =>
            {
                AudioManager.Instance.Play("block_collect");
                action.grid.UpdateGoal(gameObject);
                Destroy(gameObject);
                //action.WorkFinishCallBack();
            });


            action.WorkStartCallBack();
            DOVirtual.DelayedCall(dur / 3f, () =>
            {
                action.WorkFinishCallBack();
            }).SetLink(gameObject);
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
                base.Pop(action, dur);
            }
        }
    }

}