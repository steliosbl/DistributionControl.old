namespace DistributionCommon
{
    using System;
    using System.IO;

    public class Logger
    {
        private readonly string filename;
        private readonly bool outputToConsole;

        public Logger(string filename, bool outputToConsole)
        {
            this.filename = filename;
            this.outputToConsole = outputToConsole;
            this.Write('\n' + "----------------------------------" + '\n');
        }

        public void Log(string msg, int severity = 0)
        {
            var tags = new string[] { "[INFO]", "[WARN]", "[SEVERE]", "[CRITICAL]" };

            string message = "[" + DateTime.Now.ToString() + "] " + tags[severity] + " " + msg;

            this.Write(message + '\n');
            if (this.outputToConsole)
            {
                Console.WriteLine(message);
            }
        }

        private void Write(string message)
        {
            try
            {
                File.AppendAllText(this.filename, message);
            }
            catch (IOException)
            {
            }
        }
    }
}
