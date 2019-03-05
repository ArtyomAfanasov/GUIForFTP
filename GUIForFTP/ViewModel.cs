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
            = new ObservableCollection<string>() { "/" };

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
            
        // todo
        public void Connect(string portFromThisViewModel, string addressFromThisViewModel)
        {
            clientIsModel = new ClientIsModel(portFromThisViewModel, addressFromThisViewModel, this);
            
            var tree = clientIsModel.OnConnectionShowDirectoriesTree();

            foreach (string dirOrFile in tree)
            {
                DirectoriesAndFiles.Add(dirOrFile);
            }
        }

        public void UpdateDirectoriesTree()
        {

        }

    }
}