using Microsoft.Extensions.Configuration;
using System.Net.Http;

public class TeachersService : AirtableBaseService
{
    public TeachersService(HttpClient httpClient, IConfiguration configuration)
        : base(httpClient, configuration, configuration["Airtable:Tables:Teachers"])
    {
    }

}
