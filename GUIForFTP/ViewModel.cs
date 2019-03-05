using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GUIForFTP
{
    /// <summary>
    /// Класс, соединяющий модель и представление
    /// </summary>
    class ViewModel : INotifyPropertyChanged
    {           
        /// <summary>
        /// Объект для уведомления изменения свойств объекта
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
        /// Уведомляет систему об изменении свойств.
        /// А система обновляет привязанные элементы
        /// </summary>
        /// <param name="changedProperty"></param>
        public void OnPropertyChanged([CallerMemberName]string changedProperty = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(changedProperty));
        }

        private string address;

        /// <summary>
        /// Адрес сервера
        /// </summary>
        public string Address
        {   get => address;
            set
            {
                address = value;               
            }
        }

        private string port;

        /// <summary>
        /// Порт сервера
        /// </summary>
        public string Port
        {
            get => port;
            set
            {
                port = value;                
            }
        }

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
                    
        public void Connect(string portFromThisViewModel, string addressFromThisViewModel)
        {
            DirectoriesAndFiles.Clear();
            clientIsModel = new ClientIsModel(portFromThisViewModel, addressFromThisViewModel, this);
            
            var tree = clientIsModel.OnConnectionShowDirectoriesTree(false, "");

            foreach (string dirThenFile in tree)
            {
                DirectoriesAndFiles.Add(dirThenFile);
            }
        }

        public void UpdateDirectoriesTree(string addDirectoryToServerPath)
        {
            var tree = clientIsModel.OnConnectionShowDirectoriesTree(true, addDirectoryToServerPath); // TODO!

            DirectoriesAndFiles.Clear();
            foreach (string dirThenFile in tree)
            {
                DirectoriesAndFiles.Add(dirThenFile);
            }            
        }

        public async void DownloadFile(string fileName)
        {
            await clientIsModel.DownloadFile(fileName);
        }

        public async void DownloadAllFiles()
        {
            await clientIsModel.DownloadAllFiles();
        }
    }
}