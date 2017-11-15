using System;
using System.ComponentModel.DataAnnotations;

namespace Seguranca_WebApi.Model
{
	public class LoginUser
    {
        [Required]
        [EmailAddress]
        public string Email
        {
            get;
            set;
        }
        [Required]
        public string Senha
        {
            get;
            set;
        }
    }
}
