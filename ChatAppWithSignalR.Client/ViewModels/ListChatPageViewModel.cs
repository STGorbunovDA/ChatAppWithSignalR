using ChatAppWithSignalR.Client.Models;
using ChatAppWithSignalR.Client.Services;
using ChatAppWithSignalR.Client.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ChatAppWithSignalR.Client.ViewModels
{
    public class ListChatPageViewModel : ViewModelBase, IQueryAttributable
    {
        private User _userInfo;
        private ObservableCollection<User> _userFriends;
        private ObservableCollection<LastestMessage> _lastestMessages;
        private bool _isRefreshing;
        private ServiceProviderInstance _serviceProvider;
        //private ChatHub _chatHub;

        public User UserInfo
        {
            get { return _userInfo; }
            set { _userInfo = value; OnPropertyChanged(); }
        }
        public ObservableCollection<User> UserFriends
        {
            get { return _userFriends; }
            set { _userFriends = value; OnPropertyChanged(); }
        }

        public ObservableCollection<LastestMessage> LastestMessages
        {
            get { return _lastestMessages; }
            set { _lastestMessages = value; OnPropertyChanged(); }
        }

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set { _isRefreshing = value; OnPropertyChanged(); }
        }

        public ICommand RefreshCommand { get; set; }

        public ICommand OpenChatPageCommand { get; set; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {

        }
    }
}
