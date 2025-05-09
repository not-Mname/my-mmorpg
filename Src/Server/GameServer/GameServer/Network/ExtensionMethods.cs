using System;

using System.Net.Sockets;

namespace GameServer.Network
{
    
    public delegate Boolean SocketAsyncMethod(SocketAsyncEventArgs args);

    
    public static class ExtensionMethods
    {
        
        public static void InvokeAsyncMethod(this Socket socket, SocketAsyncMethod method, EventHandler<SocketAsyncEventArgs> callback, SocketAsyncEventArgs args)
        {
            if (!method(args))
            {
                callback(socket, args);
            }
        }
    }
}