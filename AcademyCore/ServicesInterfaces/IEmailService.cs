using Academy.Core.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.ServicesInterfaces
{
    public interface IEmailService
    {
       public Task SendEmailAsync(Message message);
    }
}
