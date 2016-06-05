namespace SimpleHighScoreApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.WindowsAzure.MobileServices;
    using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
    using Microsoft.WindowsAzure.MobileServices.Sync;

    public class AzureDataService
    {
        private MobileServiceClient mobileService;
        private IMobileServiceSyncTable<FirstHighScores> localTable;
        private IMobileServiceTable<FirstHighScores> remoteTable;

        public AzureDataService()
        {
            Debug.WriteLine("c'tor AzureDataService");
            this.mobileService = new MobileServiceClient("http://yourazurewebsite.net");
        }

        public async Task Initialize()
        {
            try
            {
                if (!this.mobileService.SyncContext.IsInitialized)
                {
                    // setup local sqlite store and intialize local table
                    MobileServiceSQLiteStore store = new MobileServiceSQLiteStore("localscores.db");
                    Debug.WriteLine("Created SQLite database ...");

                    store.DefineTable<FirstHighScores>();
                    Debug.WriteLine("Defined table in the SQLite database ...");

                    // need synchronization context
                    await this.mobileService.SyncContext.InitializeAsync(store);
                    Debug.WriteLine("Initialized synchronization context ...");

                    // retrieve references of tables (local and remote)
                    this.localTable = this.mobileService.GetSyncTable<FirstHighScores>();
                    this.remoteTable = this.mobileService.GetTable<FirstHighScores>();

                    Debug.WriteLine("Initialization done.");
                }
                else
                {
                    Debug.WriteLine("Initialization already done.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0}", ex.Message);
            }
        }

        public async Task InsertIntoLocalTable(String name, int score)
        {
            await this.Initialize();

            // create and insert a random score object into local table
            var someScore = new FirstHighScores()
            {
                Name = name,
                PlayedAt = DateTime.Now,
                Score = score
            };

            await this.localTable.InsertAsync(someScore);

            Debug.WriteLine("Inserted into local store: {0} [{1} points]", someScore.Name, someScore.Score);
        }

        public async Task<List<FirstHighScores>> Sync()
        {
            await this.Initialize();

            try
            {
                PullOptions options = new PullOptions () { MaxPageSize = 100 };
                await this.localTable.PullAsync("topHighScorers", this.localTable.CreateQuery(), options);
            }
            catch (MobileServicePushFailedException ex)
            {
                MobileServicePushStatus status = ex.PushResult.Status;
                if (status == MobileServicePushStatus.Complete)
                {
                    Debug.WriteLine("Unable to Sync -- Probably no Connectivity !!!: " + ex.Message);
                }
                else
                {
                    String err = String.Format("PullAsync failed: {0} errors, message: {1}",
                        ex.PushResult.Errors.Count, ex.Message);
                    Debug.WriteLine(err + " - " + ex.PushResult.Status);
                }
            }
            catch (WebException ex)
            {
                Debug.WriteLine("PullAsync failed: " + ex.Message);
            } 
            catch (Exception ex)
            {
                Debug.WriteLine("PullAsync failed: " + ex.Message);
            }

            // retrieve information from local table - either synched or not
            List<FirstHighScores> highScores =
                await this.localTable.OrderByDescending(item => item.Score).Take(10).ToListAsync();
            return highScores;
        }

        public async Task DumpLocalTable()
        {
            await this.Initialize();

            Debug.WriteLine("Dump of local Table:");

            // retrieve full information from local table
            List<FirstHighScores> highScores = await this.localTable.ToListAsync();
            Debug.WriteLine("Found entries: {0}", highScores.Count);
            foreach (FirstHighScores entry in highScores)
                Debug.WriteLine("  Entry: {0}", entry);

            Debug.WriteLine("");
            Debug.WriteLine("Pending operations in the sync context queue: {0}",
                this.mobileService.SyncContext.PendingOperations);
        }
    }
}
