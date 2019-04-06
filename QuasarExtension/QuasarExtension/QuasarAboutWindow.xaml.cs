using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
        public void MyNameMouseEnter(object sender,MouseEventArgs e)
        {
            myname.Foreground = Brushes.LightSkyBlue;
        }

        private void HyperlinkMouseEnter(object sender, MouseEventArgs e)
        {
            link1.Foreground = Brushes.LightSkyBlue;
        }

        public void MyNameMouseLeave(object sender, MouseEventArgs e)
        {
            myname.Foreground = Brushes.DeepSkyBlue;
        }

        private void HyperlinkMouseLeave(object sender, MouseEventArgs e)
        {
            link1.Foreground = Brushes.DeepSkyBlue;
        }
    }
}
