namespace SimpleHighScoreApp.Services
{
    using System;
    using Microsoft.WindowsAzure.MobileServices;
    using Newtonsoft.Json;

    [DataTable("FirstHighScores")]
    public class FirstHighScores
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [Version]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public String Name { get; set; }

        [JsonProperty(PropertyName = "PlayedAt")]
        public DateTime PlayedAt { get; set; }

        [JsonProperty(PropertyName = "Score")]
        public int Score { get; set; }

        public override string ToString()
        {
            return String.Format(
                "Highscore: Name={0}, PlayedAt={1}, Score={2}",
                this.Name, this.PlayedAt.ToString(), this.Score);
        }
    }
}
