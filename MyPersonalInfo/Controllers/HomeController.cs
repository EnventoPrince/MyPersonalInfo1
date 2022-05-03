using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MyPersonalInfo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MyPersonalInfo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public HomeController(IWebHostEnvironment hostEnvironment)
        {
            webHostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CandidateList()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public JsonResult SaveData(Candidate obj)
        {
            Response res = new Response();
            try
            {
                if (!string.IsNullOrEmpty(obj.FirstName) && !string.IsNullOrEmpty(obj.EmailAddress) && !string.IsNullOrEmpty(obj.GitProfile) && !string.IsNullOrEmpty(obj.AboutYou))
                {
                    if (Util.IsEmailValid(obj.EmailAddress))
                    {
                        string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploadDocs");

                        string CV = string.Empty;
                        if (obj.CVFile != null)
                        {
                            CV = Guid.NewGuid().ToString() + "_" + obj.CVFile.FileName;
                            string filePath = Path.Combine(uploadsFolder, CV);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                obj.CVFile.CopyTo(fileStream);
                            }
                        }

                        string CL = string.Empty;
                        if (obj.CoverLetter != null)
                        {
                            CL = Guid.NewGuid().ToString() + "_" + obj.CoverLetter.FileName;
                            string filePath = Path.Combine(uploadsFolder, CL);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                obj.CoverLetter.CopyTo(fileStream);
                            }
                        }
                        XDocument xmldoc = XDocument.Load(string.Concat(webHostEnvironment.WebRootPath, "/Candidates.xml"));
                        XElement node = xmldoc.Root.Elements("Candidate").Where(i => i.Element("EmailAddress").Value == obj.EmailAddress).FirstOrDefault();
                        var count = xmldoc.Descendants("Candidate").Count();
                        if (node == null)
                        {
                            XElement dep = new XElement("Candidate",
                            new XElement("Id", count + 1),
                            new XElement("FirstName", obj.FirstName),
                            new XElement("LastName", obj.LastName),
                            new XElement("EmailAddress", obj.EmailAddress),
                            new XElement("ContactNo", obj.ContactNo),
                            new XElement("GitProfile", obj.GitProfile),
                            new XElement("LiveInUS", obj.LiveInUS),
                            new XElement("AboutYou", obj.AboutYou),
                            new XElement("CVFile", CV),
                            new XElement("CoverLetter", CL),
                            new XElement("RegDate", Util.CurrentDate),
                            new XElement("Status", 1)
                            );
                            xmldoc.Root.Add(dep);
                            xmldoc.Save(string.Concat(webHostEnvironment.WebRootPath, "/Candidates.xml"));
                            res.Success = true;
                            res.Message = "Your data has been saved successfully.";
                        }
                        else
                        {
                            res.Success = false;
                            res.Message = "This Data already exist.";
                        }
                    }
                    else
                    {
                        res.Success = false;
                        res.Message = "Use valid email address.";
                    }
                }
                else
                {
                    res.Success = false;
                    res.Message = "Please input valid data.";
                }
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = ex.Message;
            }
            return new JsonResult(res);
        }


        [HttpPost]
        public JsonResult GetData(Candidate obj)
        {
            XElement xelement;
            xelement = XElement.Load(string.Concat(webHostEnvironment.WebRootPath, "/Candidates.xml"));

            //Reads data 
            var query = (from cand in xelement.Elements("Candidate") orderby Convert.ToInt32(cand.Element("Id").Value) descending select cand);
            
            List<Candidate> CandidateList = new List<Candidate>();

            if (query != null)
            {
                foreach (var data in query)
                {
                    Candidate d = new Candidate();
                    d.Id = Convert.ToInt32(data.Element("Id").Value);
                    d.FirstName = data.Element("FirstName").Value;
                    d.LastName = data.Element("LastName").Value;
                    d.Name = d.FirstName+" "+ d.LastName;
                    d.EmailAddress = data.Element("EmailAddress").Value;
                    d.ContactNo = data.Element("ContactNo").Value;
                    d.GitProfile = data.Element("GitProfile").Value;
                    d.LiveInUS = Convert.ToBoolean(data.Element("LiveInUS").Value);
                    d.AboutYou = data.Element("AboutYou").Value;
                    d.RegDate = Convert.ToDateTime(data.Element("RegDate").Value);
                    d.Reg_Date = d.RegDate.ToString("dd-MMM-yyyy");
                    d.Status = Convert.ToInt32(data.Element("Status").Value);
                    if (d.Status == 1)
                    {
                        d.StatusTxt = "<span class='badge badge-success'>Approved</span>";
                    }
                    else if (d.Status == -1)
                    {
                        d.StatusTxt = "<span class='badge badge-danger'>Rejected</span>";
                    }
                    else if (d.Status == 0)
                    {
                        d.StatusTxt = "<span class='badge badge-warning'>Pending</span>";
                    }
                    d.CV = "<a href='" + d.CVFile + "' class='image' target='_blank' title='View CV'><i class='fa fa-eye' aria-hidden='true'></i></a>&nbsp;|&nbsp;<a href='" + d.CVFile + "' class='image' download='download' title='Download CV'><i class='fa fa-download' aria-hidden='true'></i></a>";
                    if (data.Element("CoverLetter").Value != string.Empty)
                    {
                        d.CL = "<a href='" + d.CoverLetter + "' class='image' target='_blank' title='View Cover Letter'><i class='fa fa-eye' aria-hidden='true'></i></a>&nbsp;|&nbsp;<a href='" + d.CoverLetter + "' class='image' download='download' title='Download Cover Letter'><i class='fa fa-download' aria-hidden='true'></i></a>";
                    }
                    else
                    {
                        d.CL = "No File";
                    }
                   CandidateList.Add(d);
                }
            }
            else
            {
                CandidateList.Add(null);
            }
            return new JsonResult(CandidateList);
        }
    }
}
