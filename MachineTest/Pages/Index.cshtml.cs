using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;

namespace MachineTest.Pages
{
    public class IndexModel : PageModel
    {
        static string homeSecurityApiResponse;
        public string HomeSecurityStatus { get; private set; } = "DISARMED";
        public bool InputCodeVisible { get; private set; } = false;

        static HttpClient client = new HttpClient();


        public static async Task<Uri> CreateProductAsync()
        {
            var product = @"{'name':'mahdi'}";
            HttpResponseMessage response = await client.PostAsJsonAsync(
                @"https://gh21pxvwe5.execute-api.us-east-1.amazonaws.com/Prod", product);
            response.EnsureSuccessStatusCode();

            homeSecurityApiResponse = await response.Content.ReadAsStringAsync();

            return response.Headers.Location;
        }


    }
}