using System;
using System.Threading.Tasks;

namespace CTGPR.Downloader
{
    /// <summary>
    /// Program class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entry point of the program.
        /// </summary>
        public static async Task Main()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Title = "CTGP-R Ghost Downloader";

            await Downloader.Leaderboards();
            Downloader.Shutdown();

            Console.WriteLine(Environment.NewLine + "Press ENTER to continue . . .");
            Console.ReadLine();
        }
    }
}
