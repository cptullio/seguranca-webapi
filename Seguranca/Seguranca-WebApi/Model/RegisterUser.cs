using System;
using System.ComponentModel.DataAnnotations;

namespace Seguranca_WebApi.Model
{
	public class RegisterUser
    {
        [Required]
        public string Usuario
        {
            get;
            set;
        }
        [Required]
        public string NomeCompleto
        {
            get;
            set;
        }
        [EmailAddress]
        [Required]
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
