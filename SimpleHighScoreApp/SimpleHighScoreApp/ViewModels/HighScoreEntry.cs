namespace SimpleHighScoreApp.ViewModels
{
    using System;
    using System.ComponentModel;

    public class HighScoreEntry : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int position;

        public HighScoreEntry()
        {
            this.Name = String.Empty;
            this.Position = 0;
            this.Score = 0;
        }

        public String Name { get; set; }
        public int Score { get; set; }

        public int Position
        {
            set
            {
                if (value != this.position)
                {
                    this.position = value;

                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("Position"));
                    }
                }
            }

            get
            {
                return this.position;
            }
        }
    }
}
