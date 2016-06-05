namespace SimpleHighScoreApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Plugin.Connectivity;
    using Plugin.Connectivity.Abstractions;
    using Xamarin.Forms;
    using SimpleHighScoreApp.Services;

    public class HighScoresViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private AzureDataService service;
        private ObservableCollection<HighScoreEntry> highScorers;
        private String playerName;
        private String playerScore;
        private String statusMessage;
        private String statusConnectivity;

        // c'tor
        public HighScoresViewModel()
        {
            this.service = new AzureDataService();
            this.highScorers = new ObservableCollection<HighScoreEntry>();

            this.StatusConnectivity =
                (CrossConnectivity.Current.IsConnected) ? "Online" : "Offline";  
        }

        // properties
        public ObservableCollection<HighScoreEntry> HighScorers
        {
            get
            {
                return this.highScorers;
            }

            set
            {
                this.highScorers = value;
            }
        }

        public String PlayerName
        {
            get
            {
                return this.playerName;
            }

            set
            {
                if (this.playerName != value)
                {
                    this.playerName = value;

                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(
                            this, new PropertyChangedEventArgs("PlayerName"));
                    }
                }
            }
        }

        public String PlayerScore
        {
            get
            {
                return this.playerScore;
            }

            set
            {
                if (this.playerScore != value)
                {
                    this.playerScore = value;

                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(
                            this, new PropertyChangedEventArgs("PlayerScore"));
                    }
                }
            }
        }

        public String StatusMessage
        {
            get
            {
                return this.statusMessage;
            }

            set
            {
                if (this.statusMessage != value)
                {
                    this.statusMessage = value;

                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(
                            this, new PropertyChangedEventArgs("StatusMessage"));
                    }
                }
            }
        }

        public String StatusConnectivity
        {
            get
            {
                return this.statusConnectivity;
            }

            set
            {
                if (this.statusConnectivity != value)
                {
                    this.statusConnectivity = value;

                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(
                            this, new PropertyChangedEventArgs("StatusConnectivity"));
                    }
                }
            }
        }

        // commands
        public ICommand InsertCommand
        {
            get
            {
                return new Command(async () =>
                {
                    Debug.WriteLine("> InsertCommand");
                    this.StatusMessage = "";

                    if (String.IsNullOrEmpty (this.PlayerName) || String.IsNullOrEmpty (this.PlayerScore))
                    {
                        this.StatusMessage = "ERROR: Please enter Username or Score!";
                        return;
                    }

                    int scoreValue;
                    if (!Int32.TryParse(this.PlayerScore, out scoreValue))
                    {
                        this.StatusMessage = "ERROR: Score Value: Illegal format!";
                        return;
                    }

                    // reset input fields
                    String userName = this.PlayerName;
                    this.PlayerName = "";
                    this.PlayerScore = "";

                    // enter players name and score into local database
                    await this.Insert(userName, scoreValue);
                    Debug.WriteLine("< InsertCommand");
                });
            }
        }

        public ICommand SynchCommand
        {
            get
            {
                return new Command(async () =>
                {
                    this.StatusMessage = "";
                    Debug.WriteLine("> SynchCommand");
                    await this.Sync();
                    Debug.WriteLine("< SynchCommand");
                });
            }
        }

        public ICommand DumpCommand
        {
            get
            {
                return new Command(async () =>
                {
                    Debug.WriteLine("> DumpCommand");
                    await this.DumpLocalTable();
                    Debug.WriteLine("> DumpCommand");
                });
            }
        }

        public ICommand AppearingCommand
        {
            get
            {
                return new Command(() =>
                {
                    CrossConnectivity.Current.ConnectivityChanged += this.ConnecitvityChanged;
                    Debug.WriteLine("OnAppearing: Connectivity is now {0}", CrossConnectivity.Current.IsConnected);
                });
            }
        }

        public ICommand DisappearingCommand
        {
            get
            {
                return new Command(() =>
                {
                    CrossConnectivity.Current.ConnectivityChanged -= this.ConnecitvityChanged;
                    Debug.WriteLine("OnDisappearing: Connectivity is now {0}", CrossConnectivity.Current.IsConnected);
                });
            }
        }

        private void ConnecitvityChanged(Object sender, ConnectivityChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Debug.WriteLine("Connectivity is now {0}", e.IsConnected);
                this.StatusConnectivity = (e.IsConnected) ? "Online" : "Offline";  
            });
        }

        // private (asynchronous) helper methods 
        private async Task Insert (String name, int score)
        {
            // insert name and score into local database
            await this.service.InsertIntoLocalTable(name, score);

            // update observable collection bind to UI
            HighScoreEntry next = new HighScoreEntry()
            {
                Name = name,
                Score = score,
                Position = 1
            };

            if (this.highScorers.Count == 0)
            {
                // empty collection
                this.highScorers.Add(next);
            }
            else
            {
                // seach position in observable collection, where to insert
                int i = this.highScorers.Count;
                while (i > 0)
                {
                    HighScoreEntry entry = this.highScorers[i-1];
                    if (score > entry.Score)
                        i--;
                    else
                        break;
                }

                // insert new entry
                this.highScorers.Insert(i, next);
            }

            // adjust property 'Position' of all entries
            for (int i = 0; i < this.highScorers.Count; i++)
            {
                HighScoreEntry entry = this.highScorers[i];
                entry.Position = i + 1;
            }
        }

        private async Task Sync()
        {
            // retrieve actual list of high scores from backend
            List<FirstHighScores> currentHighScores = await this.service.Sync();

            this.highScorers.Clear();

            // map list into observable collection (without AutoMapper :-( )
            for (int i = 0; i < currentHighScores.Count; i++)
            {
                HighScoreEntry entry = new HighScoreEntry()
                {
                    Name = currentHighScores[i].Name,
                    Score = currentHighScores[i].Score,
                    Position = i+1
                };

                this.highScorers.Add(entry);
            }
        }

        private async Task DumpLocalTable()
        {
            await this.service.DumpLocalTable();
        }
    }
}
