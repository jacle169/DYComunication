using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace DYCom_Net4
{
    public class DYWcfServer
    {
        ServiceHost host;

        public string StartWcfServer(Binding binding, Type ServerClassType, Type ServerInterfaceType, string ServerAddress, string ServerMexAddress)
        {
            try
            {
                if (host == null)
                {
                    host = new ServiceHost(ServerClassType);
                    host.AddServiceEndpoint(ServerInterfaceType, binding, ServerAddress);

                    ServiceMetadataBehavior me = new ServiceMetadataBehavior();
                    me.HttpGetEnabled = true;
                    me.HttpGetUrl = new Uri(ServerMexAddress);
                    host.Description.Behaviors.Add(me);

                    host.Open();
                }
                else if (host.State != CommunicationState.Opened)
                {
                    host.Close();
                    host.Open();
                }
                return "Server opened";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string StartWcfServer(Binding binding, Type ServerClassType, Type ServerInterfaceType, string ServerAddress)
        {
            try
            {
                if (host == null)
                {
                    host = new ServiceHost(ServerClassType);
                    host.AddServiceEndpoint(ServerInterfaceType, binding, ServerAddress);
                    host.Open();
                }
                else if (host.State != CommunicationState.Opened)
                {
                    host.Close();
                    host.Open();
                }
                return "Server opened";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void Stop()
        {
            host.Close();
        }
    }
}
