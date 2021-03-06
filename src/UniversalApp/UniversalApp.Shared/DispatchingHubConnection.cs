﻿using System;
using System.Threading.Tasks;
using System.Windows;
#if SILVERLIGHT
using System.Windows.Threading;
#else
using Dispatcher = Windows.UI.Core.CoreDispatcher;
#endif
using Windows.UI.Core;

namespace Microsoft.AspNet.SignalR.Client
{

    /// <summary>
#if SILVERLIGHT
    /// A HubConnection that invokes callbacks via a <see cref="System.Windows.Threading.Dispatcher"/>.
#else
    /// A HubConnection that invokes callbacks via a <see cref="Windows.UI.Core.CoreDispatcher"/>.
#endif
    /// </summary>
    public class DispatchingHubConnection : HubConnection
    {
        private readonly Dispatcher _dispatcher;
        private Action<StateChange> _stateChanged;
        private Action _connectionSlow;
        private Action _reconnecting;
        private Action _reconnected;
        private Action _closed;
        private Action<Exception> _error;

        public DispatchingHubConnection(string url, Dispatcher dispatcher)
            : base(url)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            _dispatcher = dispatcher;
            base.StateChanged += HubConnection_StateChanged;
            base.ConnectionSlow += HubConnection_ConnectionSlow;
            base.Reconnecting += HubConnection_Reconnecting;
            base.Reconnected += HubConnection_Reconnected;
            base.Closed += HubConnection_Closed;
            base.Error += HubConnection_Error;
        }

        public new event Action<StateChange> StateChanged
        {
            add { _stateChanged = (Action<StateChange>)Action.Combine(_stateChanged, value); }
            remove { Action.Remove(_stateChanged, value); }
        }

        private void HubConnection_StateChanged(StateChange stateChange)
        {
            if (_stateChanged != null)
            {
                _stateChanged.Dispatch(stateChange, _dispatcher);
            }
        }

        public new event Action ConnectionSlow
        {
            add { _connectionSlow = (Action)Action.Combine(_connectionSlow, value); }
            remove { Action.Remove(_connectionSlow, value); }
        }

        private void HubConnection_ConnectionSlow()
        {
            if (_connectionSlow != null)
            {
                _connectionSlow.Dispatch(_dispatcher);
            }
        }

        public new event Action Reconnecting
        {
            add { _reconnecting = (Action)Action.Combine(_reconnecting, value); }
            remove { Action.Remove(_reconnecting, value); }
        }

        private void HubConnection_Reconnecting()
        {
            if (_reconnecting != null)
            {
                _reconnecting.Dispatch(_dispatcher);
            }
        }

        public new event Action Reconnected
        {
            add { _reconnected = (Action)Action.Combine(_reconnected, value); }
            remove { Action.Remove(_reconnected, value); }
        }

        private void HubConnection_Reconnected()
        {
            if (_reconnected != null)
            {
                _reconnected.Dispatch(_dispatcher);
            }
        }

        public new event Action Closed
        {
            add { _closed = (Action)Action.Combine(_closed, value); }
            remove { Action.Remove(_closed, value); }
        }

        private void HubConnection_Closed()
        {
            if (_closed != null)
            {
                _closed.Dispatch(_dispatcher);
            }
        }

        public new event Action<Exception> Error
        {
            add { _error = (Action<Exception>)Action.Combine(_error, value); }
            remove { Action.Remove(_error, value); }
        }

        private void HubConnection_Error(Exception ex)
        {
            if (_error != null)
            {
                _error.Dispatch(ex, _dispatcher);
            }
        }

        public new DispatchingHubProxy CreateHubProxy(string name)
        {
            return new DispatchingHubProxy(base.CreateHubProxy(name), _dispatcher);
        }
    }
}
