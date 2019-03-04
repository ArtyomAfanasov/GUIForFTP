namespace SimpleFTP_Server
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// Класс, принимающий и обрабатывающий запросы о листинге и скачивании файлов
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Порт для подключения к серверу
        /// </summary>
        private const int port = 8888;

        /// <summary>
        /// IP адресс сервера
        /// </summary>
        private IPAddress localAdrress = IPAddress.Parse("127.0.0.1"); 

        /// <summary>
        /// Объект для мониторинга запросов
        /// </summary>
        private TcpListener listener;    

        /// <summary>
        /// Ответ на запрос о размере файла и его содержимом
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Размер файла и его содержимое</returns>
        private string[] Get(string path)
        {
            if (!File.Exists(path))
            {
                return new string[1] { "size=-1" };
            }

            var file = new FileInfo(path);                                    
                       
            var content = Deserialize(File.ReadAllLines(path));

            var answer = new string[2];
            answer[0] = "size=" + file.Length.ToString();                        
            answer[1] = content;
                             
            return answer; 
        }

        /// <summary>
        /// Ответ на запрос о:
        /// количестве файлов и папок в директории
        /// названии файла или папки 
        /// существование директории (если не существует, то вернёт size=-1)
        /// </summary>
        /// <param name="path">Путь директории или файла</param>
        /// <returns>
        /// Массив информации о:
        /// количестве файлов и папок в директории
        /// названии файла или папки 
        /// существование директории (если не существует, то вернёт size=-1)
        /// </returns>
        private string[] List(string path)
        {
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
        
        /// <summary>
        /// Перевод информации из массива строк в строку
        /// </summary>
        /// <param name="serializable">Массив информации</param>
        /// <returns>Строка с информацией</returns>
        private string Deserialize(string[] serializable)
        {
            string deserializeString = "";
            for (int i = 0; i < serializable.Length - 1; i++)
            {
                deserializeString += serializable[i] + " ";
            }
            deserializeString += serializable[serializable.Length - 1];

            return deserializeString;
        }

        /// <summary>
        /// Обработка запросов клиентов сервером
        /// </summary>
        public async void Listen()
        {           
            listener = new TcpListener(localAdrress, port);
            listener.Start();
            Console.WriteLine("Сервер слушает . . .");            
            while (true)
            {
                try
                {
                    using (var client = await listener.AcceptTcpClientAsync())
                    {
                        Console.WriteLine("Клиент подключился");

                        var stream = client.GetStream();
                        var reader = new StreamReader(stream);
                        var request = reader.ReadLine();
                        var path = reader.ReadLine();

                        Console.WriteLine($"Получен запрос: вид - {request}, путь - {path}");
                        string answer;
                        StreamWriter writer = new StreamWriter(stream);

                        switch (request)
                        {
                            case "1":
                                answer = Deserialize(List(path));
                                Console.WriteLine($"Буду отправлять: {answer}");
                                writer.WriteLine(answer);
                                writer.Flush();
                                break;
                            case "2":
                                answer = Deserialize(Get(path));
                                Console.WriteLine($"Буду отправлять: {answer}");
                                writer.WriteLine(answer);
                                writer.Flush();
                                break;
                            default:
                                answer = Deserialize(new string[1] { "Есть только запросы: 1 или 2" });
                                Console.WriteLine("Буду отправлять: Есть только запросы: 1 или 2");
                                writer.WriteLine(answer);
                                writer.Flush();
                                break;
                        }
                    }                                                                                                                                                  
                }
                catch (Exception e)
                {                    
                    Console.WriteLine($"\n\nОшибка {e.Message}");
                    Console.WriteLine("\n\nСервер снова слушает . . .");
                }               
            }            
        }
      
        private string[] ListDirecories(string path)
        {
            try
            {
                FileSystemInfo[] filesAndDirectories;

                if (Directory.Exists(path))
                {
                    var answer = new string[3];
                    var isDir = "isDir?-false";
                    var info = new DirectoryInfo(path);
                    
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
                    return new string[1] { "size=-1" };
                }                
            }
            catch (DirectoryNotFoundException e)
            {
                return new string[1] { $"Ошибка-{ e.Message }" };
            }

        }
    }
}