namespace GUIForFTP
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
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
        private ObservableCollection<string> directoriesAndFiles = new ObservableCollection<string>();

        /// <summary>
        /// Коллекция скачиваемых файлов для передачи VM
        /// </summary>
        //private ObservableCollection<string> downloadingFile = new ObservableCollection<string>();

        /// <summary>
        /// Стек для возврата на уровни выше
        /// </summary>
        private Stack<string> workingPath = new Stack<string>();

        /// <summary>
        /// Коллекция флагов, определяющих директорию
        /// </summary>
        private List<bool> isDirectory = new List<bool>();

        /// <summary>
        /// Директория, на которую "смотрит" сервер
        /// </summary>
        private string serverPath = "";

        /// <summary>
        /// Путь на данном шаге
        /// </summary>
        private string currentServerPath = "";

        /// <summary>
        /// Путь для сохранения файлов (объект из класса модели)
        /// </summary>
        public string pathToSaveFileModel = new DirectoryInfo(Directory.GetCurrentDirectory()).
                                    Parent.Parent.FullName + @"\Donwload";

        /// <summary>
        /// Получить путь, на который смотрит сервер
        /// </summary>
        private void GetServerPath()
        {
            if (serverPath == "")
            {
                Directory.CreateDirectory(pathToSaveFileModel);
                try
                {
                    using (var client = new TcpClient(modelAddress, Convert.ToInt32(modelPort)))
                    {                    
                        var stream = client.GetStream();
                        var writer = new StreamWriter(stream);
                        writer.WriteLine("path");
                        writer.WriteLine("giveMePath");
                        writer.Flush();

                        var reader = new StreamReader(stream);  // получили путь, если его не было
                        serverPath = reader.ReadLine();

                        currentServerPath = serverPath;                    
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        // Мб это можно использовать и как обновление дерева
        // GetDirectoriesAndFiles ------ мб назвать так
        // todo
        /// <summary>
        /// Получение дерева директорий
        /// </summary>
        public ObservableCollection<string> OnConnectionShowDirectoriesTree(bool isUpdateTree, string addDirectoryToServerPath)
        {
            GetServerPath();            
            if (isUpdateTree)
            {                
                if (addDirectoryToServerPath == "/")
                {
                    currentServerPath = workingPath.Pop();
                    //if (workingPath.Count == 1)
                    //{
                    // ??? чтобы не уйти глубже. ЛИБО ПРОСТО НЕ ДОБАЛВЯТЬ "/" у корня
                    //}
                }
                else
                {                    
                    workingPath.Push(currentServerPath);                    
                    currentServerPath += @"\" + addDirectoryToServerPath;
                }                
            }

            // Надо сделать корректный путь
            //-------------------------------------
            try
            {
                using (var client = new TcpClient(modelAddress, Convert.ToInt32(modelPort)))
                {                                
                    var stream = client.GetStream();
                    var writer = new StreamWriter(stream);
                    writer.WriteLine("Listing");
                    writer.WriteLine(currentServerPath);
                    writer.Flush();                    

                    var reader = new StreamReader(stream);
                    var stringDirsAndFiles = reader.ReadLine(); 

                    var splitDirsAndFiles = stringDirsAndFiles.Split(' ');
                    var dirsArray  = splitDirsAndFiles[0].Split('/');       
                    var filesArray = splitDirsAndFiles[1].Split('/');

                    directoriesAndFiles.Clear();
                    isDirectory.Clear();

                    if (currentServerPath != serverPath)
                    {
                        directoriesAndFiles.Add("/");
                        isDirectory.Add(false);
                    }
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
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            viewModel.isDirectory = isDirectory;

            return directoriesAndFiles;
        }
        
        public async Task DownloadFile(string fileName)
        {
            try
            {
                using (var client = new TcpClient(modelAddress, Convert.ToInt32(modelPort)))
                {                
                    var stream = client.GetStream();
                    var writer = new StreamWriter(stream);
                    writer.WriteLine("Download");
                    writer.WriteLine(currentServerPath + @"\" + fileName); 
                    writer.Flush();

                    //downloadingFile.Add(currentServerPath + @"\" + fileName);
                    viewModel.DownloadingFiles.Add(fileName);

                    var reader = new StreamReader(stream);
                    var content = reader.ReadToEnd();

                    var textFile = new StreamWriter(pathToSaveFileModel + @"\" + fileName);
                    textFile.WriteLine(content);                    
                    textFile.Close();            
                    
                    viewModel.DownloadingFiles.Remove(fileName);
                    viewModel.DownloadedFiles.Add(fileName);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}