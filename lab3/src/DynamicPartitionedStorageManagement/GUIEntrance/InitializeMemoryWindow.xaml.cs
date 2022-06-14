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
using System.Windows.Shapes;
using MemoryManager;

namespace GUIEntrance
{
    /// <summary>
    /// Interaction logic for InitializeMemory.xaml
    /// </summary>
    public partial class InitializeMemoryWindow : Window
    {
        private nuint startAddress = 0u;
        private int size = 0;
        public MemoryManager.MemoryManager? MemoryManager { get; set; }

        public InitializeMemoryWindow()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ = sender;
                _ = e;

                var startAddressStr = startAddressInput.Text;
                var sizeToAllocStr = sizeToAllocInput.Text;

                try
                {
                    startAddress = nuint.Parse(startAddressStr);
                    size = int.Parse(sizeToAllocStr);
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

                try
                {
                    this.MemoryManager = MemoryManagerFactory.CreateMemoryManager(startAddress, size,
                        (MemoryManagerFactory.AllocationStrategy)
                        Enum.Parse
                        (
                            typeof(MemoryManagerFactory.AllocationStrategy),
                            (strategySelect.SelectedItem as ComboBoxItem)?.Content.ToString() ?? ""
                        )
                    );
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + '\n' + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
    }
}
