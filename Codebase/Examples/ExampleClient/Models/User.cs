using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExampleClient.Models
{
    public class User : INotifyPropertyChanged
    {
        private int id;
        private bool isConnected;
        private string name;
        private DateTime? connectedTimestamp;
        private DateTime? disconnectedTimestamp;


        public User(int id, string name, DateTime? connectedTimestamp, DateTime? disconnectedTimestamp)
        {
            Id = id;
            Name = name;
            ConnectedTimestamp = connectedTimestamp;
            DisconnectedTimestamp = disconnectedTimestamp;
        }


        public int Id
        {
            get => id;
            set => Set(ref id, value);
        }


        public bool IsConnected
        {
            get => isConnected;
            private set => Set(ref isConnected, value);
        }


        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }


        public DateTime? ConnectedTimestamp
        {
            get => connectedTimestamp;
            set
            {
                Set(ref connectedTimestamp, value);
                IsConnected = connectedTimestamp.HasValue && !disconnectedTimestamp.HasValue;
            }
        }


        public DateTime? DisconnectedTimestamp
        {
            get => disconnectedTimestamp;
            set
            {
                Set(ref disconnectedTimestamp, value);
                IsConnected = connectedTimestamp.HasValue && !disconnectedTimestamp.HasValue;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public ExampleContracts.Models.User ToDTO() =>
            new ExampleContracts.Models.User(Id, Name, ConnectedTimestamp, DisconnectedTimestamp);


        public static User FromDTO(ExampleContracts.Models.User user) =>
            new User(user.Id, user.Name, user.ConnectedTimestamp, user.DisconnectedTimestamp);


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


        #region Comparer

        public class Comparer : IEqualityComparer<User>
        {
            public static Comparer Default { get; } = new Comparer();


            public bool Equals(User x, User y) =>
                x.Id == y.Id;


            public int GetHashCode(User obj) =>
                obj.Id;
        }

        #endregion
    }
}