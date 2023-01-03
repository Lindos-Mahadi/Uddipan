using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using gBanker.Web;
using gBanker.Web.Controllers;
using gBanker.Web.Tests.Dependencies;
using gBanker.Service;
using gBanker.Web.ViewModels;
using gBanker.Service.ReportServies;

namespace gBanker.Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest: TestBase
    {
        private HomeController _homeController;
        //public HomeControllerTest(HomeController homeController)
        //{
            
        //    this._homeController = homeController;
        //}
        [TestInitialize]
        public void SetupTests()
        {
            var officeMappingService = container.Resolve<IEmployeeOfficeMappingService>();
            var officeService = container.Resolve<IOfficeService>();
            var dayInitialService = container.Resolve<IDayInitialService>();
            var memberService = container.Resolve<IMemberService>();
            var dashBoardService = container.Resolve<IDashBoardService>();
            var accService = container.Resolve<IAccReportService>();
            _homeController = new HomeController(officeMappingService, officeService, dayInitialService, memberService, dashBoardService, accService);
        }
        [TestMethod]
        public void Index()
        {           
            // Act
            ViewResult result = _homeController.Index() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void GetDashBoardItemsTest()
        {
            // Act
            var result = _homeController.GetDashboardItems();
            // Assert
            Assert.AreNotEqual((result.Data as DashboardViewModel).TotalOfficeCount, 0);
        }       
        [TestMethod]
        public void SelectOfficeTest()
        {
            // Act
            int officeId = 66;
            var result = _homeController.SelectOffice(officeId);
            // Assert
            Assert.IsNotNull(result);
        }
    }
}
