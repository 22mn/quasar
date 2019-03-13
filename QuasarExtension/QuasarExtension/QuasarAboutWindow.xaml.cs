using System;
using System.Windows;
using System.Windows.Navigation;
using System.Diagnostics;
namespace QuasarExtension
{
    /// <summary>
    /// Interaction logic for QuasarAboutWindow.xaml
    /// </summary>
    public partial class QuasarAboutWindow : Window
    {
        public QuasarAboutWindow()
        {
            InitializeComponent();
        }

        public void NavigateURL(object sender, RequestNavigateEventArgs args)
        {
            Process.Start(new ProcessStartInfo(args.Uri.AbsoluteUri));
            args.Handled = true;
        }
    }
}
