using System;

namespace OnlineMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Check.CheckOnline();
            }
        }
    }
}
