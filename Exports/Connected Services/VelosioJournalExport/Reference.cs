﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     //
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Crossroads.Web.Common.Configuration;
using System;

namespace VelosioJournalExport
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="VelosioJournalExport.SendGLBatchSoap")]
    public interface SendGLBatchSoap
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/HelloWorld", ReplyAction="*")]
        System.Threading.Tasks.Task<VelosioJournalExport.HelloWorldResponse> HelloWorldAsync(VelosioJournalExport.HelloWorldRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/LoadBatch", ReplyAction="*")]
        System.Threading.Tasks.Task<VelosioJournalExport.LoadBatchResponse> LoadBatchAsync(VelosioJournalExport.LoadBatchRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class HelloWorldRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="HelloWorld", Namespace="http://tempuri.org/", Order=0)]
        public VelosioJournalExport.HelloWorldRequestBody Body;
        
        public HelloWorldRequest()
        {
        }
        
        public HelloWorldRequest(VelosioJournalExport.HelloWorldRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class HelloWorldRequestBody
    {
        
        public HelloWorldRequestBody()
        {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class HelloWorldResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="HelloWorldResponse", Namespace="http://tempuri.org/", Order=0)]
        public VelosioJournalExport.HelloWorldResponseBody Body;
        
        public HelloWorldResponse()
        {
        }
        
        public HelloWorldResponse(VelosioJournalExport.HelloWorldResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class HelloWorldResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string HelloWorldResult;
        
        public HelloWorldResponseBody()
        {
        }
        
        public HelloWorldResponseBody(string HelloWorldResult)
        {
            this.HelloWorldResult = HelloWorldResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class LoadBatchRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="LoadBatch", Namespace="http://tempuri.org/", Order=0)]
        public VelosioJournalExport.LoadBatchRequestBody Body;
        
        public LoadBatchRequest()
        {
        }
        
        public LoadBatchRequest(VelosioJournalExport.LoadBatchRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class LoadBatchRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string Token;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string BatchId;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=2)]
        public decimal TotalDebits;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=3)]
        public decimal TotalCredits;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=4)]
        public int NumTransactions;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string BatchData;
        
        public LoadBatchRequestBody()
        {
        }
        
        public LoadBatchRequestBody(string Token, string BatchId, decimal TotalDebits, decimal TotalCredits, int NumTransactions, string BatchData)
        {
            this.Token = Token;
            this.BatchId = BatchId;
            this.TotalDebits = TotalDebits;
            this.TotalCredits = TotalCredits;
            this.NumTransactions = NumTransactions;
            this.BatchData = BatchData;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class LoadBatchResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="LoadBatchResponse", Namespace="http://tempuri.org/", Order=0)]
        public VelosioJournalExport.LoadBatchResponseBody Body;
        
        public LoadBatchResponse()
        {
        }
        
        public LoadBatchResponse(VelosioJournalExport.LoadBatchResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class LoadBatchResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string LoadBatchResult;
        
        public LoadBatchResponseBody()
        {
        }
        
        public LoadBatchResponseBody(string LoadBatchResult)
        {
            this.LoadBatchResult = LoadBatchResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public interface SendGLBatchSoapChannel : VelosioJournalExport.SendGLBatchSoap, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public partial class SendGLBatchSoapClient : System.ServiceModel.ClientBase<VelosioJournalExport.SendGLBatchSoap>, VelosioJournalExport.SendGLBatchSoap
    {
        
    /// <summary>
    /// Implement this partial method to configure the service endpoint.
    /// </summary>
    /// <param name="serviceEndpoint">The endpoint to configure</param>
    /// <param name="clientCredentials">The client credentials</param>
    static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public SendGLBatchSoapClient(EndpointConfiguration endpointConfiguration, IConfigurationWrapper configurationWrapper) : 
                base(SendGLBatchSoapClient.GetBindingForEndpoint(endpointConfiguration), SendGLBatchSoapClient.GetEndpointAddress(endpointConfiguration, configurationWrapper))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public SendGLBatchSoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(SendGLBatchSoapClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public SendGLBatchSoapClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(SendGLBatchSoapClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public SendGLBatchSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<VelosioJournalExport.HelloWorldResponse> VelosioJournalExport.SendGLBatchSoap.HelloWorldAsync(VelosioJournalExport.HelloWorldRequest request)
        {
            return base.Channel.HelloWorldAsync(request);
        }
        
        public System.Threading.Tasks.Task<VelosioJournalExport.HelloWorldResponse> HelloWorldAsync()
        {
            VelosioJournalExport.HelloWorldRequest inValue = new VelosioJournalExport.HelloWorldRequest();
            inValue.Body = new VelosioJournalExport.HelloWorldRequestBody();
            return ((VelosioJournalExport.SendGLBatchSoap)(this)).HelloWorldAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<VelosioJournalExport.LoadBatchResponse> VelosioJournalExport.SendGLBatchSoap.LoadBatchAsync(VelosioJournalExport.LoadBatchRequest request)
        {
            return base.Channel.LoadBatchAsync(request);
        }
        
        public System.Threading.Tasks.Task<VelosioJournalExport.LoadBatchResponse> LoadBatchAsync(string Token, string BatchId, decimal TotalDebits, decimal TotalCredits, int NumTransactions, string BatchData)
        {
            VelosioJournalExport.LoadBatchRequest inValue = new VelosioJournalExport.LoadBatchRequest();
            inValue.Body = new VelosioJournalExport.LoadBatchRequestBody();
            inValue.Body.Token = Token;
            inValue.Body.BatchId = BatchId;
            inValue.Body.TotalDebits = TotalDebits;
            inValue.Body.TotalCredits = TotalCredits;
            inValue.Body.NumTransactions = NumTransactions;
            inValue.Body.BatchData = BatchData;
            return ((VelosioJournalExport.SendGLBatchSoap)(this)).LoadBatchAsync(inValue);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.SendGLBatchSoap))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                result.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.SendGLBatchSoap12))
            {
                System.ServiceModel.Channels.CustomBinding result = new System.ServiceModel.Channels.CustomBinding();
                System.ServiceModel.Channels.TextMessageEncodingBindingElement textBindingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement();
                textBindingElement.MessageVersion = System.ServiceModel.Channels.MessageVersion.CreateVersion(System.ServiceModel.EnvelopeVersion.Soap12, System.ServiceModel.Channels.AddressingVersion.None);
                result.Elements.Add(textBindingElement);
                System.ServiceModel.Channels.HttpsTransportBindingElement httpsBindingElement = new System.ServiceModel.Channels.HttpsTransportBindingElement();
                httpsBindingElement.AllowCookies = true;
                httpsBindingElement.MaxBufferSize = int.MaxValue;
                httpsBindingElement.MaxReceivedMessageSize = int.MaxValue;
                result.Elements.Add(httpsBindingElement);
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration, IConfigurationWrapper configWrapper)
        {
            var url = Environment.GetEnvironmentVariable("VELOSIO_EXPORT_URL");

            if ((endpointConfiguration == EndpointConfiguration.SendGLBatchSoap))
            {
                return new System.ServiceModel.EndpointAddress(url);
            }
            if ((endpointConfiguration == EndpointConfiguration.SendGLBatchSoap12))
            {
                return new System.ServiceModel.EndpointAddress(url);
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        public enum EndpointConfiguration
        {
            
            SendGLBatchSoap,
            
            SendGLBatchSoap12,
        }
    }
}
