namespace DistributedWorker
{
    public class Worker
    {
        public readonly int ID;
        private DistributedJob.Base job;
        private System.Threading.Thread workThread;

        public Worker(DistributedJob.Blueprint blueprint)
        {
            this.job = new DistributedJob.Custom(blueprint);
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
