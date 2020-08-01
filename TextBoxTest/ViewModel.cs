using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace TextBoxTest
{
    class ViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SetTextCommand { get; } = new RelayCommand(parameter => SetText(parameter, "abc"));
        public ICommand ClearTextCommand { get; } = new RelayCommand(parameter => SetText(parameter, string.Empty));

        private static readonly PaletteHelper _paletteHelper = new PaletteHelper();

        public ViewModel()
        {
            _isDarkTheme = _paletteHelper.GetTheme().GetBaseTheme() == BaseTheme.Dark;
        }

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (SetValue(ref _isDarkTheme, value))
                {
                    var theme = _paletteHelper.GetTheme();
                    theme.SetBaseTheme(value ? Theme.Dark : Theme.Light);
                    _paletteHelper.SetTheme(theme);
                }
            }
        }
        private bool _isDarkTheme;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetValue(ref _isEnabled, value);
        }
        private bool _isEnabled = true;

        private bool SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        private static void SetText(object parameter, string text)
        {
            foreach (var obj in TraverseVisualTree(parameter as DependencyObject))
            {
                switch (obj)
                {
                case TextBox textBox:
                    textBox.Text = text;
                    break;

                case PasswordBox passwordBox:
                    passwordBox.Password = text;
                    break;
                }
            }
        }

        private static IEnumerable<DependencyObject> TraverseVisualTree(DependencyObject parent)
        {
            if (parent is null)
                yield break;
            yield return parent;
            for (int i = 0, n = VisualTreeHelper.GetChildrenCount(parent); i < n; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                foreach (var obj in TraverseVisualTree(child))
                    yield return obj;
            }
        }

        class RelayCommand: ICommand
        {
#pragma warning disable CS0067
            public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067
            private readonly Action <object> _execute;
            public RelayCommand(Action <object> execute) => _execute = execute;
            public bool CanExecute(object parameter) => true;
            public void Execute(object parameter) => _execute.Invoke(parameter);
        }
    }
}
