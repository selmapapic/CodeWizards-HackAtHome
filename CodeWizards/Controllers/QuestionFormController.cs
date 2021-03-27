using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CodeWizards.Data;
using CodeWizards.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CodeWizards.Controllers
{
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

            TodayAndGeneralStatistics todayAndGeneralStatistics = new TodayAndGeneralStatistics
            {
                CovidGeneral = covidStatistics,
                CovidToday = todayCases
            };

            return View(todayAndGeneralStatistics);
            
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
