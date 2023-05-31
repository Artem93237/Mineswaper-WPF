using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Minesweeper.ViewModels;

namespace Minesweeper.Views
{
    public class Field : UserControl
    {
        public Field()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void FieldPressed(object? sender, PointerPressedEventArgs e)
        {
            FieldViewModel fieldViewModel = (FieldViewModel)DataContext!;
            if (e.GetCurrentPoint(null).Properties.IsRightButtonPressed) fieldViewModel.FieldRightClicked();
            if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed) fieldViewModel.FieldLeftClicked();
        }
    }
}