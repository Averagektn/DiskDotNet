namespace Disk.ViewModel.Common
{
    public class PopupViewModel : ObserverViewModel
    {
        private string _popupMessage = string.Empty;
        public string PopupMessage { get => _popupMessage; set => SetProperty(ref _popupMessage, value); }

        private bool _isShowPopup;
        public bool IsShowPopup { get => _isShowPopup; set => SetProperty(ref _isShowPopup, value); }

        public async Task ShowPopup(string message)
        {
            PopupMessage = message;
            IsShowPopup = true;
            await Task.Delay(TimeSpan.FromSeconds(3));
            IsShowPopup = false;
        }
    }
}
