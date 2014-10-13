﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 4.0.50826.0
// 
namespace DYcomWcfGameClient.poxy {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="poxy.IdyGameServer", CallbackContract=typeof(DYcomWcfGameClient.poxy.IdyGameServerCallback))]
    public interface IdyGameServer {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IdyGameServer/Send")]
        System.IAsyncResult BeginSend(byte[] data, System.AsyncCallback callback, object asyncState);
        
        void EndSend(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IdyGameServerCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IdyGameServer/onData")]
        void onData(byte[] data);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IdyGameServerChannel : DYcomWcfGameClient.poxy.IdyGameServer, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class IdyGameServerClient : System.ServiceModel.DuplexClientBase<DYcomWcfGameClient.poxy.IdyGameServer>, DYcomWcfGameClient.poxy.IdyGameServer {
        
        private BeginOperationDelegate onBeginSendDelegate;
        
        private EndOperationDelegate onEndSendDelegate;
        
        private System.Threading.SendOrPostCallback onSendCompletedDelegate;
        
        private bool useGeneratedCallback;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public IdyGameServerClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public IdyGameServerClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public IdyGameServerClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public IdyGameServerClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public IdyGameServerClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public IdyGameServerClient(string endpointConfigurationName) : 
                this(new IdyGameServerClientCallback(), endpointConfigurationName) {
        }
        
        private IdyGameServerClient(IdyGameServerClientCallback callbackImpl, string endpointConfigurationName) : 
                this(new System.ServiceModel.InstanceContext(callbackImpl), endpointConfigurationName) {
            useGeneratedCallback = true;
            callbackImpl.Initialize(this);
        }
        
        public IdyGameServerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                this(new IdyGameServerClientCallback(), binding, remoteAddress) {
        }
        
        private IdyGameServerClient(IdyGameServerClientCallback callbackImpl, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                this(new System.ServiceModel.InstanceContext(callbackImpl), binding, remoteAddress) {
            useGeneratedCallback = true;
            callbackImpl.Initialize(this);
        }
        
        public IdyGameServerClient() : 
                this(new IdyGameServerClientCallback()) {
        }
        
        private IdyGameServerClient(IdyGameServerClientCallback callbackImpl) : 
                this(new System.ServiceModel.InstanceContext(callbackImpl)) {
            useGeneratedCallback = true;
            callbackImpl.Initialize(this);
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> SendCompleted;
        
        public event System.EventHandler<onDataReceivedEventArgs> onDataReceived;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult DYcomWcfGameClient.poxy.IdyGameServer.BeginSend(byte[] data, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginSend(data, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        void DYcomWcfGameClient.poxy.IdyGameServer.EndSend(System.IAsyncResult result) {
            base.Channel.EndSend(result);
        }
        
        private System.IAsyncResult OnBeginSend(object[] inValues, System.AsyncCallback callback, object asyncState) {
            this.VerifyCallbackEvents();
            byte[] data = ((byte[])(inValues[0]));
            return ((DYcomWcfGameClient.poxy.IdyGameServer)(this)).BeginSend(data, callback, asyncState);
        }
        
        private object[] OnEndSend(System.IAsyncResult result) {
            ((DYcomWcfGameClient.poxy.IdyGameServer)(this)).EndSend(result);
            return null;
        }
        
        private void OnSendCompleted(object state) {
            if ((this.SendCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SendCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void SendAsync(byte[] data) {
            this.SendAsync(data, null);
        }
        
        public void SendAsync(byte[] data, object userState) {
            if ((this.onBeginSendDelegate == null)) {
                this.onBeginSendDelegate = new BeginOperationDelegate(this.OnBeginSend);
            }
            if ((this.onEndSendDelegate == null)) {
                this.onEndSendDelegate = new EndOperationDelegate(this.OnEndSend);
            }
            if ((this.onSendCompletedDelegate == null)) {
                this.onSendCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnSendCompleted);
            }
            base.InvokeAsync(this.onBeginSendDelegate, new object[] {
                        data}, this.onEndSendDelegate, this.onSendCompletedDelegate, userState);
        }
        
        private void OnonDataReceived(object state) {
            if ((this.onDataReceived != null)) {
                object[] results = ((object[])(state));
                this.onDataReceived(this, new onDataReceivedEventArgs(results, null, false, null));
            }
        }
        
        private void VerifyCallbackEvents() {
            if (((this.useGeneratedCallback != true) 
                        && (this.onDataReceived != null))) {
                throw new System.InvalidOperationException("Callback events cannot be used when the callback InstanceContext is specified. Pl" +
                        "ease choose between specifying the callback InstanceContext or subscribing to th" +
                        "e callback events.");
            }
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            this.VerifyCallbackEvents();
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override DYcomWcfGameClient.poxy.IdyGameServer CreateChannel() {
            return new IdyGameServerClientChannel(this);
        }
        
        private class IdyGameServerClientCallback : object, IdyGameServerCallback {
            
            private IdyGameServerClient proxy;
            
            public void Initialize(IdyGameServerClient proxy) {
                this.proxy = proxy;
            }
            
            public void onData(byte[] data) {
                this.proxy.OnonDataReceived(new object[] {
                            data});
            }
        }
        
        private class IdyGameServerClientChannel : ChannelBase<DYcomWcfGameClient.poxy.IdyGameServer>, DYcomWcfGameClient.poxy.IdyGameServer {
            
            public IdyGameServerClientChannel(System.ServiceModel.DuplexClientBase<DYcomWcfGameClient.poxy.IdyGameServer> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginSend(byte[] data, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = data;
                System.IAsyncResult _result = base.BeginInvoke("Send", _args, callback, asyncState);
                return _result;
            }
            
            public void EndSend(System.IAsyncResult result) {
                object[] _args = new object[0];
                base.EndInvoke("Send", _args, result);
            }
        }
    }
    
    public class onDataReceivedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public onDataReceivedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public byte[] data {
            get {
                base.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }
}
