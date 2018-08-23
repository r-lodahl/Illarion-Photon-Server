using System;
using System.Collections.Generic;
using Illarion.Net.Common;
using Illarion.Net.Common.Events.Player;
using Illarion.Net.Common.Operations.Player;
using Illarion.Server.Events;
using Microsoft.Extensions.DependencyInjection;
using Photon.SocketServer;
using Photon.SocketServer.UnitTesting;
using Xunit;
using Xunit.Abstractions;

namespace Illarion.Server.Photon
{
    public sealed class PlayerOperationHandlerTest
    {

        private readonly ITestOutputHelper _output;

        public PlayerOperationHandlerTest(ITestOutputHelper output) =>
            _output = output ?? throw new ArgumentNullException(nameof(output));

        [Trait("Category", "Networking")]
        [Theory]
        [MemberData(nameof(SendChatMessageTestData))]
        public void SendChatMessageTest(string message, MapChatChannelType chatType)
        {
            PhotonApplicationProxy applicationProxy = StartTestApplication(_output);
            var application = (TestApplication) applicationProxy.Application;
            
            try
            {
                var sendTestClient = new UnitTestClient();
                sendTestClient.Connect(applicationProxy);

                sendTestClient.SendOperationRequest(new OperationRequest(
                    (byte) PlayerOperationCode.LoadingReady,
                    new Dictionary<byte, object>()
                ));

                SendResult sendResult = sendTestClient.SendOperationRequest(new OperationRequest(
                    (byte) PlayerOperationCode.SendMessage,
                    new Dictionary<byte, object>()
                    {
                        {(byte) SendMessageOperationRequestParameterCode.ChatType, (byte) chatType},
                        {(byte) SendMessageOperationRequestParameterCode.Message, message}
                    }
                ));

                Assert.Equal(SendResult.Ok, sendResult);

                EventData data = sendTestClient.WaitForEvent(105);
                Assert.NotNull(data);
                Assert.Equal((byte) PlayerEventCode.Chat, data.Code);
                Assert.Equal(message, data.Parameters[(byte) ChatMessageEventParameterCode.Message]);
                Assert.Equal(chatType,
                    (MapChatChannelType) data.Parameters[(byte) ChatMessageEventParameterCode.Message]);
                Assert.Equal(application.LastCreatedPeer.Character.CharacterId,
                    data.Parameters[(byte) ChatMessageEventParameterCode.CharacterId]);
                Assert.IsAssignableFrom<IPlayerOperationHandler>(application.LastCreatedPeer.CurrentOperationHandler);
            }
            finally
            {
                applicationProxy.Stop();
            }
        }

        public static IEnumerable<object[]> SendChatMessageTestData()
        {
            yield return new object[] { "Hallo!", MapChatChannelType.Speaking };
            yield return new object[] { "Hallo!", MapChatChannelType.Global };
            yield return new object[] { "Hallo!", MapChatChannelType.Whispering };
            yield return new object[] { "Hallo!", MapChatChannelType.Yelling };
            yield return new object[] { "Hallo!", null };
            yield return new object[] { "\n", MapChatChannelType.Speaking };
            yield return new object[] { null, MapChatChannelType.Speaking };
            yield return new object[] { string.Empty, MapChatChannelType.Speaking };
            yield return new object[] { "🤷 Shrug ¯\\_(ツ)_/¯", MapChatChannelType.Speaking };
            yield return new object[] { "ThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongStringThisIsAVeryLongString", MapChatChannelType.Speaking };
        }

        private static PhotonApplicationProxy StartTestApplication(ITestOutputHelper outputHelper) =>
            StartTestApplication(new ServiceCollection().AddIllarionTestLogging(outputHelper).BuildServiceProvider());

        private static PhotonApplicationProxy StartTestApplication(IServiceProvider serviceProvider)
        {
            var operationHandler = new PlayerOperationHandler(serviceProvider);
            var application = new TestApplication(operationHandler);
            var applicationProxy = new PhotonApplicationProxy(application);
            applicationProxy.Start();

            return applicationProxy;
        }
    }
}
