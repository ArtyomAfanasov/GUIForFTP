﻿namespace GUIForFTP
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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
                pathToSaveFile = value;
                if (clientModel != null)
                {
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
            
            var tree = await clientModel.ShowDirectoriesTree(false, "");

            foreach (string dirThenFile in tree)
            {
                DirectoriesAndFiles.Add(dirThenFile);
            }
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
            try
            {
                await clientModel.DownloadFile(fileName);
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так");                
            }            
        }

        /// <summary>
        /// Сообщить модели скачать все файлы
        /// </summary>
        /// <returns></returns>
        public async Task DownloadAllFiles()
        {
            await clientModel.DownloadAllFiles();
        }
    }
}