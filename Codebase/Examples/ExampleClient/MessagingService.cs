using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ExampleClient.Models;

using ExampleContracts.Requests;
using ExampleContracts.Responses;
using ExampleContracts.Topics;

using Pigeon;
using Pigeon.Topics;

using DTO = ExampleContracts.Models;

namespace ExampleClient
{
    public class MessagingService :
        ITopicHandler<PostedMessage>,
        ITopicHandler<UserConnected>,
        ITopicHandler<UserDisconnected>,
        IDisposable
    {
        private readonly IRouter<IRouterInfo> router;

        private IDisposable messageSubscription;
        private IDisposable userConnectedSubscription;
        private IDisposable userDisconnectedSubscription;


        public MessagingService(IRouter<IRouterInfo> router)
        {
            this.router = router ?? throw new ArgumentNullException(nameof(router));
        }


        public Action<Message> OnMessagePosted { get; set; }
        public Action<User> OnUserConnected { get; set; }
        public Action<User> OnUserDisconnected { get; set; }


        public User ConnectedUser { get; private set; }


        public async Task<User> Connect(string userName)
        {
            var request = new UserConnecting(new DTO.User(-1, userName, DateTime.UtcNow));
            var response = await router.Send<UserConnecting, Response<DTO.User>>(request, TimeSpan.FromSeconds(5));
            ConnectedUser = User.FromDTO(response.Body);
            return ConnectedUser;
        }


        public async Task<User> Disconnect()
        {
            var request = new UserDisconecting(ConnectedUser.ToDTO());
            var response = await router.Send<UserDisconecting, Response<DTO.User>>(request, TimeSpan.FromSeconds(5));
            return User.FromDTO(response.Body);
        }


        public async Task<List<User>> GetAllUsers()
        {
            var request = new ConnectedUsers();
            var response = await router.Send<ConnectedUsers, ConnectedUsersList>(request, TimeSpan.FromSeconds(5));
            return response.Users.Select(u => User.FromDTO(u)).ToList();
        }


        public async Task PostMessage(Message message)
        {
            var request = new PostMessage(message.ToDTO());
            var response = await router.Send<PostMessage, Response<DTO.Message>>(request, TimeSpan.FromSeconds(5));
        }


        public void SubscribeToTopics()
        {
            messageSubscription = router.Subscribe<PostedMessage>();
            userConnectedSubscription = router.Subscribe<UserConnected>();
            userDisconnectedSubscription = router.Subscribe<UserDisconnected>();
        }


        public void UnsubscribeFromTopics()
        {
            messageSubscription?.Dispose();
            userConnectedSubscription?.Dispose();
            userDisconnectedSubscription?.Dispose();

            messageSubscription = null;
            userConnectedSubscription = null;
            userDisconnectedSubscription = null;
        }


        public void Handle(PostedMessage message) =>
            OnMessagePosted?.Invoke(Message.FromDTO(message.Message));


        public void Handle(UserConnected message) =>
            OnUserConnected?.Invoke(User.FromDTO(message.User));


        public void Handle(UserDisconnected message) =>
            OnUserDisconnected?.Invoke(User.FromDTO(message.User));


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!(ConnectedUser is null))
                        Disconnect().GetAwaiter().GetResult();

                    OnMessagePosted = null;
                    OnUserConnected = null;
                    OnUserDisconnected = null;
                }

                disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
