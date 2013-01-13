using System;
using System.Configuration;
using System.Threading;
using Microsoft.Practices.Unity;
using Tfs2Trello.Integration;

namespace Tfs2Trello
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = Ioc.Configure(new UnityContainer());
            try {
                var tfs = container.Resolve<ITfsTrelloIntegration>();
                tfs.Initialize();
                Thread.Sleep(int.Parse(ConfigurationManager.AppSettings["PollingInterval"])); // Sleep extra at initalize so everything finishes
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
