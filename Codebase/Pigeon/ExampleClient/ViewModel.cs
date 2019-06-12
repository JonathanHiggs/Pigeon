using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using ExampleContracts;

using Pigeon;
using Pigeon.Topics;

namespace ExampleClient
{
    public class ViewModel : INotifyPropertyChanged, ITopicHandler<Message>, ITopicHandler<UserConnected>, ITopicHandler<UserDisconnected>
    {
        private readonly IRouter<IRouterInfo> router;

        private readonly object messagesLock = new object();
        private readonly object usersLock = new object();
        private readonly object messageIdLock = new object();

        private int userId;
        private string userName;
        private bool connected;
        private string inputContent;
        private bool sending;

        private int messageId = 1;
        private IDisposable messageSubscription;
        private IDisposable userConnectedSubscription;
        private IDisposable userDisconnectedSubscription;


        public ViewModel(IRouter<IRouterInfo> router)
        {
            this.router = router ?? throw new ArgumentNullException(nameof(router));

            BindingOperations.EnableCollectionSynchronization(Messages, messagesLock);
            BindingOperations.EnableCollectionSynchronization(Users, usersLock);
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public int UserId
        {
            get => userId;
            set => Set(ref userId, value);
        }


        public string UserName
        {
            get => userName;
            set => Set(ref userName, value);
        }


        public bool Connected
        {
            get => connected;
            set => Set(ref connected, value);
        }


        public string InputContent
        {
            get => inputContent;
            set => Set(ref inputContent, value);
        }


        public bool Sending
        {
            get => sending;
            set => Set(ref sending, value);
        }


        public ObservableCollection<ReceivedMessage> Messages { get; } = new ObservableCollection<ReceivedMessage>();


        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();


        public async Task Connect()
        {
            if (Connected)
                return;

            var connected = new Connect(UserName);

            var response = await router.Send<Connect, Connected>(connected);

            if (response.Success)
            {
                UserId = response.UserId;
                Connected = true;

                messageSubscription = router.Subscribe<Message>();
                userConnectedSubscription = router.Subscribe<UserConnected>();
                userDisconnectedSubscription = router.Subscribe<UserDisconnected>();
            }
            else
            {
                MessageBox.Show($"Failed to connect\n\n{response.Reason}");
            }
        }


        public async Task Disconnect()
        {
            if (!Connected)
                return;

            var disconnect = new Disconect(UserId, UserName);
            var response = await router.Send<Disconect, Disconnected>(disconnect);

            if (response.Success)
            {
                UserId = -1;
                Connected = false;
                messageSubscription?.Dispose();
                messageSubscription = null;
            }
            else
            {
                MessageBox.Show($"Failed to disconnect\n\n{response.Reason}");
            }
        }


        public async Task PostMessage()
        {
            Sending = true;

            int newMessageId;
            lock (messageIdLock)
            {
                newMessageId = messageId++;
            }

            var message = new Message(UserId, UserName, newMessageId++, InputContent, DateTime.UtcNow);

            var response = await router.Send<Message, MessagePosted>(message);

            if (response.Success)
            {
                InputContent = "";
            }
            else
            {
                MessageBox.Show($"Unable to send message\n\n{response.Reason}");
            }

            Sending = false;
        }


        public void Handle(Message message)
        {
            Messages.Add(new ReceivedMessage(message.UserId, message.UserName, message.Content, message.Timestamp));
        }


        public void Handle(UserConnected message)
        {
            if (Users.Any(u => u.UserId == message.UserId))
                return;

            Users.Add(new User(message.UserId, message.UserName, message.Timestamp));
        }


        public void Handle(UserDisconnected message)
        {
            var user = Users.SingleOrDefault(u => u.UserId == message.UserId);

            if (user is null)
                return;

            Users.Remove(user);
        }



        private void Set<T>(ref T store, T value, [CallerMemberName] string propertyName = "")
        {
            store = value;
            RaisePropertyChanged(propertyName);
        }


        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
