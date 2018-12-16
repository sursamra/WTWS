using NUnit.Framework;
using RiskAnalyser;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Tests
{
    public class Tests
    {
        List<ProductRecordRaw> data = new List<ProductRecordRaw>();
        [SetUp]
        public void Setup()
        {            
            data.AddRange(new[]
            {
             new ProductRecordRaw
             {
                 Name = "Comp",
                 OriginYear = 1992,
                 DevelopmentYear = 1992,
                 IncrementalValue = 110
             },
             new ProductRecordRaw
             {
                 Name = "Comp",
                 OriginYear = 1992,
                 DevelopmentYear = 1993,
                 IncrementalValue = 170
             },
             new ProductRecordRaw
             {
                 Name = "Comp",
                 OriginYear = 1993,
                 DevelopmentYear = 1993,
                 IncrementalValue = 200
             },

             new ProductRecordRaw
             {
                 Name = "Non - Comp",
                 OriginYear = 1990,
                 DevelopmentYear = 1990,
                 IncrementalValue = 45.2M
             },
             new ProductRecordRaw
             {
                 Name = "Non - Comp",
                 OriginYear = 1990,
                 DevelopmentYear = 1991,
                 IncrementalValue = 64.8M
             },
             new ProductRecordRaw
             {
                 Name = "Non - Comp",
                 OriginYear = 1990,
                 DevelopmentYear = 1993,
                 IncrementalValue = 37
             },
             new ProductRecordRaw
             {
                 Name = "Non - Comp",
                 OriginYear = 1991,
                 DevelopmentYear = 1991,
                 IncrementalValue = 50
             },
             new ProductRecordRaw
             {
                 Name = "Non - Comp",
                 OriginYear = 1991,
                 DevelopmentYear = 1992,
                 IncrementalValue = 75
             },
             new ProductRecordRaw
             {
                 Name = "Non - Comp",
                 OriginYear = 1991  ,
                 DevelopmentYear = 1993,
                 IncrementalValue = 25
             },
             new ProductRecordRaw
             {
                 Name = "Non - Comp",
                 OriginYear = 1992,
                 DevelopmentYear = 1992,
                 IncrementalValue = 55
             },
             new ProductRecordRaw
             {
                 Name = "Non - Comp",
                 OriginYear = 1992,
                 DevelopmentYear = 1993,
                 IncrementalValue = 85
             },
             new ProductRecordRaw
             {
                 Name = "Non - Comp",
                 OriginYear = 1993,
                 DevelopmentYear = 1993,
                 IncrementalValue = 100
             } });
        }

        [Test]
        public void TestOriginYear()
        {
            ClaimRingfence claim = new ClaimRingfence();           
           Report report = claim.GetReport(data);
            Assert.IsTrue(report.OriginYear == 1990);
        }
        [Test]
        public void TestDevelopmentYears()
        {
            ClaimRingfence claim = new ClaimRingfence();
            Report report = claim.GetReport(data);
            Assert.IsTrue(report.DevelopmentYears == 4);
        }
        [Test]
        public void TestAllProducts()
        {
            ClaimRingfence claim = new ClaimRingfence();
            Report report = claim.GetReport(data);
            Assert.IsTrue(2 == report.Data.Keys.Count);
        }
        [Test]
        public void TestMissingYearReport()
        {
            ClaimRingfence claim = new ClaimRingfence();
            Report report = claim.GetReport(data);
            Assert.AreEqual("0,0,0,0,0,0,0" , report.Data.Values.First().Substring(0, "0,0,0,0,0,0,0".Length));
        }

        [Test]
        public void TestMissingYearReport()
        {
            ClaimRingfence claim = new ClaimRingfence();
            Report report = claim.GetReport(data);
            Assert.AreEqual("0,0,0,0,0,0,0", report.Data.Values.First().Substring(0, "0,0,0,0,0,0,0".Length));
        }
    }
}