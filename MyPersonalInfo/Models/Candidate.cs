using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace MyPersonalInfo.Models
{
    public class Candidate
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string ContactNo { get; set; }
        public string GitProfile { get; set; }
        public bool LiveInUS { get; set; }
        public string AboutYou { get; set; }
        public IFormFile CVFile { get; set; }
        public IFormFile CoverLetter { get; set; }
        public string CV { get; set; }
        public string CL { get; set; }
        public int? Status { get; set; }
        public string StatusTxt { get; set; }
        public DateTime RegDate { get; set; }
        public string Reg_Date { get; set; }
    }

    public class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
