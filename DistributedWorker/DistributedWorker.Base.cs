namespace DistributedWorker
{
    public class Base
    {
        public readonly int ID;
        private DistributionCommon.Job.Base job;
        private System.Threading.Thread workThread;

        public Base(DistributionCommon.Job.Blueprint blueprint)
        {
            this.job = new DistributionCommon.Job.Custom(blueprint);
        }

        public bool Wake()
        {
            if (this.workThread == null)
            {
                this.workThread = new System.Threading.Thread(() => this.job.Work());
                this.workThread.Start();
            }

            return true;
        }

        public bool Sleep()
        {
            if (this.workThread != null)
            {
                this.workThread.Abort();
                this.workThread = null;
            }

            return true;
        }
    }
}
