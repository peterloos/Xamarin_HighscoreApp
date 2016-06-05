namespace SimpleHighScoreApp.Views
{
    using System;
    using System.Diagnostics;

    using SimpleHighScoreApp.ViewModels;
    using Xamarin.Forms;

    public partial class MainViewPage : ContentPage
    {
        private HighScoresViewModel viewModel;
        public MainViewPage()
        {
            Debug.WriteLine("c'tor MainViewPage");
            InitializeComponent();
            this.viewModel = (HighScoresViewModel) this.BindingContext;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // issuing command 'Appearing'
            this.viewModel.AppearingCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // issuing command 'Disappearing'
            this.viewModel.DisappearingCommand.Execute(null);
        }
    }
}
