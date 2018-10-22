using System;
using System.Collections.Generic;
using AppName.Web.Models;
using AppName.Web.Providers;
using AppName.Web.Services;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace AppTests.AppName.Web.Services
{
    [TestFixture]
    public class AccrualServiceTests
    {
        [TestCase(AccrualFrequency.Biweekly, Ending.CurrentYear, 4, null, null)]
        [TestCase(AccrualFrequency.SemiMonthly, Ending.CurrentYear, 3, 7, 21)]
        public void one_month_biweekly_generates_success(AccrualFrequency frequency, Ending ending, int expectedRows, int? dayOfPayA, int? dayOfPayB)
        {
            // arrange
            var dnp = new Mock<IDotNetProvider>();
            dnp.Setup(_ => _.DateTimeNow).Returns(new DateTime(2018, 10, 11));
            
            var config = CreateAccrualInstance(frequency, ending, dayOfPayA, dayOfPayB);
            
            // act
            var service = new AccrualService(dnp.Object);
            var rows = service.Calculate(config);
            
            // assert
            Assert.IsNotNull(rows);
            Assert.AreEqual(expectedRows, rows.Count);
            Console.WriteLine(JsonConvert.SerializeObject(rows, Formatting.Indented));
        }

        [Test]
        public void AccrualToJson()
        {
            var accrual = CreateAccrualInstance(AccrualFrequency.SemiMonthly, Ending.CurrentYear, 5, 21);
            Console.WriteLine(JsonConvert.SerializeObject(accrual, Formatting.Indented));
        }
        
        private static Accrual CreateAccrualInstance(AccrualFrequency frequency, Ending ending, int? dayOfPayA = null, int? dayOfPayB = null)
        {
            var accrual = new Accrual(
                accrualId: Guid.Parse("34bb297c-630e-4036-84c6-8925eaa88f80"),
                name: "Test Name",
                userId: "unittest|784734738",
                startingHours: 50,
                maxHours: 255,
                accrualRate: 10,
                accrualFrequency: frequency,
                dayOfPayA: dayOfPayA,
                dayOfPayB: dayOfPayB,
                startingDate: new DateTime(2018, 12, 1),
                ending: ending,
                isHeart: false,
                isArchived: false,
                minHours: 40, lastModified: new DateTime(2018, 10, 10, 3, 1, 12, 154), hourlyRate: 15,
                actions: new List<AccrualActionRecord>
                {
                    new AccrualActionRecord("c153da22-6df0-4fab-8069-7ef52946c635", AccrualAction.Created, null, null, null, new DateTime(2018, 10, 10)),
                    new AccrualActionRecord("e74f6d32-7ca8-4d6c-beab-89a6ff18aec3", AccrualAction.Adjustment, new DateTime(2018, 12, 3), 8, "jury duty 12/3/18", new DateTime(2018, 10, 10)),
                    new AccrualActionRecord("1fdfb091-0c99-4141-8943-82df48759ebd", AccrualAction.Adjustment, new DateTime(2018, 12, 4), 8, "jury duty 12/4/18", new DateTime(2018, 10, 10)),
                    new AccrualActionRecord("912304e3-ac13-4782-aa19-1be009ef164f", AccrualAction.Adjustment, new DateTime(2018, 12, 5), 8, "jury duty 12/5/18", new DateTime(2018, 10, 10)),
                    new AccrualActionRecord("2badfd74-d3ee-435a-88c9-4a5f604cfca2", AccrualAction.Adjustment, new DateTime(2018, 12, 25), 8, "christmas 2018", new DateTime(2018, 10, 10)),
                    
                });

            return accrual;
        }
    }
}