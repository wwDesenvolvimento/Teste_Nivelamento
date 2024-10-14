using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Program
{
    public static async Task Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = await GetTotalScoredGoals(teamName, year);

        Console.WriteLine("Team "+ teamName +" scored "+ totalGoals.ToString() + " goals in "+ year);

        teamName = "Barcelona";
        year = 2014;
        totalGoals = await GetTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static async Task<int> GetTotalScoredGoals(string team, int year)
    {

        int totalGoals = 0;

        // Soma os gols marcados como team1
        totalGoals += await GetGoalsForTeam(team, year, "team1");

        // Soma os gols marcados como team2
        totalGoals += await GetGoalsForTeam(team, year, "team2");

        return totalGoals;
    }

    private static async Task<int> GetGoalsForTeam(string team, int year, string teamRole)
    {
        int totalGoals = 0;
        int page = 1;
        bool hasMorePages = true;
        using (HttpClient client = new HttpClient())
        {
            while (hasMorePages)
            {
                string url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&{teamRole}={team}&page={page}";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(jsonData);

                    // Total de páginas
                    int totalPages = json["total_pages"].ToObject<int>();

                    // Processa cada partida e soma os gols
                    foreach (var match in json["data"])
                    {
                        totalGoals += match[$"{teamRole}goals"].ToObject<int>();
                    }

                    // Verifica se há mais páginas para processar
                    page++;
                    hasMorePages = page <= totalPages;
                }
                else
                {
                    Console.WriteLine("Erro ao acessar a API: " + response.ReasonPhrase);
                    hasMorePages = false;
                }
            }
        }

        return totalGoals;
    }

}