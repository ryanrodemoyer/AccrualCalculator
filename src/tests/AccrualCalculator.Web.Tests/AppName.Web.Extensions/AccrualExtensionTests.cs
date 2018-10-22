using System;
using System.Collections.Generic;
using AppName.Web.Extensions;
using AppName.Web.Models;
using AppName.Web.Providers;
using Microsoft.IdentityModel.Xml;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace AppTests.AppName.Web.Extensions
{
    [TestFixture]
    public class AccrualExtensionTests
    {
        private static Accrual CreateAccrualInstance(Ending ending)
        {
            var accrualActionRecords = new List<AccrualActionRecord> {new AccrualActionRecord("e1520621-a182-4404-b927-1a9b4a2c0c80", AccrualAction.Created, null, null, null, new DateTime(2018, 10, 10))};

            var accrual = new Accrual(
                accrualId: Guid.Parse("34bb297c-630e-4036-84c6-8925eaa88f80"),
                userId: "unittest|784734738",
                name: "Test Name",
                startingHours: 50,
                accrualRate: 10,
                startingDate: new DateTime(2018, 7, 4),
                accrualFrequency: AccrualFrequency.Biweekly,
                ending: ending,
                lastModified: new DateTime(2018, 10, 4, 16, 43, 14, 549),
                isHeart: false,
                isArchived: false,
                hourlyRate: 15,
                dayOfPayA: 5,
                dayOfPayB: 20,
                minHours: 40,
                maxHours: 255,
                actions: accrualActionRecords);

            return accrual;
        }
        
        [TestCase(Ending.CurrentYear, "2018-01-01", "2018-12-31")]
        [TestCase(Ending.CurrentYear, "2018-12-31", "2018-12-31")]
        [TestCase(Ending.PlusOne, "2018-12-31", "2019-12-31")]
        [TestCase(Ending.PlusTwo, "2019-01-01", "2021-12-31")]
        [TestCase(Ending.PlusThree, "2019-7-4", "2022-12-31")]
        public void EndingCurrentYear_EndDateIsCorrect(Ending ending, string currentDateString, string dateTimeString)
        {
            var dnp = new Mock<IDotNetProvider>();
            dnp.Setup(_ => _.DateTimeNow).Returns(DateTime.Parse(currentDateString));

            Accrual accrual = CreateAccrualInstance(ending);

            DateTime end = accrual.GetEndDate(dnp.Object);
            
            end.ShouldBe(DateTime.Parse(dateTimeString));
        }
    }
}