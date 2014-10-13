using System;
using System.Text;
using System.ServiceModel;
using System.IO;
using System.ServiceModel.Web;
using System.ServiceModel.Description;

namespace DYCom_Net4
{
    [ServiceContract]
    interface ITcpPolicyRetriever
    {
        [OperationContract, WebGet(UriTemplate = "/clientaccesspolicy.xml")]
        Stream GetSilverlightPolicy();
    }

    class policyServer : ITcpPolicyRetriever
    {
        public string policyString = @"<?xml version=""1.0"" encoding =""utf-8""?><access-policy><cross-domain-access><policy><allow-from><domain uri=""*"" /></allow-from><grant-to><socket-resource port=""4502-4534"" protocol=""tcp"" /></grant-to></policy></cross-domain-access></access-policy>";
        public Stream GetSilverlightPolicy()
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
            return new MemoryStream(Encoding.UTF8.GetBytes(policyString));
        }
    }

    public class DYWcfPolicy
    {
        ServiceHost host;

        public string StartPolicy(Uri policyUrl, string PolicyString)
        {
            try
            {
                if (host == null)
                {
                    host = new ServiceHost(typeof(policyServer), policyUrl);
                    host.AddServiceEndpoint(typeof(ITcpPolicyRetriever), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
                    host.Open();
                    ((policyServer)OperationContext.Current.InstanceContext.GetServiceInstance()).policyString = PolicyString;
                }
                else if (host.State != CommunicationState.Opened)
                {
                    host.Close();
                    host.Open();
                }
                return "DYCOM's WCF.net.tcp cross domain running";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string StartPolicy(Uri policyUrl)
        {
            try
            {
                if (host == null)
                {
                    host = new ServiceHost(typeof(policyServer), policyUrl);
                    host.AddServiceEndpoint(typeof(ITcpPolicyRetriever), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());
                    host.Open();
                }
                else if (host.State != CommunicationState.Opened)
                {
                    host.Close();
                    host.Open();
                }
                return "DYCOM's WCF.net.tcp cross domain running";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void StopPolicy()
        {
            host.Close();
        }

    }
}
