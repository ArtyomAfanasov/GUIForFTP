using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GUIForFTP
{
    /// <summary>
    /// Класс, соединяющий модель и представление
    /// </summary>
    class ViewModel
    {                   
        /// <summary>
        /// Объект Model
        /// </summary>
        private ClientIsModel clientIsModel;

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
                if (clientIsModel != null)
                {
                    clientIsModel.pathToSaveFileModel = pathToSaveFile;
                }                
            }
        }
                    
        public async void Connect(string portFromThisViewModel, string addressFromThisViewModel)
        {
            DirectoriesAndFiles.Clear();
            clientIsModel = new ClientIsModel(portFromThisViewModel, addressFromThisViewModel, this);
            
            var tree = await clientIsModel.ShowDirectoriesTree(false, "");

            foreach (string dirThenFile in tree)
            {
                DirectoriesAndFiles.Add(dirThenFile);
            }
        }

        public async void UpdateDirectoriesTree(string addDirectoryToServerPath)
        {
            var tree = await clientIsModel.ShowDirectoriesTree(true, addDirectoryToServerPath); // TODO!

            DirectoriesAndFiles.Clear();
            foreach (string dirThenFile in tree)
            {
                DirectoriesAndFiles.Add(dirThenFile);
            }            
        }

        public async void DownloadFile(string fileName)
        {
            try
            {
                await clientIsModel.DownloadFile(fileName);
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так");
            }            
        }

        public async Task DownloadAllFiles()
        {
            await clientIsModel.DownloadAllFiles();
        }
    }
}