namespace GUIForFTP
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows;

    /// <summary>
    /// Класс, соединяющий модель и представление
    /// </summary>
    class ViewModel
    {                  
        public ViewModel()
        {
            Active = new ObservableCollection<string>();
        }

        /// <summary>
        /// Объект Model
        /// </summary>
        private ClientModel clientModel;

        /// <summary>
        /// Коллекция директорий и папок для изменения View
        /// </summary>                      
        public ObservableCollection<string> DirectoriesAndFiles { get; set; } 
            = new ObservableCollection<string>();

        /// <summary>
        /// Коллекция скачиваемых файлов
        /// </summary>
        public ObservableCollection<string> DownloadingFiles { get; set; }
            = new ObservableCollection<string>();

        /// <summary>
        /// Скачанные файлы
        /// </summary>
        public ObservableCollection<string> DownloadedFiles { get; set; }
            = new ObservableCollection<string>();

        /// <summary>
        /// Активность пользователя.
        /// </summary>
        private ObservableCollection<string> active;

        /// <summary>
        /// Активность пользователя.
        /// </summary>
        public ObservableCollection<string> Active
        {
            get => active;
            set
            {                
                active = value;
            }
        }            

        /// <summary>
        /// Коллекция флагов, определяющих директорию
        /// </summary>
        public List<bool> isDirectory = new List<bool>();        
       
        /// <summary>
        /// Адрес сервера
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Порт сервера
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// Путь для сохранения файлов
        /// </summary>
        private string pathToSaveFile;

        /// <summary>
        /// Путь для сохранения файлов. 
        /// Работа с полем ввода пути для сохранения файлов.
        /// </summary>
        public string PathToSaveFile
        {
            get => pathToSaveFile;
            set
            {
                if (value != "")
                {                                        
                    if (!Directory.Exists(value))
                    {
                        MessageBox.Show("Каталога, который Вы хотите задать как папку загрузок не существует.");
                        return;
                    }

                    if (clientModel == null)
                    {
                        MessageBox.Show("Перед выбором папки для загрузок необходимо подключиться к серверу");
                        return;
                    }

                    pathToSaveFile = value;
                    clientModel.pathToSaveFileModel = pathToSaveFile;                    
                }                                                                 
            }
        }
                    
        /// <summary>
        /// Подключиться к серверу
        /// </summary>
        /// <param name="portFromThisViewModel">Порт, полученный от Vm</param>
        /// <param name="addressFromThisViewModel">Адресс, полученный от VM</param>
        public async void Connect(string portFromThisViewModel, string addressFromThisViewModel)
        {
            Active.Clear();           
            Active.Add("Ваша активность:");
            
            clientModel = new ClientModel(portFromThisViewModel, addressFromThisViewModel, this);

            await clientModel.ConnectToServerFirstTime();
            await clientModel.GetServerPathOnConnectionToServer();
            await clientModel.ShowDirectoriesTree(false, "");                  
        }

        /// <summary>
        /// Сообщить модели об изменении представления катологов
        /// </summary>
        /// <param name="addDirectoryToServerPath">Имя папки, выбранной пользователем</param>
        public async void UpdateDirectoriesTree(string addDirectoryToServerPath)
        {
            await clientModel.ShowDirectoriesTree(true, addDirectoryToServerPath);                 
        }

        /// <summary>
        /// Передать модели информации о необходимости скачать файл
        /// </summary>
        /// <param name="fileName">Имя файла, выбранного пользователем</param>
        public async void DownloadFile(string fileName)
        {
            // в случае, если путь для загрузок выбран корректный, но до подключения к серверу
            // И чтобы после этого и после подлкючения к серверу можно было качать файлы в уже выбранный корректный путь 
            if (((MainWindow)Application.Current.MainWindow).textBoxSavePath.Text != "")
            {
                PathToSaveFile = ((MainWindow)Application.Current.MainWindow).textBoxSavePath.Text;
            }
            
            await clientModel.DownloadFile(fileName, false);    // ?????? todo обработка ошибок                                
        }

        /// <summary>
        /// Сообщить модели скачать все файлы
        /// </summary>        
        public void DownloadAllFiles()
        {
            if (clientModel != null)
            {
                clientModel.DownloadAllFiles();
            }
            else
            {
                MessageBox.Show("Чтобы скачать все файлы в папке сначала необходимо подключится к серверу");
            }                                                               
        }
    }
}