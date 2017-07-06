using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using AddressSearch.Models;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;

namespace AddressSearch.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index(Address addr)
        {
            Address addrResult = new Address();
            string baseUrl = "http://www.zillow.com/webservice/GetSearchResults.htm";
            string zwsid = "X1-ZWz1dyb53fdhjf_6jziz";

            if (addr.street != null)
            {
                string cityStateZip = addr.city + ", " + addr.state + " " + addr.zipCode;
                string urlParamerters = baseUrl + BuildUrlParam(addr.street, cityStateZip, zwsid);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/xml"));

                    HttpResponseMessage response = client.GetAsync(urlParamerters).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var details = response.Content.ReadAsStreamAsync().Result;
                        //addrResult = JsonConvert.DeserializeObject<Address>(details);
                        Stream stream = details;
                        var sr = new StreamReader(stream);
                        var soapResponse = XDocument.Load(sr);

                        if (soapResponse.Descendants("message").FirstOrDefault().Element("code").Value == "0")
                        {
                            addrResult.street = soapResponse.Descendants("response").Descendants("address").FirstOrDefault().Element("street").Value;
                            addrResult.city = soapResponse.Descendants("response").Descendants("address").FirstOrDefault().Element("city").Value;
                            addrResult.state = soapResponse.Descendants("response").Descendants("address").FirstOrDefault().Element("state").Value;
                            addrResult.zipCode = soapResponse.Descendants("response").Descendants("address").FirstOrDefault().Element("zipcode").Value;
                            addrResult.latitude = soapResponse.Descendants("response").Descendants("address").FirstOrDefault().Element("latitude").Value;
                            addrResult.longitude = soapResponse.Descendants("response").Descendants("address").FirstOrDefault().Element("longitude").Value;
                            addrResult.amount = Convert.ToDecimal(soapResponse.Descendants("response").Descendants("zestimate").FirstOrDefault().Element("amount").Value);
                            addrResult.valueChange = Convert.ToDecimal(soapResponse.Descendants("response").Descendants("zestimate").FirstOrDefault().Element("valueChange").Value);
                            addrResult.low = Convert.ToDecimal(soapResponse.Descendants("response").Descendants("zestimate").Descendants("valuationRange").FirstOrDefault().Element("low").Value);
                            addrResult.high = Convert.ToDecimal(soapResponse.Descendants("response").Descendants("zestimate").Descendants("valuationRange").FirstOrDefault().Element("high").Value);
                            addrResult.homeDetails = soapResponse.Descendants("response").Descendants("links").FirstOrDefault().Element("homedetails").Value;
                            addrResult.graphsAndData = soapResponse.Descendants("response").Descendants("links").FirstOrDefault().Element("graphsanddata").Value;
                            addrResult.mapThisHome = soapResponse.Descendants("response").Descendants("links").FirstOrDefault().Element("mapthishome").Value;
                            addrResult.comparables = soapResponse.Descendants("response").Descendants("links").FirstOrDefault().Element("comparables").Value;
                            addrResult.regionName = soapResponse.Descendants("response").Descendants("localRealEstate").FirstOrDefault().Element("region").Attribute("name").Value;
                            addrResult.regionType = soapResponse.Descendants("response").Descendants("localRealEstate").FirstOrDefault().Element("region").Attribute("type").Value;
                            addrResult.zindexValue = Convert.ToDouble(soapResponse.Descendants("response").Descendants("localRealEstate").Descendants("region").FirstOrDefault().Element("zindexValue").Value);
                            addrResult.forSaleByOwner = soapResponse.Descendants("response").Descendants("localRealEstate").Descendants("region").Descendants("links").FirstOrDefault().Element("forSaleByOwner").Value;
                            addrResult.forSale = soapResponse.Descendants("response").Descendants("localRealEstate").Descendants("region").Descendants("links").FirstOrDefault().Element("forSale").Value;
                        }
                        else
                        {
                            string errorMessage = soapResponse.Descendants("message").FirstOrDefault().Element("text").Value;
                            ModelState.AddModelError("Error", errorMessage);                            
                        }

                    }
                }
            }
            return View(addrResult);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        private string BuildUrlParam(string address, string cityStateZip, string zwsid)
        {
            string urlParam = "?zws-id=" + WebUtility.HtmlEncode(zwsid) + "&address=" + WebUtility.HtmlEncode(address) + "&citystatezip=" + WebUtility.HtmlEncode(cityStateZip);
            return urlParam;
        }
    }
}
