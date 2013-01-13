using System;
using System.Configuration;
using System.Threading;

namespace Tfs2Trello
{
    class Program
    {
        static void Main(string[] args)
        {
            try {
                var tfs = new TfsToTrelloIntegration();
                tfs.Initialize();
                while (true) {
                    Thread.Sleep(int.Parse(ConfigurationManager.AppSettings["PollingInterval"]));
                    tfs.UpdateTrelloBoard();
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                if (e.InnerException != null) {
                    Console.WriteLine(e.InnerException.Message);
                    Console.WriteLine(e.InnerException.StackTrace);
                }
                Console.ReadKey();
            }
        }
    }
}
