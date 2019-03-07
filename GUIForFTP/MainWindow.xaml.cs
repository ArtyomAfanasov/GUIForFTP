namespace GUIForFTP
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

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
                // в случае, если путь для загрузок выбран корректный, но до подключения к серверу
                // И чтобы после этого и после подлкючения к серверу можно было качать файлы в уже выбранный корректный путь 
                if (((MainWindow)Application.Current.MainWindow).textBoxSavePath.Text != "")
                {
                    viewModel.PathToSaveFile = ((MainWindow)Application.Current.MainWindow).textBoxSavePath.Text;
                }

                viewModel.DownloadFile(((ListBox)sender).SelectedItem.ToString());
            }                                      
        }

        private async void buttonDownloadAll_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //try
            //{

            // в случае, если путь для загрузок выбран корректный, но до подключения к серверу
            // И чтобы после этого и после подлкючения к серверу можно было качать файлы в уже выбранный корректный путь 
            //if (((MainWindow)Application.Current.MainWindow).textBoxSavePath.Text != "")
            //{
            //    viewModel.PathToSaveFile = ((MainWindow)Application.Current.MainWindow).textBoxSavePath.Text;
            //}

            await viewModel.DownloadAllFiles();

            //}
            //catch (NullReferenceException)
            //{
                //MessageBox.Show("Чтобы скачать все файлы в папке сначала необходимо подключится к серверу");
            //}
        }
    }
}
