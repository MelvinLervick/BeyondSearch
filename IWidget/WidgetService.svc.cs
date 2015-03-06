using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WebPageWidget;
using WebPageWidget.Common;

namespace IWidget
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Widget" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Widget.svc or Widget.svc.cs at the Solution Explorer and start debugging.
    public class WidgetService : IWidgetService
    {
        public string GetData(string value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public string GetWidget(Parameters parameters)
        {
            var testFolder = ConfigurationManager.AppSettings["WidgetFolder"];
            var testWidget = parameters.Widget.CreateWidget();

            string html = testWidget;

            while (html.Contains("<widget"))
            {
                var poss = html.IndexOf("<widget", 0, System.StringComparison.Ordinal);
                var pose = html.IndexOf(">", poss, System.StringComparison.Ordinal);
                var widget = html.Substring(poss, pose - poss + 1);
                var posNs = widget.IndexOf("name=\"", 0, System.StringComparison.Ordinal) + 6;
                var posNe = widget.IndexOf("\"", posNs, System.StringComparison.Ordinal);
                var widgetName = widget.Substring(posNs, posNe - posNs);

                var widgetFileName = System.IO.Path.Combine(testFolder, widgetName + ".json");
                var dWidget = new WebWidget();
                if (dWidget.ReadWidgetFile(widgetFileName))
                {
                    var wHtml = dWidget.CreateWidget();
                    html = html.Replace(widget, wHtml);
                }
            }

            return html;
        }
    }
}
