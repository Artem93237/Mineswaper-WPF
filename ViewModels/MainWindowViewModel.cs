namespace Minesweeper.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(InfoTextViewModel infoTextViewModel, ControlsViewModel controlsViewModel,
            BoardViewModel boardViewModel)
        {
            InfoTextViewModel = infoTextViewModel;
            ControlsViewModel = controlsViewModel;
            BoardViewModel = boardViewModel;
        }

        public BoardViewModel BoardViewModel { get; set; }
        public InfoTextViewModel InfoTextViewModel { get; set; }
        public ControlsViewModel ControlsViewModel { get; set; }
    }
}