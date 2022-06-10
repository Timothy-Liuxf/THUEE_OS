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
using System.ComponentModel;
using System.Collections.ObjectModel;
using MemoryManager;

namespace GUIEntrance
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public string StartAddressInput
        {
            get => startAddressInput;
            set
            {
                startAddressInput = value;
                RaisePropertyChanged(nameof(StartAddressInput));
            }
        }
        private string startAddressInput = "";

        public string SizeToAllocInput
        {
            get => sizeToAllocInput;
            set
            {
                sizeToAllocInput = value;
                RaisePropertyChanged(nameof(SizeToAllocInput));
            }
        }
        private string sizeToAllocInput = "";

        public event PropertyChangedEventHandler? PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand AllocMemory { get; init; }
        public ICommand FreeMemory { get; init; }

        private MemoryManager.MemoryManager memoryManager;

        //public double MemoryDiaplayerWidth { get; set; }
        //public double MemoryDiaplayerHeight { get; set; }
        public ObservableCollection<FrameworkElement> MemoryDisplayerItems { get; } = new();

        private readonly Brush rectangleBorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        private const double rectangleBorderThickness = 1.0;
        private readonly Brush displayerBorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 200));
        public void RePaintMemoryDisplayer()
        {
            nuint startAddress = memoryManager.StartAddress;
            int memorySize = memoryManager.MemorySize;

            if (memorySize == 0)
            {
                return;
            }
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow is null || mainWindow.MemoryDisplayer is null)
            {
                return;
            }
            double width = mainWindow.MemoryDisplayer.ActualWidth;
            double height = mainWindow.MemoryDisplayer.ActualHeight;

            double rate = width / memorySize;
            var allocatedList = memoryManager.GetAllocatedMemories();
            var rectList = new LinkedList<Rectangle>();

            nuint lastPaintAddress = startAddress;
            foreach (var allocatedBlock in allocatedList)
            {
                if (allocatedBlock.Memory != lastPaintAddress)
                {
                    int freeSize = (int)(allocatedBlock.Memory - lastPaintAddress);
                    var newRect = new Rectangle() { Width = freeSize * rate, Height = height, StrokeThickness = rectangleBorderThickness, Stroke = rectangleBorderBrush };
                    newRect.SetValue(Canvas.TopProperty, 0.0);
                    newRect.SetValue(Canvas.LeftProperty, (lastPaintAddress - startAddress) * rate);
                    rectList.AddLast(newRect);
                }

                {
                    var newRect = new Rectangle() { Width = allocatedBlock.Size * rate, Height = height, Fill = new SolidColorBrush(Color.FromRgb(0, 162, 232)), StrokeThickness = rectangleBorderThickness, Stroke = rectangleBorderBrush };
                    newRect.SetValue(Canvas.TopProperty, 0.0);
                    newRect.SetValue(Canvas.LeftProperty, (allocatedBlock.Memory - startAddress) * rate);
                    rectList.AddLast(newRect);
                    lastPaintAddress = allocatedBlock.Memory + (nuint)allocatedBlock.Size;
                }
            }
            if (lastPaintAddress != startAddress + (nuint)memorySize)
            {
                int leftSize = (int)((startAddress + (nuint)memorySize) - lastPaintAddress);
                var newRect = new Rectangle() { Width = leftSize * rate, Height = height, StrokeThickness = rectangleBorderThickness, Stroke = rectangleBorderBrush };
                newRect.SetValue(Canvas.TopProperty, 0.0);
                newRect.SetValue(Canvas.LeftProperty, (lastPaintAddress - startAddress) * rate);
                rectList.AddLast(newRect);
            }

            MemoryDisplayerItems.Clear();
            // MemoryDisplayerItems.Add(new Rectangle() { Width = width, Height = height, Fill = new SolidColorBrush(Color.FromRgb(0, 0, 232)) });
            foreach (var rect in rectList)
            {
                MemoryDisplayerItems.Add(rect);
            }
            MemoryDisplayerItems.Add(new Border() { Width = width, Height = height, BorderThickness = new Thickness(2.0), BorderBrush = displayerBorderBrush });
            RaisePropertyChanged(nameof(MemoryDisplayerItems));
        }

        private void AllocMemoryAction()
        {
            try
            {
                int size = 0;
                try
                {
                    size = int.Parse(SizeToAllocInput);
                }
                catch (FormatException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                catch (OverflowException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var addr = memoryManager.AllocateMemory(size);
                if (addr is not null)
                {
                    var info = string.Format($"Alloc success! Start address: {addr.Value}; Size: { size }");
                    LogInfo = info;
                    RePaintMemoryDisplayer();
                }
                else
                {
                    MessageBox.Show("Fail to alloc memory!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + '\n' + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void FreeMemoryAction()
        {
            try
            {
                nuint startAddress = 0;
                try
                {
                    startAddress = nuint.Parse(StartAddressInput);
                }
                catch (FormatException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                catch (OverflowException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (memoryManager.FreeMemory(startAddress))
                {
                    var info = string.Format($"Successfully free memory at: { startAddress }!");
                    LogInfo = info;
                    RePaintMemoryDisplayer();
                }
                else
                {
                    MessageBox.Show("Fail to free memory!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + '\n' + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        public string LogInfo
        {
            get => string.Join('\n', logs);
            private set
            {
                if (logs.Count == MAX_LOGS_LINES)
                {
                    logs.RemoveFirst();
                }
                logs.AddLast(value);
                RaisePropertyChanged(nameof(LogInfo));
            }
        }
        private const int MAX_LOGS_LINES = 1024;
        private LinkedList<string> logs = new();

        public MainWindowViewModel()
        {
            AllocMemory = new RelayCommand(AllocMemoryAction);
            FreeMemory = new RelayCommand(FreeMemoryAction);

            var initialization = new InitializeMemoryWindow();
            initialization.ShowDialog();
            if (initialization.MemoryManager is null)
            {
                Application.Current.Shutdown();

                // Unreachable, just to disable warnings.

                throw new Exception("Unreachable code!");
            }
            memoryManager = initialization.MemoryManager;
            LogInfo = String.Format($"Successfully created memory manager!\n- Start address: {memoryManager.StartAddress}\n- Size: {memoryManager.MemorySize}");
            RePaintMemoryDisplayer();
        }
    }
}
