namespace SimpleHighScoreApp
{
    using System;
    using System.Diagnostics;

    using SimpleHighScoreApp.Views;
    using Xamarin.Forms;

    public class App : Application
    {
        public App()
        {
            Debug.WriteLine("===================================================================");
            Debug.WriteLine("c'tor App");
            this.MainPage = new NavigationPage(new MainViewPage());
        }
    }
}
