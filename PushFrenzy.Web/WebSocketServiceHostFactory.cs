//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.ServiceModel;
//using System.ServiceModel.Activation;
//using Microsoft.ServiceModel.WebSockets;
//using System.ServiceModel.Channels;
//using System.ServiceModel.Description;
//using System.Threading.Tasks;

//namespace PushFrenzy.Web
//{
//    public class WebSocketServiceHostFactory : ServiceHostFactory
//    {
//        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
//        {
//            var host = new WebSocketHost(serviceType, 
//                new ServiceThrottlingBehavior { MaxConcurrentSessions = int.MaxValue, MaxConcurrentCalls = 20 }, baseAddresses);

//            var binding = WebSocketHost.CreateWebSocketBinding(https: false, sendBufferSize: 2048, receiveBufferSize: 2048);
//            binding.SendTimeout = TimeSpan.FromMilliseconds(500);            
//            binding.OpenTimeout = TimeSpan.FromDays(1);
//            host.AddWebSocketEndpoint(binding);
            
//            return host;
//        }        
//    }    

//}