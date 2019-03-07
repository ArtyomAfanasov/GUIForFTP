namespace GUIForFTP
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Net.Sockets;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
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

        private string pathToSaveFile;

        /// <summary>
        /// Путь для сохранения файлов
        /// </summary>
        public string PathToSaveFile
        {
            get => pathToSaveFile;
            set
            {
                if (value != "")
                {                    
                    /*var pathToSave = value;
                    var intexLastSlash = pathToSave.LastIndexOf("\\");
                    if (intexLastSlash == -1)
                    {
                        MessageBox.Show("Каталога, в котором вы хотите создать папку для загрузок не существует.");
                        return;
                    }
                    var pathWithoutFolderName = pathToSave.Remove(intexLastSlash, pathToSave.Length - intexLastSlash);
                    */

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
            DirectoriesAndFiles.Clear();
            clientModel = new ClientModel(portFromThisViewModel, addressFromThisViewModel, this);

            //try
            //{
                var tree = await clientModel.ShowDirectoriesTree(false, "");
                foreach (string dirThenFile in tree)
                {
                    DirectoriesAndFiles.Add(dirThenFile);
                }
            //}
            //catch (Exception exception)
            //{
            //    MessageBox.Show(exception.Message); // ????????????? todo
            //}
        }

        /// <summary>
        /// Сообщить модели об изменении представления катологов
        /// </summary>
        /// <param name="addDirectoryToServerPath"></param>
        public async void UpdateDirectoriesTree(string addDirectoryToServerPath)
        {
            var tree = await clientModel.ShowDirectoriesTree(true, addDirectoryToServerPath); // TODO!

            DirectoriesAndFiles.Clear();
            foreach (string dirThenFile in tree)
            {
                DirectoriesAndFiles.Add(dirThenFile);
            }            
        }

        /// <summary>
        /// Передать модели информации о необходимости скачать файл
        /// </summary>
        /// <param name="fileName"></param>
        public async void DownloadFile(string fileName)
        {
            // в случае, если путь для загрузок выбран корректный, но до подключения к серверу
            // И чтобы после этого и после подлкючения к серверу можно было качать файлы в уже выбранный корректный путь 
            if (((MainWindow)Application.Current.MainWindow).textBoxSavePath.Text != "")
            {
                PathToSaveFile = ((MainWindow)Application.Current.MainWindow).textBoxSavePath.Text;
            }

            try
            {
                await clientModel.DownloadFile(fileName);
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так");         // ????????????? todo        
            }            
        }

        /// <summary>
        /// Сообщить модели скачать все файлы
        /// </summary>
        /// <returns></returns>
        public void DownloadAllFiles()
        {
            try
            {
                clientModel.DownloadAllFiles();
            }           
            catch (NullReferenceException)
            {    
            MessageBox.Show("Чтобы скачать все файлы в папке сначала необходимо подключится к серверу");
            }
        }
    }
}