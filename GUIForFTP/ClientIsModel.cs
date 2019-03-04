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
    }
}