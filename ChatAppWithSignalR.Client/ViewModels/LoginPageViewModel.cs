using ChatAppWithSignalR.Client.Services;
using ChatAppWithSignalR.Client.ViewModels.Base;
using ChatAppWithSignalR.Shared;
using System.Windows.Input;

namespace ChatAppWithSignalR.Client.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private string userName;
        private string password;
        private bool isProcessing;

        public string UserName
        {
            get { return userName; }
            set { userName = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(); }
        }

        public bool IsProcessing
        {
            get { return isProcessing; }
            set { isProcessing = value; OnPropertyChanged(); }
        }

        private ServiceProviderInstance _serviceProvider;

        public ICommand LoginCommand { get; set; }

        public LoginPageViewModel(ServiceProviderInstance serviceProvider)
        {
            UserName = "wanda";
            Password = "Abc12345";
            IsProcessing = false;
            _serviceProvider = serviceProvider;

            LoginCommand = new ViewModelCommand(ExecuteIsProcessingCommand);
           
        }

        private void ExecuteIsProcessingCommand(object obj)
        {
            if (IsProcessing) return;

            if (UserName.Trim() == "" || Password.Trim() == "") return;

            IsProcessing = true;

            Login().GetAwaiter().OnCompleted(() =>
            {
                IsProcessing = false;
            });
        }

        async Task Login()
        {
            try
            {
                var request = new AuthenticateRequest
                {
                    LoginId = UserName,
                    Password = Password,
                };

                var response = await _serviceProvider.Authenticate(request);

                if (response.StatusCode == 200)
                {
                    await Shell.Current.GoToAsync($"ListChatPage?userId={response.Id}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("ChatApp", response.StatusMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("ChatApp", ex.Message, "OK");
            }
        }
    }
}
