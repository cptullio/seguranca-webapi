using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Seguranca_WebApi.ForIdentity
{
    public class Contexto : IdentityDbContext<Usuario>
    {
        public Contexto(DbContextOptions options) : base(options)
        {
            this.Database.EnsureCreated();
        }
    }
}
