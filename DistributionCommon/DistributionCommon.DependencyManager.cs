namespace DistributionCommon
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class DependencyManager
    {
        public DependencyManager(string[] dependencies)
        {
            this.Depencencies = dependencies;
        }

        public string[] Depencencies { get; private set; }

        public List<string> FindMissing()
        {
            var result = new List<string>();
            foreach (string file in this.Depencencies)
            {
                if (!File.Exists(file))
                {
                    result.Add(file);
                }
            }

            return result;
        }
    }
}
