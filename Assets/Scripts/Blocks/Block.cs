using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace PCase
{
    public class Block : MonoBehaviour
    {
        public int x;
        public int y;

        [SerializeField]
        protected string popAudioName = "cube_explode";

        public virtual List<Action> OnSelect(Action action)
        {
            Shake(action);
            return new();
        }

        public virtual List<Action> OnUpdate(Action action)
        {
            return new();
        }

        public virtual List<Action> OnMove(Action action) 
        { 
            return new(); 
        }

        public virtual void Pop(Action action, float dur = 0.20f)
        {
            action.WorkStartCallBack();

            if (popAudioName != null)
            {
                AudioContoller.Instance.Play(popAudioName);
            }

            transform.DOScale(Vector3.zero, dur)
                .SetEase(Ease.InBack)
                .SetLink(gameObject)
                .OnKill(() =>
                {
                    Destroy(gameObject);
                    action.WorkFinishCallBack();
                });
        }

        public virtual void GoDown(Action action, Vector2 start, Vector2 end, float dur = 0.30f)
        {
            RectTransform rt = GetComponent<RectTransform>();
            if (rt == null) return;

            action.WorkStartCallBack();

            rt.anchoredPosition = start;

            rt.DOAnchorPos(end, dur)
              .SetEase(Ease.OutBounce) 
              .SetLink(gameObject)
              .OnKill(() =>
              {
                  action.WorkFinishCallBack();
              });
        }

        public virtual void Shake(Action action, float dur = 0.15f)
        {
            action.WorkStartCallBack();

            transform.DOShakePosition(
                duration: dur,
                strength: new Vector3(1.7f, 1.7f, 0),
                vibrato: 20,
                randomness: 90,
                snapping: false,
                fadeOut: true)
                .SetLink(gameObject)
                .OnKill(() =>
                {
                    action.WorkFinishCallBack();
                });
        }

        public virtual bool IsSameBlock(Block other)
        {
            return other != null && this.GetType() == other.GetType();
        }
    }
}
