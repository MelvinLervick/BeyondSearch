using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebPageWidget;
using WebPageWidget.Common;

namespace WebPageWidgetUnitTests
{
    [TestClass]
    public class WebWidgetUnitTests
    {
        private WebWidget testWidget;
        private const string ValidWidgetDefinitionWithNameOnly = "{\"parameters\": {\"name\":\"Test Widget\"} }";
        private const string ValidWidgetDefinition = "{\"parameters\": {\"name\":\"Test Widget2\", \"author\":\"Test Author\", \"type\":\"page\", \"version\":\"1\", \"locked\":false, \"encrypted\":true, \"key\":\"keyvalue\"} }";

        [TestInitialize]
        public void Initialize()
        {
            testWidget = new WebWidget();
        }

        [TestMethod]
        public void WebWidget_VerifyInitialWidgetParameters()
        {
            VerifyDefaultWidget();
        }

        [TestMethod]
        public void WebWidgetInvalidJsonContentCreatesADefaultWidget()
        {
            testWidget = new WebWidget("This is an invalid JSON snippet");
            VerifyDefaultWidget();
        }

        [TestMethod]
        public void WebWidgetValidJsonContentWithNameOnlySetsWidgetName()
        {
            testWidget = new WebWidget(ValidWidgetDefinitionWithNameOnly);
            Assert.AreEqual( "Test Widget", testWidget.Parameters.Name,
                "Verify WebWidget Parameter Name is initialized correctly.");

        }

        [TestMethod]
        public void WebWidgetValidJsonContentParametersSetsWidgetContentParameters()
        {
            testWidget = new WebWidget(ValidWidgetDefinition);
            Assert.AreEqual("Test Widget2", testWidget.Parameters.Name,
                "Verify WebWidget Parameter Name is initialized correctly.");

        }

        private void VerifyDefaultWidget()
        {
            Assert.AreEqual("Enter Widget's Name", testWidget.Parameters.Name,
                "Verify WebWidget Parameter Name is initialized correctly.");
            Assert.AreEqual("Enter Author's Name", testWidget.Parameters.Author,
                "Verify WebWidget Parameter Author is initialized correctly.");
            Assert.AreEqual(WidgetType.Site, testWidget.Parameters.Type,
                "Verify WebWidget Parameter Type is initialized correctly.");
            Assert.AreEqual( "0", testWidget.Parameters.Version,
                "Verify WebWidget Parameter Version is initialized correctly.");
            Assert.IsFalse(testWidget.Parameters.Locked, "Verify WebWidget Parameter Locked is initialized correctly.");
            Assert.IsFalse( testWidget.Parameters.Encrypted,
                "Verify WebWidget Parameter Encrypted is initialized correctly.");
            Assert.AreEqual( "", testWidget.Parameters.Key, "Verify WebWidget Parameter Key is initialized correctly." );
        }
    }
}
