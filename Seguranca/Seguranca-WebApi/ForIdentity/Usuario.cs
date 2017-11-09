    using System;
    using Microsoft.AspNetCore.Identity;

    namespace Seguranca_WebApi.ForIdentity
    {
        public class Usuario : IdentityUser
        {
            public Usuario()
            {
            }

            public string NomeCompleto
            {
                get;
                set;
            }
        }
    }
