using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioWeb.Domain
{
    public class PortfolioUser : IdentityUser
    {
        public string Description { get; set; }
    }
}
