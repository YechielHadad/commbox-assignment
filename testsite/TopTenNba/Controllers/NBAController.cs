using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Commbox.Controllers
{
    public class NBAController 
    {
        private Dictionary<string, NBAPlayerFullData> playersFullData = new Dictionary<string, NBAPlayerFullData>();
        private Dictionary<string, List<NBAPlayerFullData>> topTen = new Dictionary<string, List<NBAPlayerFullData>>();
        private Dictionary<string, NBATeam> teamsFullData = new Dictionary<string, NBATeam>();
        static object _lockPlayer = new object();
        static object _lockTeam = new object();

        public Task<List<NBAPlayerFullData>> GetAsync(string year)
        {
            return Task.Run(async () =>
            {
                HttpClient client = null;
                List<NBAPlayerFullData> players = new List<NBAPlayerFullData>();
                try
                {
                    if (!topTen.ContainsKey(year))
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        client = new HttpClient();
                        client.BaseAddress = new Uri("https://data.nba.net/");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        string uri = $"data/10s/prod/v1/{year}/players.json";
                        HttpResponseMessage response = await client.GetAsync(uri);
                        string content = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            JObject keyValuePairs = JObject.Parse(content);
                            JToken league = keyValuePairs["league"];
                            if (league != null)
                            {
                                JArray standard = league["standard"] as JArray;
                                if (standard != null)
                                {
                                    IEnumerable<Task<object>> tasks = Enumerable.Range(0, standard.Count).Select(i => GetProfile(standard[i], client, year));
                                    await Task.WhenAll(tasks.Where(t => t != null));
                                }
                            }
                        }
                    }

                    if (topTen.ContainsKey(year))
                    {
                        if (topTen[year].Count > 10)
                        {
                            players = topTen[year].GetRange(0, 10);
                        }
                        else
                        {
                            players = topTen[year];
                        }
                    }
                    return players;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }

        private async Task<object> GetProfile(JToken item, HttpClient client, string requestedYear)
        {
            string personId = item["personId"].ToString();
            NBAPlayerFullData player = null;
            if (!playersFullData.ContainsKey(personId))
            {
                string playerUri = $"data/10s/prod/v1/{requestedYear}/players/{personId}_profile.json";

                HttpResponseMessage response = await client.GetAsync(playerUri);

                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    JObject keyValuePairs = JObject.Parse(content);
                    JToken leagu = keyValuePairs["league"];     
                    List<JToken> seasons = leagu.FindTokens("season");
                    bool seasonFound = false;
                    if (seasons != null && seasons.Count > 0)
                    {
                        foreach (JToken season in seasons[0])
                        {
                            if (season["seasonYear"].ToString() == requestedYear)
                            {
                                JArray teams = season["teams"] as JArray;
                                if (teams != null && teams.Count > 0)
                                {
                                    JToken team = teams[0];
                                    player = new NBAPlayerFullData();
                                    player.ppg = !string.IsNullOrEmpty(team["ppg"].ToString()) ? Convert.ToDouble(team["ppg"]) : 0;
                                    player.rpg = !string.IsNullOrEmpty(team["rpg"].ToString()) ? Convert.ToDouble(team["rpg"]) : 0;
                                    player.apg = !string.IsNullOrEmpty(team["apg"].ToString()) ? Convert.ToDouble(team["apg"]) : 0;
                                    player.bpg = !string.IsNullOrEmpty(team["bpg"].ToString()) ? Convert.ToDouble(team["bpg"]) : 0;
                                    player.fgp = !string.IsNullOrEmpty(team["fgp"].ToString()) ? Convert.ToDouble(team["fgp"]) : 0;

                                    player.team = new NBATeam();
                                    string teamId = team["teamId"].ToString();
                                    if (teamsFullData.ContainsKey(teamId))
                                    {
                                        player.team = teamsFullData[teamId];
                                    }
                                    else
                                    {
                                        player.team.teamId = teamId;
                                        string teamUri = $"data/10s/prod/v1/{requestedYear}/teams.json";
                                        HttpResponseMessage teamResponse = await client.GetAsync(teamUri);

                                        content = await teamResponse.Content.ReadAsStringAsync();
                                        if (teamResponse.IsSuccessStatusCode)
                                        {
                                            keyValuePairs = JObject.Parse(content);
                                            leagu = keyValuePairs["league"];
                                            JArray standard = leagu["standard"] as JArray;
                                            if (standard != null)
                                            {
                                                IEnumerable<Task<object>> tasks = Enumerable.Range(0, standard.Count).Select(i => SetTeamData(standard[i], client, requestedYear));
                                                await Task.WhenAll(tasks.Where(t => t != null));
                                                if (teamsFullData.ContainsKey(teamId))
                                                {
                                                    player.team = teamsFullData[teamId];
                                                }

                                            }
                                        }
                                    }
                                    seasonFound = true;
                                }
                                break;
                            }
                        }
                    }
                    if (seasonFound)
                    {
                        player.playerId = personId;
                        player.playerDateOfBirth = item["dateOfBirthUTC"].ToString();
                        player.playerFullName = item["firstName"].ToString() + " " + item["lastName"].ToString();
                        player.playerHeightInMeters = item["heightMeters"].ToString();
                        player.playerPosition = item["teamSitesOnly"] != null ? item["teamSitesOnly"]["posFull"].ToString() : "";

                        lock (_lockPlayer)
                        {
                            if (!playersFullData.ContainsKey(personId))
                            {
                                playersFullData.Add(personId, player);
                            }
                        }
                    }
                }
            }
            else
            {
                player = playersFullData[personId];
            }

            if (player != null)
            {
                lock (_lockPlayer)
                {
                    if (!topTen.ContainsKey(requestedYear))
                    {
                        topTen.Add(requestedYear, new List<NBAPlayerFullData>());
                    }
                    if (!topTen[requestedYear].Contains(player, new NBAPlayerEqualityComparer()))
                    {
                        topTen[requestedYear].InsertSorted(player, Comparer<NBAPlayerFullData>.Create((x, y) => x.CompareTo(y)));
                    }
                }
            }
            return null;
        }

        private Task<object> SetTeamData(JToken jToken, HttpClient client, string year)
        {
            NBATeam team = new NBATeam();
            team.teamId = jToken["teamId"].ToString();
            team.teamName = jToken["nickname"].ToString();
            team.teamCountry = jToken["city"].ToString();
            team.conferenceName = jToken["confName"].ToString();
            lock (_lockTeam)
            {
                if (!teamsFullData.ContainsKey(team.teamId))
                {
                    teamsFullData.Add(team.teamId, team);
                }
            }
            return null;
        }
    }
}
