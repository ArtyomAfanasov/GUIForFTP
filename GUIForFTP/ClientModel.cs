namespace GUIForFTP
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;

    class ClientModel
    {
        /// <summary>
        /// Порт сервера
        /// </summary>
        private readonly string modelPort;

        /// <summary>
        /// Адрес сервера
        /// </summary>
        private readonly string modelAddress;

        private readonly ViewModel viewModel;

        public ClientModel(string portFromVM, string addressFromVM, ViewModel viewModel)
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
                                    FullName + @"\GUIForFTPDonwload";

        /// <summary>
        /// Получить путь, на который смотрит сервер
        /// </summary>
        private async Task GetServerPathOnConnectionToServer()
        {
            if (serverPath == "")
            {
                Directory.CreateDirectory(pathToSaveFileModel);

                using (var client = await Task.Factory.StartNew(() =>
                            new TcpClient(modelAddress, Convert.ToInt32(modelPort))))
                {

                    var stream = client.GetStream();
                    var writer = new StreamWriter(stream);
                    await writer.WriteLineAsync("path");
                    await writer.WriteLineAsync("giveMePath");
                    await writer.FlushAsync();

                    var reader = new StreamReader(stream);
                    serverPath = await reader.ReadLineAsync();

                    currentServerPath = serverPath;
                }                                               
            }
        }
        
        /// <summary>
        /// Запрос серверу о получении коллекции файлов и папок 
        /// </summary>
        public async Task<ObservableCollection<string>> ShowDirectoriesTree(bool isUpdateTree, string addDirectoryToServerPath)
        {
            await GetServerPathOnConnectionToServer();            
            if (isUpdateTree)
            {                
                if (addDirectoryToServerPath == "..")
                {
                    currentServerPath = workingPath.Pop();                    
                }
                else
                {                    
                    workingPath.Push(currentServerPath);                    
                    currentServerPath += @"\" + addDirectoryToServerPath;
                }                
            }

            using (var client = await Task.Factory.StartNew(() =>
                        new TcpClient(modelAddress, Convert.ToInt32(modelPort))))
            {

                var stream = client.GetStream();
                var writer = new StreamWriter(stream);
                await writer.WriteLineAsync("Listing");
                await writer.WriteLineAsync(currentServerPath);
                await writer.FlushAsync();

                var reader = new StreamReader(stream);
                var stringDirsAndFiles = await reader.ReadLineAsync();

                var splitDirsAndFiles = stringDirsAndFiles.Split(' ');
                var dirStringWithSpace = splitDirsAndFiles[0].Replace("?", " ");
                var dirsArray = dirStringWithSpace.Split('/');
                var filesStringWithSpace = splitDirsAndFiles[1].Replace("?", " ");
                var filesArray = filesStringWithSpace.Split('/');

                directoriesAndFiles.Clear();
                isDirectory.Clear();

                if (currentServerPath != serverPath)
                {
                    directoriesAndFiles.Add("..");
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

                viewModel.isDirectory = isDirectory;
            }

            return directoriesAndFiles;
        }
        
        /// <summary>
        /// Запрос серверу на скачивание файла
        /// </summary>
        /// <param name="fileName"></param>        
        public async Task DownloadFile(string fileName)
        {            
            //try
            //{
                using (var client = new TcpClient(modelAddress, Convert.ToInt32(modelPort)))
                {                
                    var stream = client.GetStream();
                    var writer = new StreamWriter(stream);
                    await writer.WriteLineAsync("Download");
                    await writer.WriteLineAsync(currentServerPath + @"\" + fileName); 
                    await writer.FlushAsync();
                    
                    viewModel.DownloadingFiles.Add(fileName);

                    var reader = new StreamReader(stream);
                    var content = await reader.ReadToEndAsync();

                    using (var textFile = new StreamWriter(pathToSaveFileModel + @"\" + fileName))
                    {
                        textFile.WriteLine(content);                        
                    }
                    
                    viewModel.DownloadingFiles.Add(fileName + " скачался!");
                    viewModel.DownloadedFiles.Add(fileName);
                }
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message); // ????????????? todo
            //}
        }

        /// <summary>
        /// Запрос серверу о необходимости скачать все файлы в директории
        /// </summary>        
        public async Task DownloadAllFiles()
        {
            // в случае, если путь для загрузок выбран корректный, но до подключения к серверу
            // И чтобы после этого и после подлкючения к серверу можно было качать файлы в уже выбранный корректный путь 
            if (((MainWindow)Application.Current.MainWindow).textBoxSavePath.Text != "")
            {
                viewModel.PathToSaveFile = ((MainWindow)Application.Current.MainWindow).textBoxSavePath.Text;
            }

            DirectoryInfo directoryInfo;

            //try
            //{
                /*if (currentServerPath != "")
                {
                    directoryInfo = new DirectoryInfo(currentServerPath);
                }
                else
                {
                    MessageBox.Show("Чтобы скачать все файлы в папке сначала необходимо подключится к серверу."); // ????????????? todo
                    return;
                }*/            
            
                directoryInfo = new DirectoryInfo(currentServerPath);

                if (directoryInfo.GetFiles().Length > 0)
                {                
                    foreach (FileInfo file in directoryInfo.GetFiles())
                    {
                        new Task(async () => await DownloadFile(file.Name)).
                                Start(TaskScheduler.FromCurrentSynchronizationContext());                                                                                            
                    }
                }
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message); // ????????????? todo
            //}
        }
    }
}