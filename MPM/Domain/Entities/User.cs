using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User: IdentityUser<long>, IBaseModel<long>
    {
        public DateTime CreateDateTime { get; set; }
    }
}
