using Microsoft.Practices.Unity;
using Tfs2Trello.Integration;
using Tfs2Trello.Tfs;
using Tfs2Trello.Trello;
using TrelloNet;

namespace Tfs2Trello
{
    public class Ioc
    {
        public static IUnityContainer Container { get; private set; }

        public static IUnityContainer Configure(IUnityContainer container)
        {
            Container = container;
            container.RegisterType<ITrelloClient, TrelloClient>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITfsTrelloIntegration, TfsTrelloIntegration>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITfsClient, TfsClient>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITrelloConfig, TrelloConfig>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITrello, TrelloNet.Trello>();
            return container;
        }
    }
}