using Illarion.Net.Common;
using Illarion.Net.Common.Operations.Initial;
using Illarion.Server.Persistence.Accounts;
using Illarion.Server.Photon.Rpc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Photon.SocketServer;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;

namespace Illarion.Server.Photon
{
  public sealed class InitialOperationHandler : PlayerPeerOperationHandler, IInitialOperationHandler
  {
    private readonly IServiceProvider _services;

    public InitialOperationHandler(IServiceProvider services) : base(services) => _services = services ?? throw new ArgumentNullException(nameof(services));

    protected override void OnDisconnect(PlayerPeerBase peer)
    {
    }

    protected override OperationResponse OnOperationRequest(PlayerPeerBase peer, OperationRequest operationRequest, SendParameters sendParameters)
    {
      if (peer == null) throw new ArgumentNullException(nameof(peer));
      if (operationRequest == null) throw new ArgumentNullException(nameof(operationRequest));

      switch ((InitialOperationCode)operationRequest.OperationCode)
      {
        case InitialOperationCode.SetCulture:
          return OnSetCultureOperationRequest(peer, operationRequest);
        case InitialOperationCode.LoginAccount:
          return OnLoginAccountOperationRequest(peer, operationRequest, sendParameters);
        case InitialOperationCode.RegisterNewAccount:
          return OnRegisterNewAccountOperationRequest(peer, operationRequest, sendParameters);
      }

      return InvalidOperation(operationRequest);
    }

    private OperationResponse OnSetCultureOperationRequest(PlayerPeerBase peer, OperationRequest operationRequest)
    {
      var operation = new SetCultureOperation(peer.Protocol, operationRequest);
      if (operation.IsValid)
      {
        peer.Culture = operation.Culture;
        return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (byte)SetCultureOperationReturnCode.Success };
      }
      else
      {
        return MalformedRequestResponse(operationRequest, operation);
      }
    }

    private OperationResponse OnLoginAccountOperationRequest(PlayerPeerBase peer, OperationRequest operationRequest, SendParameters sendParameters)
    {
      if (!sendParameters.Encrypted)
        return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (byte)LoginAccountOperationReturnCode.NotEncrypted };

      var operation = new LoginAccountOperation(peer.Protocol, operationRequest);
      if (operation.IsValid)
      {
        Account matchingAccount = _services.GetRequiredService<AccountsContext>().Accounts.
          FirstOrDefault(a => a.AccountName.Equals(operation.AccountName, StringComparison.Ordinal));

        if (matchingAccount != null && PasswordHashing.VerifyPasswordHash(operation.Password, matchingAccount.Password))
        {
          peer.Account = matchingAccount;
          peer.SetCurrentOperationHandler(_services.GetRequiredService<IAccountOperationHandler>());
          return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (byte)LoginAccountOperationReturnCode.Success };
        }
        else
        {
          return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (byte)LoginAccountOperationReturnCode.WrongNameOrPassword };
        }
      }
      else
      {
        return MalformedRequestResponse(operationRequest, operation);
      }
    }

    private OperationResponse OnRegisterNewAccountOperationRequest(PlayerPeerBase peer, OperationRequest operationRequest, SendParameters sendParameters)
    {
      if (!sendParameters.Encrypted)
        return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (byte)RegisterNewAccountOperationReturnCode.NotEncrypted };

      var operation = new RegisterNewAccountOperation(peer.Protocol, operationRequest);
      if (operation.IsValid && Validator.TryValidateObject(operation, new ValidationContext(operation), null))
      {
        AccountsContext accountsContext = _services.GetRequiredService<AccountsContext>();
        DbSet<Account> accounts = accountsContext.Accounts;
        Account matchingAccountName = accounts.Where(a => a.AccountName.Equals(operation.AccountName, StringComparison.Ordinal)).FirstOrDefault();

        if (matchingAccountName != null)
        {
          return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (byte)RegisterNewAccountOperationReturnCode.AccountNameAlreadyUsed };
        }

        Account matchingMail = accounts.Where(a => (new MailAddress(a.EMail).Equals(operation.EMail))).FirstOrDefault();

        if (matchingMail != null)
        {
          return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (byte)RegisterNewAccountOperationReturnCode.EMailAlreadyUsed };
        }

        accounts.Add(new Account(operation.AccountName) { EMail = operation.EMail.ToString(), Password = PasswordHashing.GetPasswordHash(operation.Password) });
        accountsContext.SaveChanges();
        return new OperationResponse(operationRequest.OperationCode) { ReturnCode = (byte)RegisterNewAccountOperationReturnCode.Success };
      }
      else
      {
        return MalformedRequestResponse(operationRequest, operation);
      }
    }
  }
}
