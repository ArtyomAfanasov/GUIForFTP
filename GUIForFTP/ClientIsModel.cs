namespace GUIForFTP
{
    using System;
    using System.Collections.Generic;
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
        private ObservableCollection<string> directoriesAndFiles;

        /// <summary>
        /// Коллекция флагов, определяющих директорию
        /// </summary>
        private List<bool> isDirectory;

        /// <summary>
        /// Директория, на которую "смотрит" сервер
        /// </summary>
        private string serverPath = "";        

        /// <summary>
        /// Получить путь, на который смотрит сервер
        /// </summary>
        private void GetServerPath()
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
        }

        // Мб это можно использовать и как обновление дерева
        // todo
        /// <summary>
        /// Получение дерева директорий
        /// </summary>
        public ObservableCollection<string> OnConnectionShowDirectoriesTree(bool isUpdateTree, string addDirectoryToServerPath)
        {
            GetServerPath();

            if (isUpdateTree)
            {
                serverPath += @"\" + addDirectoryToServerPath;
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
                    var stringDirsAndFiles = reader.ReadLine(); 

                    var splitDirsAndFiles = stringDirsAndFiles.Split(' ');
                    var dirsArray  = splitDirsAndFiles[0].Split('/');       
                    var filesArray = splitDirsAndFiles[1].Split('/');

                    directoriesAndFiles = new ObservableCollection<string>();
                    isDirectory = new List<bool>(); 
                    foreach (string element in dirsArray)
                    {
                        if (element != "")
                        {
                            directoriesAndFiles.Add(element);
                            isDirectory.Add(true);
                        }
                    }
                    foreach (string element in filesArray)
                    {
                        if (element != "")
                        {
                            directoriesAndFiles.Add(element);
                            isDirectory.Add(false);
                        }
                    }                    
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            viewModel.isDirectory = isDirectory;

            return directoriesAndFiles;
        }
        
        public void DownloadFile()
        {
            using (var client = new TcpClient(modelAddress, Convert.ToInt32(modelPort)))
            {
                try
                {
                    var stream = client.GetStream();
                    var writer = new StreamWriter(stream);
                    writer.WriteLine("Download");
                    writer.WriteLine(serverPath); // +  название файла ПРИПИЛИ
                    writer.Flush();
                }
                catch
                {

                }
            }
        }
    }
}