using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text;
using Newtonsoft.Json;

namespace MachineTest.Pages
{
    public class IndexModel : PageModel
    {
        static string homeSecurityApiResponse;
        public string HomeSecurityStatus { get; private set; } = "DISARMED";
        public bool InputCodeVisible { get; private set; } = false;

        static HttpClient client = new HttpClient();

        public static async Task<Uri> IoTEventAsync()
        {
            //JsonMachine jsonMachine = new JsonMachine(@"{'event':'EnterCode' , 'state':'Disarmed', 'code':'1111'}");
            //var a = JsonConvert.SerializeObject(jsonMachine.ApiResponse);

            var request = @"{'event':'Arm' , 'state':'DISARMED', 'code':'1111'}";
            HttpResponseMessage response = await client.PostAsync(
                Startup.HomeSecurityApiUrl, new StringContent(request, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            homeSecurityApiResponse = await response.Content.ReadAsStringAsync();




            return response.Headers.Location;
        }


    }
}