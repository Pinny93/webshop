using System;
using System.Windows;

namespace WPFDesktopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string Title
        {
            get { return "Webshop Verwaltung"; }
        }

        public new static App Current
        {
            get { return (App)Application.Current; }
        }
    }
}