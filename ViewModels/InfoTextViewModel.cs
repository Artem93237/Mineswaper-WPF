using Minesweeper.Models;

namespace Minesweeper.ViewModels
{
    public class InfoTextViewModel : ViewModelBase
    {
        public InfoTextViewModel(Global global)
        {
            Global = global;
        }
        public Global Global { get; }
    }
}