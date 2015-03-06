using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WebPageWidget;

namespace IWidget
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWidget" in both code and config file together.
    [ServiceContract]
    public interface IWidgetService
    {

        [OperationContract]
        string GetData(string value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        string GetWidget(Parameters parameters);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }

    [DataContract]
    public class Parameters
    {
        string folder = @"c:\temp\";
        string fileName = "Test";
        WebWidget widget = new WebWidget();

        [DataMember]
        public string Folder
        {
            get { return folder; }
            set { folder = value; }
        }

        [DataMember]
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        [DataMember]
        public WebWidget Widget
        {
            get { return widget; }
            set { widget = value; }
        }
    }
}
