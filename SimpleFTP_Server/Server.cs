﻿namespace SimpleFTP_Server
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
                            case "Listing":
                                answer = Deserialize(GetArrayOfFilesAndDirectoies(path));
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
                            case "path":
                                answer = new DirectoryInfo(Directory.GetCurrentDirectory()).
                                    Parent.Parent.FullName;
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

        // Будет разделяться "/". А при десериализации появится пробел в строке. 
        // По нему можно будет разделить папки и файлы
        // todo Массив из трёх элементов. Во 2ом - папки в 3ем - файлы
        /// <summary>
        /// Получить массив файлов и папок
        /// </summary>
        /// <param name="path">Путь к директории</param>
        /// <returns>
        /// Двухэлементный массив, где        
        /// на первом месте - папки
        /// на втором месте - файлы.       
        /// Если директория не найдена, то вернётся одноэлементый массив с элементом "size=-1"
        /// </returns>
        private string[] GetArrayOfFilesAndDirectoies(string path)
        {
            DirectoryInfo directoryInfo;

            try
            {
                directoryInfo = new DirectoryInfo(path);
            }
            catch (DirectoryNotFoundException)
            {
                return new string[] { "size=-1" };
            }

            var countFileAndDirectorys = directoryInfo.GetFileSystemInfos().Length;
            var answer = new string[2];            

            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                answer[0] += directory.Name + "/";                           // ??? Слеш
            }
            answer[0] = answer[0].TrimEnd('/');

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                answer[1] += file.Name + "/";                                // ??? Слеш
            }
            answer[1] = answer[1].TrimEnd('/');

            return answer;
        }
    }
}