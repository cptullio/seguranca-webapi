using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Seguranca_WebApi.ForIdentity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Seguranca_WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger _logger;
        private readonly SignInManager<Usuario> _signInManager;


        public AccountController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
        }

        [HttpGet("[action]")]
        public IActionResult Register(string nome, string username, string password)
        {
            return Ok(new { nome = nome });
        }
    }
}
