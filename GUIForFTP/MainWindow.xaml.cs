namespace GUIForFTP
{
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
                viewModel.DownloadFile(((ListBox)sender).SelectedItem.ToString());
            }                                      
        }

        private void buttonDownloadAll_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {            
            viewModel.DownloadAllFiles();          
        }

        private void clearActivity_MouseUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.Active.Clear();
            viewModel.Active.Add("Ваша активность:");
        }        

        private void DownloadedFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((ListBox)sender).SelectedItem == null) // если выбрана рамка ListBox'a
            {
                return;
            }

            viewModel.OpenDownloadedFileLocation(((ListBox)sender).SelectedItem.ToString());
        }        
    }
}