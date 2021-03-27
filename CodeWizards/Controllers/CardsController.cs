using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeWizards.Controllers
{
    public class CardsController : Controller
    {
        public IActionResult Cards()
        {
            return View();
        }
    }
}
