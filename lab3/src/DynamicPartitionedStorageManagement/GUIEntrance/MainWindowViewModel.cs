////////////////////////////////////////////////////////////////////////////////
//
// This file is part of the THUEE_OS project.
//
// Copyright (C) 2022 Timothy-LiuXuefeng
//
// MIT License
//

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

        public MemoryManager.MemoryManager? MemoryManager
        {
            get => memoryManager;
            set
            {
                if (value is null)
                {
                    return;
                }
                memoryManager = value;
                LogInfo = String.Format($"\nSuccessfully created memory manager!\n- Start address: {value.StartAddress}\n- Size: {value.MemorySize}\n");
                RePaintMemoryDisplayer();
            }
        }
        private MemoryManager.MemoryManager? memoryManager = null;

        //public double MemoryDiaplayerWidth { get; set; }
        //public double MemoryDiaplayerHeight { get; set; }
        public ObservableCollection<FrameworkElement> MemoryDisplayerItems { get; } = new();

        private readonly Brush rectangleBorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        private const double rectangleBorderThickness = 1.0;
        private readonly Brush displayerBorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 200));
        public double ActHeight { get; set; }
        public void RePaintMemoryDisplayer()
        {
            if (MemoryManager is null)
            {
                return;
            }

            nuint startAddress = MemoryManager.StartAddress;
            int memorySize = MemoryManager.MemorySize;

            if (memorySize == 0)
            {
                return;
            }
            // var mainWindow = Application.Current.MainWindow as MainWindow;
            var mainWindow = ParentWindow as MainWindow;
            if (mainWindow is null || mainWindow.MemoryDisplayer is null)
            {
                return;
            }
            double width = mainWindow.MemoryDisplayer.ActualWidth;
            double height = mainWindow.MemoryDisplayer.ActualHeight;

            double rate = width / memorySize;
            var allocatedList = MemoryManager.GetAllocatedMemories();
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
                if (MemoryManager is null)
                {
                    throw new Exception("Unexpected exception!");
                }
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

                var addr = MemoryManager.AllocateMemory(size);
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
                if (MemoryManager is null)
                {
                    throw new Exception("Unexpected exception!");
                }
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

                if (MemoryManager.FreeMemory(startAddress))
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
        public Window? ParentWindow = null;

        public MainWindowViewModel()
        {
            AllocMemory = new RelayCommand(AllocMemoryAction);
            FreeMemory = new RelayCommand(FreeMemoryAction);
        }
    }
}
