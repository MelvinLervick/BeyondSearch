using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

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
    }
}
