using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MachineTest.Pages
{
    public class IndexModel : PageModel
    {
        public string HomeSecurityStatus { get; private set; } = "DISARMED";
        public bool InputCodeVisible { get; private set; } = false;

        public void OnGet()
        {
            

        }
        public IActionResult OnPost()
        {
            InputCodeVisible = !InputCodeVisible;
            return RedirectToPage("./Index");
        }


    }
}