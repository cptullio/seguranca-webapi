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
        public async Task<IActionResult> Register([FromBody]RegisterUser user, [FromQuery] bool isAdmin )
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = new Usuario();

                usuario.NomeCompleto = user.NomeCompleto;
                usuario.UserName = user.Usuario;
                usuario.Email = user.Email;

                var result =   await _userManager.CreateAsync(usuario,user.Senha);
                if (result.Succeeded){
                    if (isAdmin)
                    {
                        var claims = new List<Claim>();
                        claims.Add(new Claim("isAdmin","true"));

                        var resultClaim = await _userManager.AddClaimsAsync(usuario, claims);
                        if (resultClaim.Succeeded)
                        {
                            return Ok(new { msg = "Administrador criado com sucesso" });
                        }
                    }
                        
                    return Ok(new { msg = "Usuário criado com sucesso" });
                }
                return BadRequest(result.Errors);
            }
            return BadRequest(ModelState);
        }


        [HttpGet("[action]")]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }


        private async Task<string> GenerateTokenAsync(Usuario usuario)
        {
            var claims = await _userManager.GetClaimsAsync(usuario);
            claims.Add(new Claim("Nome", usuario.NomeCompleto));
            claims.Add(new Claim("Email", usuario.Email));
            claims.Add(new Claim("UserName", usuario.UserName));
            var token = new JwtSecurityToken(
                issuer: "localhost:5000",
                audience: "localhost:5000",

                  claims: claims,
                  expires: DateTime.UtcNow.AddMinutes(30),
                  signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey_GetThisFromAppSettings")),
                                            SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);

        }



        [HttpGet("[action]")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                Usuario usuario = await _userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));

                return Ok(GenerateTokenAsync(usuario));
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
               
                Usuario usuario = await _userManager.FindByEmailAsync(email);

                if (usuario != null)
                {
                    var identityResult = await _userManager.AddLoginAsync(usuario, info);
                    if (identityResult.Succeeded)
                    {
                        return Ok(GenerateTokenAsync(usuario));
                    }
                    else
                    {
                        return BadRequest(identityResult.Errors);
                    }
                }
                else
                {
                    var nome = info.Principal.FindFirstValue(ClaimTypes.Name);
                    Usuario newUsuario = new Usuario();
                    newUsuario.NomeCompleto = nome;
                    newUsuario.UserName = email.Split('@')[0];
                    newUsuario.Email = email;

                    var resultCreate = await _userManager.CreateAsync(newUsuario);
                    if (resultCreate.Succeeded)
                    {
                        var identityResult = await _userManager.AddLoginAsync(usuario, info);
                        if (identityResult.Succeeded)
                        {
                            return Ok(GenerateTokenAsync(usuario));
                        }
                        else
                        {
                            return BadRequest(identityResult.Errors);
                        }

                    }
                    else
                    {
                        return BadRequest(resultCreate.Errors);
                    }
                }
            }
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
                        return Ok(GenerateTokenAsync(result));
                    }
                }
                return BadRequest(new {msg = "Usuário ou Senha Inválido."}); 
                    
            }
            return BadRequest(ModelState);
        }


        [HttpGet("[action]")]
        [Authorize(policy:"Admin")]
        public IActionResult TesteAutorizacaoAdministrador()
        {
            var emailClaim = Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Email");
            return Ok(new {Email = emailClaim.Value});
        }

        [HttpGet("[action]")]
        [Authorize]
        public IActionResult TesteAutorizacao()
        {
            
            var emailClaim = Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Email");
            return Ok(new { Email = emailClaim.Value });
           
        }

    }
}
