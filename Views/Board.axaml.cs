using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Minesweeper.Views
{
    public class Board : UserControl
    {
        public Board()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}