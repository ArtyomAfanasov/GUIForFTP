namespace GUIForFTP
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Windows;

    class ClientIsModel
    {
        /// <summary>
        /// Порт сервера
        /// </summary>
        public readonly string modelPort;

        /// <summary>
        /// Адрес сервера
        /// </summary>
        public readonly string modelAddress;

        private readonly ViewModel viewModel;

        public ClientIsModel(string portFromVM, string addressFromVM, ViewModel viewModel)
        {
            modelPort = portFromVM;
            modelAddress = addressFromVM;
            this.viewModel = viewModel;
        }

        /// <summary>
        /// Коллекция файлов и папок для передачи VM
        /// </summary>
        public ObservableCollection<string> directoriesAndFiles = new ObservableCollection<string>();

        /// <summary>
        /// Директория, на которую "смотрит" сервер
        /// </summary>
        private string serverPath = "";        

        // todo
        /// <summary>
        /// Получение дерева директорий
        /// </summary>
        public void OnConnectionShowDirectoriesTree()
        {
            if (serverPath == "")
            {
                using (var client = new TcpClient(modelAddress, Convert.ToInt32(modelPort)))
                {
                    try
                    {
                        var stream = client.GetStream();
                        var writer = new StreamWriter(stream);
                        writer.WriteLine("path");
                        writer.WriteLine("giveMePath");
                        writer.Flush();

                        var reader = new StreamReader(stream);  // получили путь, если его не было
                        serverPath = reader.ReadLine();

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }

            using (var client = new TcpClient(modelAddress, Convert.ToInt32(modelPort)))
            {                
                try
                {
                    var stream = client.GetStream();
                    var writer = new StreamWriter(stream);
                    writer.WriteLine("Listing");
                    writer.WriteLine(serverPath);
                    writer.Flush();                    

                    var reader = new StreamReader(stream);
                    var stringDirsAndFiles = reader.ReadLine(); // добавляьб в obvser коллекцию элементы 

                    var splitDirsAndFiles = stringDirsAndFiles.Split(' ');
                    var dirsArray  = splitDirsAndFiles[0].Split('/');       // ??? Слеш
                    var filesArray = splitDirsAndFiles[1].Split('/');      // ??? Слеш

                    foreach (string element in dirsArray)
                    {
                        directoriesAndFiles.Add(element);
                    }
                    foreach (string element in filesArray)
                    {
                        directoriesAndFiles.Add(element);
                    }

                    viewModel.DirectoriesAndFiles = directoriesAndFiles;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }                                              
    }
}