using System;
using System.Linq;
using Illarion.Net.Common;
using Illarion.Net.Common.Operations.Account;
using Illarion.Server.Persistence.Server;
using Illarion.Server.Photon.Rpc;
using Microsoft.Extensions.DependencyInjection;
using Photon.SocketServer;

namespace Illarion.Server.Photon
{
  public sealed class AccountOperationHandler : PlayerPeerOperationHandler, IAccountOperationHandler
  {
    private readonly IServiceProvider _services;

    public AccountOperationHandler(IServiceProvider services) => _services = services ?? throw new ArgumentNullException(nameof(services));

    protected override void OnDisconnect(PlayerPeerBase peer)
    {
    }

    protected override OperationResponse OnOperationRequest(PlayerPeerBase peer, OperationRequest operationRequest, SendParameters sendParameters)
    {
      if (peer == null) throw new ArgumentNullException(nameof(peer));
      if (operationRequest == null) throw new ArgumentNullException(nameof(operationRequest));

      switch ((AccountOperationCode)operationRequest.OperationCode)
      {
        case AccountOperationCode.ListCharacters:
          break;
        case AccountOperationCode.LoginCharacter:
          return OnLoginCharacterOperationRequest(peer, operationRequest);
        case AccountOperationCode.LogoutAccount:
          break;
      }

      return InvalidOperation(operationRequest);
    }

    private OperationResponse OnLoginCharacterOperationRequest(PlayerPeerBase peer, OperationRequest operationRequest)
    {
      var operation = new LoginCharacterOperation(peer.Protocol, operationRequest);
      if (operation.IsValid)
      {
        Character matchingCharacter = _services.GetRequiredService<ServerContext>().Characters.
          Where(c => c.AccountId == peer.Account.AccountId && c.CharacterId == operation.CharacterId).
          FirstOrDefault();
        if (matchingCharacter != null)
        {
          switch (matchingCharacter.Status)
          {
            case CharacterStatus.Default:
              peer.Character = matchingCharacter;
              // TODO: Spawn the character in the world
              return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (byte)LoginCharacterOperationReturnCode.Success };
            case CharacterStatus.Blocked:
              return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (byte)LoginCharacterOperationReturnCode.Blocked };
          }
        }
        return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (byte)LoginCharacterOperationReturnCode.DoesNotExist };
      }
      else
      {
        return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (byte)LoginCharacterOperationReturnCode.Malformed, DebugMessage = operation.GetErrorMessage() };
      }
    }
  }
}