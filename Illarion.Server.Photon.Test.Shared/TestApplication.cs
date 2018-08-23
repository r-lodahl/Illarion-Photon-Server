using System;
using Microsoft.Extensions.DependencyInjection;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Illarion.Server.Photon
{
  public class TestApplication : ApplicationBase
  {
    public IOperationHandler TestOperationHandler { get; }

    public PlayerPeerBase LastCreatedPeer { get; private set; }

    public TestApplication(IOperationHandler testOperationHandler) =>
      TestOperationHandler = testOperationHandler ?? throw new ArgumentNullException(nameof(testOperationHandler));



    protected override PeerBase CreatePeer(InitRequest initRequest)
    {
      var peer = new TestPlayerPeer(initRequest);
      peer.SetCurrentOperationHandler(TestOperationHandler);
      LastCreatedPeer = peer;
      return peer;
    }

    protected override void Setup()
    {
    /*    IServiceCollection services = _serviceProviderFactory.CreateBuilder(new ServiceCollection());
        services.AddIllarionPersistanceContext(configuration);
        services.AddIllarionGameService();
        services.AddIllarionNavigationService();

        services.AddTransient<IInitialOperationHandler>(s => new InitialOperationHandler(s));
        services.AddTransient<IAccountOperationHandler>(s => new AccountOperationHandler(s));
        services.AddTransient<IPlayerOperationHandler>(s => new PlayerOperationHandler(s));

        _services = _serviceProviderFactory.CreateServiceProvider(services);

        SetupPhotonLogging(_services);
        CustomTypeRegistry.RegisterCustomTypes(_services);*/
        }

    protected override void TearDown()
    {
    }
  }
}
