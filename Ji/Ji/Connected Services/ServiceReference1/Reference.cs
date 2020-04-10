﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторного создания кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ServiceReference1
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Obj", Namespace="http://schemas.datacontract.org/2004/07/JiClass")]
    public partial class Obj : object
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="User", Namespace="http://schemas.datacontract.org/2004/07/JiClass")]
    public partial class User : object
    {
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.IChatLevelService")]
    public interface IChatLevelService
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChatLevelService/About", ReplyAction="http://tempuri.org/IChatLevelService/AboutResponse")]
        string About();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChatLevelService/About", ReplyAction="http://tempuri.org/IChatLevelService/AboutResponse")]
        System.Threading.Tasks.Task<string> AboutAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChatLevelService/Dellay", ReplyAction="http://tempuri.org/IChatLevelService/DellayResponse")]
        void Dellay(int ms);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChatLevelService/Dellay", ReplyAction="http://tempuri.org/IChatLevelService/DellayResponse")]
        System.Threading.Tasks.Task DellayAsync(int ms);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChatLevelService/Chat_GetList", ReplyAction="http://tempuri.org/IChatLevelService/Chat_GetListResponse")]
        ServiceReference1.Obj[] Chat_GetList(string tiket, long nMsg, long ParentObjId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChatLevelService/Chat_GetList", ReplyAction="http://tempuri.org/IChatLevelService/Chat_GetListResponse")]
        System.Threading.Tasks.Task<ServiceReference1.Obj[]> Chat_GetListAsync(string tiket, long nMsg, long ParentObjId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChatLevelService/Chat_Get", ReplyAction="http://tempuri.org/IChatLevelService/Chat_GetResponse")]
        ServiceReference1.Obj[] Chat_Get(string tiket, long nmessage, long ChatId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChatLevelService/Chat_Get", ReplyAction="http://tempuri.org/IChatLevelService/Chat_GetResponse")]
        System.Threading.Tasks.Task<ServiceReference1.Obj[]> Chat_GetAsync(string tiket, long nmessage, long ChatId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChatLevelService/Chat_GetUsers", ReplyAction="http://tempuri.org/IChatLevelService/Chat_GetUsersResponse")]
        ServiceReference1.User[] Chat_GetUsers(long tiket, long nmessage, int ChatId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IChatLevelService/Chat_GetUsers", ReplyAction="http://tempuri.org/IChatLevelService/Chat_GetUsersResponse")]
        System.Threading.Tasks.Task<ServiceReference1.User[]> Chat_GetUsersAsync(long tiket, long nmessage, int ChatId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public interface IChatLevelServiceChannel : ServiceReference1.IChatLevelService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public partial class ChatLevelServiceClient : System.ServiceModel.ClientBase<ServiceReference1.IChatLevelService>, ServiceReference1.IChatLevelService
    {
        
        /// <summary>
        /// Реализуйте этот разделяемый метод для настройки конечной точки службы.
        /// </summary>
        /// <param name="serviceEndpoint">Настраиваемая конечная точка</param>
        /// <param name="clientCredentials">Учетные данные клиента.</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public ChatLevelServiceClient() : 
                base(ChatLevelServiceClient.GetDefaultBinding(), ChatLevelServiceClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.BasicHttpBinding.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ChatLevelServiceClient(EndpointConfiguration endpointConfiguration) : 
                base(ChatLevelServiceClient.GetBindingForEndpoint(endpointConfiguration), ChatLevelServiceClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ChatLevelServiceClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(ChatLevelServiceClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ChatLevelServiceClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(ChatLevelServiceClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ChatLevelServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public string About()
        {
            return base.Channel.About();
        }
        
        public System.Threading.Tasks.Task<string> AboutAsync()
        {
            return base.Channel.AboutAsync();
        }
        
        public void Dellay(int ms)
        {
            base.Channel.Dellay(ms);
        }
        
        public System.Threading.Tasks.Task DellayAsync(int ms)
        {
            return base.Channel.DellayAsync(ms);
        }
        
        public ServiceReference1.Obj[] Chat_GetList(string tiket, long nMsg, long ParentObjId)
        {
            return base.Channel.Chat_GetList(tiket, nMsg, ParentObjId);
        }
        
        public System.Threading.Tasks.Task<ServiceReference1.Obj[]> Chat_GetListAsync(string tiket, long nMsg, long ParentObjId)
        {
            return base.Channel.Chat_GetListAsync(tiket, nMsg, ParentObjId);
        }
        
        public ServiceReference1.Obj[] Chat_Get(string tiket, long nmessage, long ChatId)
        {
            return base.Channel.Chat_Get(tiket, nmessage, ChatId);
        }
        
        public System.Threading.Tasks.Task<ServiceReference1.Obj[]> Chat_GetAsync(string tiket, long nmessage, long ChatId)
        {
            return base.Channel.Chat_GetAsync(tiket, nmessage, ChatId);
        }
        
        public ServiceReference1.User[] Chat_GetUsers(long tiket, long nmessage, int ChatId)
        {
            return base.Channel.Chat_GetUsers(tiket, nmessage, ChatId);
        }
        
        public System.Threading.Tasks.Task<ServiceReference1.User[]> Chat_GetUsersAsync(long tiket, long nmessage, int ChatId)
        {
            return base.Channel.Chat_GetUsersAsync(tiket, nmessage, ChatId);
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
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Не удалось найти конечную точку с именем \"{0}\".", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding))
            {
                return new System.ServiceModel.EndpointAddress("http://194.190.100.194/XSignalR/ChatLvl.svc");
            }
            throw new System.InvalidOperationException(string.Format("Не удалось найти конечную точку с именем \"{0}\".", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return ChatLevelServiceClient.GetBindingForEndpoint(EndpointConfiguration.BasicHttpBinding);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return ChatLevelServiceClient.GetEndpointAddress(EndpointConfiguration.BasicHttpBinding);
        }
        
        public enum EndpointConfiguration
        {
            
            BasicHttpBinding,
        }
    }
}