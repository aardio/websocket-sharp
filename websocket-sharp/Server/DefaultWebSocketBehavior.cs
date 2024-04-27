using System;
using System.Reflection;
using System.Collections;
using System.IO;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;
using WebSocketSharp.Server;
using System.Collections.Specialized;
using System.Security.Principal;

namespace WebSocketSharp.Server
{
    public class DynObject : IEnumerable
    {
        private object target;
        private Type type;
        public DynObject(object aardioObject) { target = aardioObject; type = target.GetType(); }
        IEnumerator IEnumerable.GetEnumerator() { return (target as IEnumerable).GetEnumerator(); }

        public object InvokeMember(string method, params object[] args) { return type.InvokeMember(method, BindingFlags.InvokeMethod, null, target, args); }
        public object InvokeMember(int dispId, params object[] args) { return type.InvokeMember("[DispId=" + dispId + "]", BindingFlags.InvokeMethod, null, target, args); }
        public object Invoke(params object[] args) { return type.InvokeMember("", BindingFlags.InvokeMethod, null, target, args); }

        public object this[string index]
        {
            get { return type.InvokeMember(index, BindingFlags.GetProperty, null, target, null); }
            set { type.InvokeMember(index, BindingFlags.SetProperty, null, target, new object[] { value }); }
        }
    }


    public class DefaultWebSocketBehavior : WebSocketBehavior
    {

        public delegate object GetService();


        public static void AddWebSocketService(HttpServer wssrv, string path, GetService getService)
        {
            wssrv.AddWebSocketService<DefaultWebSocketBehavior>(path, (DefaultWebSocketBehavior behavior) =>
            {
                object aardioObject = getService();
                behavior.InitDynObject(aardioObject);
            });
        }

        public static void AddWebSocketService(WebSocketServer wssrv, string path, GetService getService)
        {
            wssrv.AddWebSocketService<DefaultWebSocketBehavior>(path, (DefaultWebSocketBehavior behavior) =>
            {
                object aardioObject = getService();
                behavior.InitDynObject(aardioObject);
            });
        }


        private DynObject aardioDynObject;
        public void InitDynObject(object aardioObject)
        {
            aardioDynObject = new DynObject(aardioObject);
            aardioDynObject.InvokeMember("onInit___~~", this);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            aardioDynObject.InvokeMember("onMessage", e);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            aardioDynObject.InvokeMember("onError", e);
        }

        protected override void OnOpen()
        {
            aardioDynObject.InvokeMember("onOpen");
        }

        protected override void OnClose(CloseEventArgs e)
        {
            aardioDynObject.InvokeMember("onClose", e);
        }

        public new void Send(byte[] data)
        {
            base.Send(data);
        }

        public new void Send(FileInfo file)
        {
            base.Send(file);
        }

        public new void Send(string data)
        {
            base.Send(data);
        }

        public new void SendAsync(byte[] data, Action<bool> completed)
        {
            base.SendAsync(data, completed);
        }

        public new void SendAsync(FileInfo file, Action<bool> completed)
        {
            base.SendAsync(file, completed);
        }


        public new void SendAsync(string data, Action<bool> completed)
        {
            base.SendAsync(data, completed);
        }

        public new void SendAsync(Stream stream, int length, Action<bool> completed)
        {
            base.SendAsync(stream, length, completed);
        }

        public new NameValueCollection Headers
        {
            get
            {
                return base.Headers;
            }
        }

        public new NameValueCollection QueryString
        {
            get
            {
                return base.QueryString;
            }
        }

        public new WebSocketState ReadyState
        {
            get
            {
                return base.ReadyState;
            }
        }

        public new IPrincipal User
        {
            get
            {
                return base.User;
            }
        }

        public new System.Net.IPEndPoint UserEndPoint
        {
            get
            {
                return base.UserEndPoint;
            }
        }

        public new WebSocketSessionManager Sessions
        {
            get
            {
                return base.Sessions;
            }
        }

        public new bool IsAlive
        {
            get
            {
                return base.IsAlive;
            }
        }
    }
}