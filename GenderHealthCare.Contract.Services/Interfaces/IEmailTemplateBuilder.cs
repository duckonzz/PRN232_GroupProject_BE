using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IEmailTemplateBuilder
    {
        Task<string> BuildAsync<T>(string templateName, T model);
    }
}
