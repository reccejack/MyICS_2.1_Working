using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MyICSv2._1.Models;
using MyICSv2._1.ViewModels;

namespace MyICSv2._1.Views
{
    public partial class AddProcessWindow : Window
    {
        public AddProcessWindow()
        {
            InitializeComponent();
        }

        private void OnClick_Save(object? sender, RoutedEventArgs e)
        {
            var processId = ProcessIdField.Text?.Trim();

            if (string.IsNullOrWhiteSpace(processId))
            {
                // simple guard � you could show a validation message instead
                return;
            }

            // Return the new Process as the dialog result
            Close(new IndustryProcess(processId.Trim()));
        }

        private void OnClick_Cancel(object? sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}