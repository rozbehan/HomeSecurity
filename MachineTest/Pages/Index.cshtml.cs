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
using Newtonsoft.Json.Linq;
using HomeSecurityApi;

namespace MachineTest.Pages
{
    public class IndexModel : PageModel
    {
        public string MachineResponse { get; private set; } = "";
        [BindProperty]
        public string MachineStatus { get; private set; } = "";
        public bool InputCodeVisible { get; private set; } = false;
        public string Code { get; private set; }

        static HttpClient client = new HttpClient();

        public void OnGet()
        {
            if (MachineStatus == "")
            {
                IoTConnect();
            }
        }

        public async Task<Uri> IoTConnect()
        {
            var request = JsonConvert.SerializeObject(new ApiConnectBody());
            var a = await IoTPostAsync(request);
            return a;
        }

        public async Task<Uri> IoTDefault()
        {
            var request = JsonConvert.SerializeObject(new ApiDefaultBody());
            var a = await IoTPostAsync(request);
            return a;
        }

        public async Task<Uri> IoTArm()
        {
            var request = JsonConvert.SerializeObject(new ApiRequestBody() { Event = "Arm", State = "Disarmed", Code = "1111" });
            var a = await IoTPostAsync(request);
            return a;
        }

        public async Task<Uri> IoTDisarm()
        {
            var request = JsonConvert.SerializeObject(new ApiRequestBody() { Event = "Disarm", State = "Armed", Code = "1111" });
            var a = await IoTPostAsync(request);
            return a;
        }

        public async Task<Uri> IoTPostAsync(string request)
        {
            HttpResponseMessage response = await client.PostAsync(
                Startup.HomeSecurityApiUrl, new StringContent(request, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            MachineResponse = await response.Content.ReadAsStringAsync();
            SetModel();

            return response.Headers.Location;
        }

        private void SetModel()
        {
            JObject apiResponse = new JObject();
            apiResponse = JObject.Parse(MachineResponse);
            //MachineStatus = (string)apiResponse.SelectToken(MachineDefinition.State);
            //InputCodeVisible = false;
        }

    }
}