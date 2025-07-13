using System.Collections.Generic;

namespace PCase
{

    public enum ActionStatus
    {
        COMPLETE,
        RUNNING,
    }

    public class Action
    {
        public ActionStatus status { get; private set; }

        public GridController grid { get; protected set; }
        public Provider provider { get; protected set; }

        public event System.Action<Action> OnFinished;

        public int workerCount { get; protected set; } = 0;

        public Action(GridController grid, Provider provider)
        {
            this.grid = grid;
            this.provider = provider;
        }

        public virtual List<Action> Invoke()
        {
            status = ActionStatus.RUNNING;
            return provider.Run(this);
        }

        protected void Finish()
        {
            if (status == ActionStatus.RUNNING)
            {
                status = ActionStatus.COMPLETE;
                OnFinished?.Invoke(this);
            }
        }

        public virtual void WorkStartCallBack()
        {
            workerCount++;
        }

        public virtual void WorkFinishCallBack()
        {
            workerCount--;
            if (workerCount == 0)
            {
                Finish();
            }
        }

        public void SilentFinish()
        {
            status = ActionStatus.COMPLETE;
        }
    }

}