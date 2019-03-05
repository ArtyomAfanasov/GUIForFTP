namespace GUIForFTP
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;

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

        public ClientIsModel(string portFromVM, string addressFromVM)
        {
            modelPort = portFromVM;
            modelAddress = addressFromVM;
        }        

        /// <summary>
        /// Имя файла или папки для отображение в дереве директорий
        /// </summary>
        public string FileAndDirectoryName;

        // todo
        /// <summary>
        /// Получение дерева директорий
        /// </summary>
        public void OnConnection()
        {
            
        }

        /// <summary>
        /// Получить массив папок и файлов
        /// </summary>
        /// <param name="path">Путь к директории</param>
        /// <returns>Массив папок и файлов</returns>
        private string[] DoList(string path)
        {
            DirectoryInfo directoryInfo;

            try
            {
                directoryInfo = new DirectoryInfo(path);
            }
            catch (DirectoryNotFoundException)
            {
                return new string[1] { "size=-1" };
            }

            var countFileAndDirectorys = directoryInfo.GetFileSystemInfos().Length;
            var answer = new string[countFileAndDirectorys];



            try
            {
                if (Directory.Exists(path))
                {
                    var answer = new string[3];
                    var isDir = "isDir?-false";
                    var info = new DirectoryInfo(path);
                    FileSystemInfo[] filesAndDirectories;
                    int countFileAndDirectorys;

                    filesAndDirectories = info.GetFileSystemInfos();
                    countFileAndDirectorys = filesAndDirectories.Length;

                    answer[0] = "size=" + countFileAndDirectorys.ToString();
                    answer[1] = "Папка-" + info.Name;

                    isDir = "isDir?-true";

                    answer[2] = isDir;

                    return answer;
                }
                else
                {
                    if (File.Exists(path))
                    {
                        var answer = new string[3];
                        var isDir = "isDir?-false";
                        var info = new DirectoryInfo(path);
                        FileSystemInfo[] filesAndDirectories;
                        int countFileAndDirectorys;

                        var infoFile = new FileInfo(path);
                        filesAndDirectories = info.Parent.GetFileSystemInfos();
                        countFileAndDirectorys = filesAndDirectories.Length;

                        answer[0] = "size=" + countFileAndDirectorys.ToString();
                        answer[1] = "Файл-" + infoFile.Name;

                        answer[2] = isDir;

                        return answer;
                    }
                    else
                    {
                        throw new DirectoryNotFoundException();
                    }
                }
            }
            catch (DirectoryNotFoundException e)
            {
                return new string[1] { "size=-1" };
            }
        }
    }
}