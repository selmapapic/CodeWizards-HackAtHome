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

            return View(covidStatistics);
            
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

                    CovidStatisticsList covid = new CovidStatisticsList()
                    {
                        ListOfCovidStatistics = listCovid,
                    };

                    return covid;

                }

            }
        }
        
    }
}
