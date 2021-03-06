﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BeyondSearch.IWidget {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CompositeType", Namespace="http://schemas.datacontract.org/2004/07/IWidget")]
    [System.SerializableAttribute()]
    public partial class CompositeType : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool BoolValueField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string StringValueField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool BoolValue {
            get {
                return this.BoolValueField;
            }
            set {
                if ((this.BoolValueField.Equals(value) != true)) {
                    this.BoolValueField = value;
                    this.RaisePropertyChanged("BoolValue");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StringValue {
            get {
                return this.StringValueField;
            }
            set {
                if ((object.ReferenceEquals(this.StringValueField, value) != true)) {
                    this.StringValueField = value;
                    this.RaisePropertyChanged("StringValue");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Parameters", Namespace="http://schemas.datacontract.org/2004/07/IWidget")]
    [System.SerializableAttribute()]
    public partial class Parameters : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FileNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FolderField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private WebPageWidget.WebWidget WidgetField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FileName {
            get {
                return this.FileNameField;
            }
            set {
                if ((object.ReferenceEquals(this.FileNameField, value) != true)) {
                    this.FileNameField = value;
                    this.RaisePropertyChanged("FileName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Folder {
            get {
                return this.FolderField;
            }
            set {
                if ((object.ReferenceEquals(this.FolderField, value) != true)) {
                    this.FolderField = value;
                    this.RaisePropertyChanged("Folder");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public WebPageWidget.WebWidget Widget {
            get {
                return this.WidgetField;
            }
            set {
                if ((object.ReferenceEquals(this.WidgetField, value) != true)) {
                    this.WidgetField = value;
                    this.RaisePropertyChanged("Widget");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="IWidget.IWidgetService")]
    public interface IWidgetService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWidgetService/GetData", ReplyAction="http://tempuri.org/IWidgetService/GetDataResponse")]
        string GetData(string value);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWidgetService/GetData", ReplyAction="http://tempuri.org/IWidgetService/GetDataResponse")]
        System.Threading.Tasks.Task<string> GetDataAsync(string value);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWidgetService/GetDataUsingDataContract", ReplyAction="http://tempuri.org/IWidgetService/GetDataUsingDataContractResponse")]
        BeyondSearch.IWidget.CompositeType GetDataUsingDataContract(BeyondSearch.IWidget.CompositeType composite);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWidgetService/GetDataUsingDataContract", ReplyAction="http://tempuri.org/IWidgetService/GetDataUsingDataContractResponse")]
        System.Threading.Tasks.Task<BeyondSearch.IWidget.CompositeType> GetDataUsingDataContractAsync(BeyondSearch.IWidget.CompositeType composite);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWidgetService/GetWidget", ReplyAction="http://tempuri.org/IWidgetService/GetWidgetResponse")]
        string GetWidget(BeyondSearch.IWidget.Parameters parameters);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IWidgetService/GetWidget", ReplyAction="http://tempuri.org/IWidgetService/GetWidgetResponse")]
        System.Threading.Tasks.Task<string> GetWidgetAsync(BeyondSearch.IWidget.Parameters parameters);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IWidgetServiceChannel : BeyondSearch.IWidget.IWidgetService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WidgetServiceClient : System.ServiceModel.ClientBase<BeyondSearch.IWidget.IWidgetService>, BeyondSearch.IWidget.IWidgetService {
        
        public WidgetServiceClient() {
        }
        
        public WidgetServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WidgetServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WidgetServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WidgetServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string GetData(string value) {
            return base.Channel.GetData(value);
        }
        
        public System.Threading.Tasks.Task<string> GetDataAsync(string value) {
            return base.Channel.GetDataAsync(value);
        }
        
        public BeyondSearch.IWidget.CompositeType GetDataUsingDataContract(BeyondSearch.IWidget.CompositeType composite) {
            return base.Channel.GetDataUsingDataContract(composite);
        }
        
        public System.Threading.Tasks.Task<BeyondSearch.IWidget.CompositeType> GetDataUsingDataContractAsync(BeyondSearch.IWidget.CompositeType composite) {
            return base.Channel.GetDataUsingDataContractAsync(composite);
        }
        
        public string GetWidget(BeyondSearch.IWidget.Parameters parameters) {
            return base.Channel.GetWidget(parameters);
        }
        
        public System.Threading.Tasks.Task<string> GetWidgetAsync(BeyondSearch.IWidget.Parameters parameters) {
            return base.Channel.GetWidgetAsync(parameters);
        }
    }
}
