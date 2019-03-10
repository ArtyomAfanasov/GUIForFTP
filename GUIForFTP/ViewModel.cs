namespace GUIForFTP
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Windows;

    /// <summary>
    /// Класс, соединяющий модель и представление
    /// </summary>
    class ViewModel
    {                                  
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
        public ObservableCollection<string> Active { get; set; }
            = new ObservableCollection<string>();

        /// <summary>
        /// Коллекция флагов, определяющих директорию.
        /// </summary>
        public List<bool> isDirectory = new List<bool>();

        /// <summary>
        /// Для вывода диалогового окна с расположением файла.
        /// </summary>
        private OpenFileDialog openDialog = new OpenFileDialog();

        /// <summary>
        /// Паттерн расширений для регулярного выражения
        /// </summary>
        private string patternExt = @".dll$|.zip$|.exe$|.rar$|.jpg$|.jpeg$|.png$|.torrent$|.vsix$|.mkv$|.avi$|.iso$|.bin$|.djvu$";

        /// <summary>
        /// Путь выбранного файла в диалоговом окне расположения скачанных файлов
        /// </summary>
        private string chosenFilePath;

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

        /// <summary>
        /// Открыть диалоговое окно с расположением файла <see cref="ChosenFileName"/>.
        /// </summary>
        /// <param name="ChosenFileName"></param>
        public void OpenDownloadedFileLocation(string ChosenFileName)
        {
            var pathToChosenFile = clientModel.pathToSaveFileModel + @"\" + ChosenFileName;
            
            openDialog.InitialDirectory = pathToChosenFile;                                  

            do
            {
                openDialog.ShowDialog();            
                chosenFilePath = openDialog.FileName;
                
                if (chosenFilePath == "") // юзер нажал "отмена" или "крестик"
                {
                    break;
                }          
                
                try
                {
                    if (!Regex.IsMatch(chosenFilePath, patternExt)) // если файл имеет расширение, не указанное в паттерне
                    {
                        Process.Start("notepad.exe", chosenFilePath);
                    }
                    else
                    {
                        MessageBox.Show("Notepad не может открыть файлы с расширением " +
                        ".dll, .zip, .exe, .rar, .jpg, .jpeg, .torrent, .vsix, .png, .mkv, .avi, .iso, .bin, djvu");
                        openDialog.FileName = ""; // чтобы избежать зацикливания, если пользователь хочет отменить выбор
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show($"Не удалось открыть файл по пути: {chosenFilePath}");
                }
            }
            while (Regex.IsMatch(chosenFilePath, patternExt)); // пока выбранный файл имеет расширение, указанное в паттерне.
        }              
    }
}