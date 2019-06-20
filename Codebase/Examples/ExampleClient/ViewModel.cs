using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using ExampleClient.Models;

namespace ExampleClient
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly MessagingService messagingService;
        
        private readonly object messagesLock = new object();
        private readonly object usersLock = new object();

        private bool connected;
        private string input;
        private User user = new User(-1, string.Empty, null, null);


        public ViewModel(MessagingService messagingService)
        {
            this.messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));

            messagingService.OnMessagePosted = message => Messages.Add(message);

            messagingService.OnUserConnected = user =>
            {
                if (!Users.Contains(user, User.Comparer.Default))
                    Users.Add(user);
            };

            messagingService.OnUserDisconnected = user =>
            {
                var listUser = Users.SingleOrDefault(u => User.Comparer.Default.Equals(user, u));
                if (listUser is null)
                    return;
                Users.Remove(listUser);
            };

            BindingOperations.EnableCollectionSynchronization(Messages, messagesLock);
            BindingOperations.EnableCollectionSynchronization(Users, usersLock);
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public bool Connected
        {
            get => connected;
            set => Set(ref connected, value);
        }


        public string Input
        {
            get => input;
            set => Set(ref input, value);
        }
        
        
        public User User
        {
            get => user;
            set => Set(ref user, value);
        }


        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();


        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();


        public async Task Connect()
        {
            try
            {
                if (Connected)
                    return;

                User = await messagingService.Connect(user.Name);
                await UpdateUserList();
                messagingService.SubscribeToTopics();

                Connected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect\n\n{ex.Message}");
            }
        }


        public async Task Disconnect()
        {
            try
            {
                if (!Connected)
                    return;

                User = await messagingService.Disconnect();
                messagingService.UnsubscribeFromTopics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to disconnect\n\n{ex.Message}");
            }
        }


        public async Task UpdateUserList()
        {
            try
            {
                var users = await messagingService.GetAllUsers();

                foreach (var user in users)
                    if (!Users.Any(u => u.Id == user.Id))
                        Users.Add(user);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve users list\n\n{ex.Message}");
            }
        }


        public async Task PostMessage()
        {
            try
            {
                await messagingService.PostMessage(new Message(User, Input, DateTime.UtcNow));
                Input = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to send message\n\n{ex.Message}");
            }
        }


        #region ViewModel stuffs

        private void Set<T>(ref T store, T value, [CallerMemberName] string propertyName = "")
        {
            store = value;
            RaisePropertyChanged(propertyName);
        }


        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
