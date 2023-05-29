using EnsekTechTest.Models;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;

namespace EnsekTechTest.Steps
{
    [Binding]
    public sealed class StepDefinitions
    {

        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly ScenarioContext _scenarioContext;

        public StepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;

        }


        [Given(@"I log on to the system under test")]
        public void GivenILogOnToTheSystemUnderTest()
        {
            var login = new LoginBody() { username = "test", password = "testing" };
            string body = JsonConvert.SerializeObject(login);
 
            var client = new RestClient();
            var request = new RestRequest($"{Settings.BaseUrl}/login",Method.Post);
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {Settings.BearerToken}");
            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            var response = client.Execute(request);

            if (response.IsSuccessStatusCode )
            {
                var responseBody = JsonConvert.DeserializeObject<LoginResponse>(response.Content);
                _scenarioContext["access_token"] = responseBody.access_token;
            }
        }

        [Given(@"I reset the fuel data")]
        public void GivenIResetTheFuelData()
        {
            var access_token = (string)_scenarioContext["access_token"];

            var client = new RestClient();
            var request = new RestRequest($"{Settings.BaseUrl}/reset", Method.Post);
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {access_token}");
            request.AddHeader("Content-Type", "application/json");

            var response = client.Execute(request);

            response.IsSuccessStatusCode.Should().BeTrue();
        }


        [Given(@"I retrieve a report of current energy levels")]
        public void GivenIRetrieveAReportOfCurrentEnergyLevels()
        {
            var client = new RestClient();
            var request = new RestRequest($"{Settings.BaseUrl}/energy", Method.Get);

            var response = client.Execute(request);

            if (response.IsSuccessStatusCode)
            {
                var energyReport = JsonConvert.DeserializeObject<Energy>(response.Content);
                _scenarioContext["initial_energy_report"] = energyReport;
            }
        }

        [When(@"I purchase (.*) units of fuel (.*)")]
        public void WhenIPurchaseUnitsOfFuel(int quantity, int fuelId)
        {
            var client = new RestClient();
            var request = new RestRequest($"{Settings.BaseUrl}/buy/{fuelId}/{quantity}", Method.Put);

            var response = client.Execute(request);

            if (response.IsSuccessStatusCode)
            {
                var message = JsonConvert.DeserializeObject<Message>(response.Content);

                if (message.message.StartsWith("There is no"))
                {
                    _scenarioContext["message"] = message.message;
                }
                else
                {
                    var messageElements = message.message.Split(' ');
                    int qtyReported = int.Parse(messageElements[3]);
                    var unitReported = messageElements[4];
                    decimal priceReported = decimal.Parse(messageElements[9]);
                    int remainingReported = int.Parse(messageElements[12]);

                    _scenarioContext["qtyReported"] = qtyReported;
                    _scenarioContext["unitReported"] = unitReported;
                    _scenarioContext["priceReported"] = priceReported;
                    _scenarioContext["remainingReported"] = remainingReported;
                    _scenarioContext["fuelPurchased"] = fuelId;
                }
            }
        }

        [Then(@"the response message is ""(.*)""")]
        public void ThenTheResponseMessageIs(string p0)
        {
            var message = (string)_scenarioContext["message"];
            message.Should().Be(p0);
        }

        [Then(@"the reported purchase quantity is (.*)")]
        public void ThenTheReportedPurchaseQuantityIs(int p0)
        {
            int qtyReported = (int)_scenarioContext["qtyReported"];
            qtyReported.Should().Be(p0);
        }


        [Then(@"the reported remaining quantity is (.*) less than the orginal total available")]
        public void ThenTheReportedRemainingQuantityIsLessThanTheOrginalTotalAvailable(int p0)
        {
            var energyReport = (Energy)_scenarioContext["initial_energy_report"];
            var fuelId = (int)_scenarioContext["fuelPurchased"];
            int fuelRemaining = (int)_scenarioContext["remainingReported"];
            var originalAvailable = 0;

            switch (fuelId)
            {
                case 1:
                    originalAvailable = energyReport.gas.quantity_of_units;
                    break;
                case 2:
                    originalAvailable = energyReport.nuclear.quantity_of_units;
                    break;
                case 3:
                    originalAvailable = energyReport.electric.quantity_of_units;
                    break;  
                case 4:
                    originalAvailable = energyReport.oil.quantity_of_units;
                    break;
            }

            originalAvailable.Should().Be(fuelRemaining + p0);


        }

        [Then(@"the reported price matches the orginal price multiplied by (.*)")]
        public void ThenTheReportedPriceMatchesTheOrginalPriceMultipliedBy(int p0)
        {
            var energyReport = (Energy)_scenarioContext["initial_energy_report"];
            var fuelId = (int)_scenarioContext["fuelPurchased"];
            decimal price = (decimal)_scenarioContext["priceReported"];
            decimal originalPrice = 0;  

            switch (fuelId)
            {
                case 1:
                    originalPrice = energyReport.gas.price_per_unit;
                    break;
                case 2:
                    originalPrice = energyReport.nuclear.price_per_unit;
                    break;
                case 3:
                    originalPrice = energyReport.electric.price_per_unit;
                    break;
                case 4:
                    originalPrice = energyReport.oil.price_per_unit;
                    break;
            }

            price.Should().Be(originalPrice * p0);
        }

        [Then(@"the number of orders made before today is (.*)")]
        public void ThenTheNumberOfOrdersMadeBeforeTodayIs(int p0)
        {
            var client = new RestClient();
            var request = new RestRequest($"{Settings.BaseUrl}/orders", Method.Get);

            var response = client.Execute(request);

            response.IsSuccessStatusCode.Should().BeTrue();
            
            var orders = JsonConvert.DeserializeObject<List<Order>>(response.Content);
            var countOfOrders = orders.Where(o => o.ts_time < DateTime.Today).Count();

            countOfOrders.Should().Be(p0);
            
        }



    }
}
