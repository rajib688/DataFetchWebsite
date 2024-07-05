using DataFetchWebsite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFetchWebsite.Services
{
    public interface IWebsiteDataService
    {
        Task GetAllWebsiteDataAsync();
    }
}
