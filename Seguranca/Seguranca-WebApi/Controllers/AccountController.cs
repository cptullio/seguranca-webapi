using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Seguranca_WebApi.ForIdentity;
using Seguranca_WebApi.Model;


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

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody]RegisterUser user)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = new Usuario();

                usuario.NomeCompleto = user.NomeCompleto;
                usuario.UserName = user.Usuario;
                usuario.Email = user.Email;
                var result =   await _userManager.CreateAsync(usuario,user.Senha);
                if (result.Succeeded){
                    return Ok(new { msg = "Usuário criado com sucesso" });
                }
                return BadRequest(result.Errors);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody]LoginUser user)
        {
            if (ModelState.IsValid)
            {
                
                var result = await _userManager.FindByEmailAsync(user.Email);

                if (result != null)
                {
                    var passwordResult = await _userManager.CheckPasswordAsync(result, user.Senha);
                    if (passwordResult)
                    {

                        var claims = new List<Claim>
                        {
                            new Claim(JwtRegisteredClaimNames.GivenName, result.NomeCompleto),
                            new Claim(JwtRegisteredClaimNames.NameId, result.UserName),
                            new Claim(JwtRegisteredClaimNames.Email, result.Email)

                        };
                        var token = new JwtSecurityToken(
                            issuer: "localhost:5000",
                            audience: "localhost:5000",

                              claims: claims,
                              expires: DateTime.UtcNow.AddMinutes(30),
                              signingCredentials: new SigningCredentials(
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey_GetThisFromAppSettings")),
                                                        SecurityAlgorithms.HmacSha256));
                        

                        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                    }
                }
                return BadRequest(new {msg = "Usuário ou Senha Inválido."}); 
                    
            }
            return BadRequest(ModelState);
        }


        [HttpGet("[action]")]
        [Authorize]
        public IActionResult TesteAutorizacao()
        {
            return Ok(new {nome = "nome"});
        }

    }
}
