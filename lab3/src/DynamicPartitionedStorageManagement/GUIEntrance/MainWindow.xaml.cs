using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUIEntrance
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var dataContext = DataContext as MainWindowViewModel;
            if (dataContext is null)
            {
                return;
            }
            dataContext.ParentWindow = this;

            if (!CreateNewMemoryManager())
            {
                Application.Current.Shutdown();
            }
        }

        public ItemsControl MemoryDisplayer => memoryDisplayer;

        private bool CreateNewMemoryManager()
        {
            var dataContext = DataContext as MainWindowViewModel;
            if (dataContext is null)
            {
                return false;
            }

            var initialization = new InitializeMemoryWindow();
            initialization.ShowDialog();
            if (initialization.MemoryManager is null)
            {
                return false;
            }
            dataContext.MemoryManager = initialization.MemoryManager;
            return true;
        }

        private void logTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ = sender;
            _ = e;
            logTextBox.ScrollToEnd();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var dataContext = DataContext as MainWindowViewModel;
            if (dataContext == null)
            {
                return;
            }
            dataContext.RePaintMemoryDisplayer();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            CreateNewMemoryManager();
        }
    }
}
