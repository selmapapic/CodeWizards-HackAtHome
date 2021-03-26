using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeWizards.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
