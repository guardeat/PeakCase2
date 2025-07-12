using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCase
{

    public enum RocketDirection
    {
        VERTICAL,
        HORIZONTAL
    }

    public class RocketBlock : Block
    {

        [SerializeField]
        private RocketDirection direction;

        public RocketDirection Direction => direction;

        public override List<Action> OnSelect(Action action)
        {
            FireRocketProvider frp = action.grid.GetComponent<FireRocketProvider>();

            frp.SetRocket(gameObject);
            List<Action> actions = new List<Action> { new Action(action.grid, frp) };
          
            return actions;
        }

        public void Fly(Action action, float duration = 0.15f)
        {
            Transform childA = transform.GetChild(0);
            Transform childB = transform.GetChild(1);

            RectTransform rect = action.grid.GetComponent<RectTransform>();
            if (rect == null)
            {
                return;
            }

            float flyDistance = direction == RocketDirection.VERTICAL
                ? rect.rect.height
                : rect.rect.width;

            Vector3 dirA = (direction == RocketDirection.VERTICAL ? Vector3.up : Vector3.left) * flyDistance;
            Vector3 dirB = -dirA;

            action.WorkStartCallBack();
            Sequence seq = DOTween.Sequence();
            seq.SetLink(gameObject);

            seq.Join(childA.DOMove(childA.position + dirA, duration).SetEase(Ease.InQuad));
            seq.Join(childB.DOMove(childB.position + dirB, duration).SetEase(Ease.InQuad));

            seq.OnComplete(() =>
            {
                Destroy(childA.gameObject);
                Destroy(childB.gameObject);
                Destroy(gameObject);
                action.WorkFinishCallBack();
            });
        }


    }

}
