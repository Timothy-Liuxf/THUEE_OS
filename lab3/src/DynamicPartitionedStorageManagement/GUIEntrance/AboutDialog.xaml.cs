using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace GUIEntrance
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var href = sender as Hyperlink;
                if (href is not null)
                {
                    Process.Start(new ProcessStartInfo(href.NavigateUri.AbsoluteUri) { UseShellExecute = true });
                    e.Handled = true;
                }
            }
            catch
            {
                // Do nothing
            }
        }
    }
}
