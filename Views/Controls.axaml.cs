using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Minesweeper.Views
{
    public class Controls : UserControl
    {
        public Controls()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}