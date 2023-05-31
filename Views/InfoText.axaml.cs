using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Minesweeper.Views
{
    public class InfoText : UserControl
    {
        public InfoText()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}