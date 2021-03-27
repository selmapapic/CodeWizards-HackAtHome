using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CodeWizards.Data;
using CodeWizards.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CodeWizards.Controllers
{
    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    public class QuestionFormController : Controller
    {
        private CWDbContext _dbContext;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public QuestionFormController(CWDbContext dbContext)
        {
            this._dbContext = dbContext;
            //this._httpContextAccessor = httpContextAccessor;
        }


        public IActionResult Form()
        {

            return View();
        }

        public async Task<IActionResult> GetResultsAsync(QuestionFormInfoReq req)
        {
            CovidStatisticsList covidStatistics = await GetStatisticsAsync();
            TodayCases todayCases = await GetTodaysCasesAsync();
            StatisticsResult stats = await CovidTestAsync(req);

            TodayAndGeneralStatistics todayAndGeneralStatistics = new TodayAndGeneralStatistics
            {
                CovidGeneral = covidStatistics,
                CovidToday = todayCases,
                statisticsRes = stats
            };

            return View(todayAndGeneralStatistics);
            
        }

        public async Task<StatisticsResult> CovidTestAsync(QuestionFormInfoReq req)
        {
            int[] simptomi = new int[14];
            for(int i=0; i<req.Simptomi.Length; i++)
            {
                simptomi[req.Simptomi[i] - 1] = 1;
            }

            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"Age", "Gender", "Travel in previous 15 days", "Live in COVID affected area", "Contact with COVID positive person", "Wearing mask properly", "Already had COVID", "Is vaccinated", "Chronical illnesses", "Immune system weakening therapy", "Risk group", "Temperature", "Dry cough", "Weakness", "Muscle and bone pain", "Loss of taste", "Loss of smell", "Headache", "Difficulty breathing", "Short breath", "Diarrhea", "Chest pain", "Difficulty while moving", "Loss of consciousness", "Throat ache"},
                                Values = new string[,] {  {
                                        req.Godine.ToString(),
                                        req.Spol.ToString(),
                                        req.Putovanje.ToString(),
                                        req.Mjesto.ToString(),
                                        req.Kontakt.ToString(),
                                        req.Maska.ToString(),
                                        req.Bolovanje.ToString(),
                                        req.Vakcina.ToString(),
                                        req.Hronicne.ToString(),
                                        req.Terapija.ToString(),
                                        req.Rizicno.ToString(),
                                        simptomi[0].ToString(),
                                        simptomi[1].ToString(),
                                        simptomi[2].ToString(),
                                        simptomi[3].ToString(),
                                        simptomi[4].ToString(),
                                        simptomi[5].ToString(),
                                        simptomi[6].ToString(),
                                        simptomi[7].ToString(),
                                        simptomi[8].ToString(),
                                        simptomi[9].ToString(),
                                        simptomi[10].ToString(),
                                        simptomi[11].ToString(),
                                        simptomi[12].ToString(),
                                        simptomi[13].ToString()
                                    },  { "0", "F", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" },  }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };
                const string apiKey = "OP5UQHHwXqYWjrjJ3ZsrPXanE99DWnMi6Kmxa7z4uTULgwrNZSCLo4jtnTvNtQG4O17IZFdmqqKc+MyY6YUTHA=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/dfac26d08b5740aa9f52c2ee56a84c7d/services/19f09fb417c74779bee22a3db047c1a9/execute?api-version=2.0&details=true");



                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    string[] splitThis = result.Split('"');
                    string finalResult = splitThis[splitThis.Length - 56];


                    double statisticData = Convert.ToDouble(finalResult);
                    int statDataRound = (int)Math.Round(statisticData * 100);

                    StatisticsResult statRes = new StatisticsResult
                    {
                        Result = statisticData,
                        ResultRound = statDataRound
                    };

                    return statRes;
                }
                else
                {
                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                    Console.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                }

                return null;
            }
        }


        private async Task<CovidStatisticsList> GetStatisticsAsync()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            var baseAddress = new Uri("https://api.covid19api.com/");


            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
                using (var response = await httpClient.GetAsync("dayone/country/bosnia-and-herzegovina"))
                {
                    string responseData = await response.Content.ReadAsStringAsync();

                    CovidStatistics[] covidStatistics = JsonConvert.DeserializeObject<CovidStatistics[]>(responseData);
                    List<CovidStatistics> listCovid = new List<CovidStatistics>() { };

                    for (int i = covidStatistics.Count() - 1; i > covidStatistics.Count() - 31; i--)
                    {
                        listCovid.Add(covidStatistics[i]);
                    }

                    listCovid.Reverse();

                    CovidStatisticsList covid = new CovidStatisticsList()
                    {
                        ListOfCovidStatistics = listCovid,
                    };

                    return covid;

                }

            }
        }

        private async Task<TodayCases> GetTodaysCasesAsync()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            var baseAddress = new Uri("https://corona.lmao.ninja/v2/");


            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
                using (var response = await httpClient.GetAsync("countries/Bosnia?yesterday=true&strict=true"))
                {
                    string responseData = await response.Content.ReadAsStringAsync();

                    TodayCases todayCases = JsonConvert.DeserializeObject<TodayCases>(responseData);

                    return todayCases;

                }

            }
        }

    }
}
