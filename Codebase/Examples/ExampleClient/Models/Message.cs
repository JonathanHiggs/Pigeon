using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExampleClient.Models
{
    public class Message : INotifyPropertyChanged
    {
        private User user;
        private string content;
        private DateTime timestamp;


        public Message(User user, string content, DateTime timestamp)
        {
            User = user;
            Content = content;
            Timestamp = timestamp;
        }


        public User User
        {
            get => user;
            set => Set(ref user, value);
        }


        public string Content
        {
            get => content;
            set => Set(ref content, value);
        }


        public DateTime Timestamp
        {
            get => timestamp;
            set => Set(ref timestamp, value);
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public ExampleContracts.Models.Message ToDTO() =>
            new ExampleContracts.Models.Message(User.ToDTO(), Content, Timestamp);


        public static Message FromDTO(ExampleContracts.Models.Message message) =>
            new Message(User.FromDTO(message.User), message.Content, message.Timestamp);
                

        #region Model stuffs

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