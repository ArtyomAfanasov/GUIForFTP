using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GUIForFTP
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception.Message == "Путь имеет недопустимую форму.")
            {
                MessageBox.Show("Чтобы скачать все файлы в папке сначала необходимо подключится к серверу");
            }
            else
            {
                MessageBox.Show(e.Exception.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            }                                     
            e.Handled = true;
        }
    }
}
