﻿namespace GUIForFTP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.Port = "8888";
            viewModel.Address = "127.0.0.1";
        }

        /// <summary>
        /// Объект VM
        /// </summary>
        private ViewModel viewModel = new ViewModel();

        private void buttonConnect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.Connect(viewModel.Port, viewModel.Address);              
        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((ListBox)sender).SelectedItem.ToString() == "..")
            {
                viewModel.UpdateDirectoriesTree("..");
                return;
            }

            if ( viewModel.isDirectory[((ListBox)sender).SelectedIndex] )    
            {                
                viewModel.UpdateDirectoriesTree(((ListBox)sender).SelectedItem.ToString()); 
            }
            else
            {
                viewModel.DownloadFile(((ListBox)sender).SelectedItem.ToString());
            }                                      
        }

        private async void buttonDownloadAll_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await viewModel.DownloadAllFiles();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Для начала подключитесь к серверу");
            }
        }
    }
}
