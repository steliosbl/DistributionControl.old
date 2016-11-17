namespace DistributedWorker
{
    public class Worker
    {
        public readonly int ID;
        private DistributionCommon.Job.Base job;
        private System.Threading.Thread workThread;

        public Worker(DistributionCommon.Job.Blueprint blueprint)
        {
            this.job = new DistributionCommon.Job.Custom(blueprint);
        }

        public bool Awake
        {
            get
            {
                return this.workThread != null;
            }
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
