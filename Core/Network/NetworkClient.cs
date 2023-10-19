using System;
using System.Net;
using System.Net.Sockets;

using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Core.Utilities;
using BetterBeatSaber.Shared.Network.Packets;

using JetBrains.Annotations;

using LiteNetLib;
using LiteNetLib.Utils;

namespace BetterBeatSaber.Core.Network; 

public sealed class NetworkClient : UnitySingleton<NetworkClient>, IDisposable {

    #region Injections

    [UsedImplicitly]
    internal static string Ip = null!;

    [UsedImplicitly]
    internal static ushort Port;
    
    [UsedImplicitly]
    internal static string Key = null!;
    
    #endregion
    
    #region Constants

    private const int MaxReconnectionAttempts = 5;
    
    #endregion

    private readonly BetterLogger _logger;
    
    private readonly EventBasedNetListener _listener;
    private readonly NetPacketProcessor _packetProcessor;
    private readonly NetManager _netManager;
    private NetPeer? _peer;

    private readonly NetDataWriter _dataWriter;

    public event Action? OnConnected;
    public event Action? OnDisconnected;

    private bool _shouldDisconnect;
    private int _reconnectionAttempts;

    public bool IsReconnecting { get; private set; }

    public IPEndPoint? EndPoint => _peer?.EndPoint;
    public bool IsConnected => _netManager.IsRunning && _peer?.ConnectionState == ConnectionState.Connected;
    public NetStatistics Statistics => _netManager.Statistics;
    public float? Ping => _peer?.Ping;
    public float? RTT => _peer?.RoundTripTime;

    public string? ServerName { get; private set; } = null;

    public NetworkClient() {

        _logger = BetterBeatSaber.Logger.GetChildLogger("Network");
        
        NetDebug.Logger = _logger;
        
        _listener = new EventBasedNetListener();
        _packetProcessor = new NetPacketProcessor();
        _netManager = new NetManager(_listener) {
            EnableStatistics = true,
            AutoRecycle = true
            #if DEBUG
            ,
            BroadcastReceiveEnabled = true,
            SimulateLatency = true,
            SimulationMinLatency = 1,
            SimulationMaxLatency = 50
            #endif
        };
        _dataWriter = new NetDataWriter();
        
        RegisterPacketHandler<AuthResponsePacket>(OnAuthResponsePacketReceived);
        
        AuthManager.Instance.OnAuthenticated += OnAuthenticated;
        
    }

    private void OnAuthenticated() =>
        Connect();

    #region Packet Handlers

    private void OnAuthResponsePacketReceived(AuthResponsePacket packet) {
        ServerName = packet.ServerName;
        if (packet.Success) {
            _logger.Info("Authenticated");
        } else {
            if(packet.Reason != null)
                _logger.Warn("Failed to authenticate ({Reason})", packet.Reason);
            else
                _logger.Warn("Failed to authenticate");
        }
    }
    
    #endregion
    
    #region Unity Event Functions

    public void Start() {
        _listener.NetworkReceiveEvent += OnNetworkReceiveEvent;
        _listener.PeerConnectedEvent += OnPeerConnectedEvent;
        _listener.PeerDisconnectedEvent += OnPeerDisconnectedEvent;
        _listener.NetworkErrorEvent += OnNetworkErrorEvent;
    }

    private void OnDestroy() {
        
        Disconnect();

        UnregisterPacketHandler<AuthResponsePacket>();
        
        _listener.NetworkReceiveEvent -= OnNetworkReceiveEvent;
        _listener.PeerConnectedEvent -= OnPeerConnectedEvent;
        _listener.PeerDisconnectedEvent -= OnPeerDisconnectedEvent;
        _listener.NetworkErrorEvent -= OnNetworkErrorEvent;

    }
    
    private void Update() {
        _netManager.PollEvents();
    }
    
    #endregion

    #region Event Handlers

    private void OnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod) {
        _packetProcessor.ReadAllPackets(reader);
    }
    
    private void OnPeerConnectedEvent(NetPeer peer) {
        
        _peer = peer;
        
        _logger.Info("Connected ");
        _logger.Info("Authenticating ...");
        
        SendPacket(new AuthPacket {
            Session = AuthManager.Instance.Token!
        });

        OnConnected?.Invoke();

    }

    private void OnPeerDisconnectedEvent(NetPeer _, DisconnectInfo disconnectInfo) {
        
        _peer = null;
        
        _logger.Info("Disconnected");
        
        if (!_shouldDisconnect && disconnectInfo.Reason != DisconnectReason.ConnectionRejected && _reconnectionAttempts < MaxReconnectionAttempts) {
            IsReconnecting = true;
            if (disconnectInfo.Reason != DisconnectReason.Reconnect) {
                _reconnectionAttempts++;
                _logger.Info("Trying to reconnect ...");
                Connect(true);
            }
        } else {
            IsReconnecting = false;
            _reconnectionAttempts = 0;
        }
        
        OnDisconnected?.Invoke();
        
        _shouldDisconnect = false;
        
    }
    
    private void OnNetworkErrorEvent(IPEndPoint _, SocketError error) {
        _logger.Warn("Network Error: {Error}", error.ToString());
    }
    
    #endregion

    #region Methods
    
    public void Connect(bool isReconnect = false) {

        if (!isReconnect && _netManager.IsRunning)
            return;
        
        _logger.Info("Connecting ...");
        
        _netManager.Start();
        _netManager.Connect(Ip, Port, Key);
        
    }

    public void Disconnect() {
        
        if (!IsConnected)
            return;

        _shouldDisconnect = true;
        
        _netManager.DisconnectAll();
        _netManager.Stop();
        
    }

    public void RegisterPacketHandler<T>(Action<T> packetHandler) where T : INetSerializable, new() {
        _packetProcessor.SubscribeNetSerializable(packetHandler);
    }

    public void UnregisterPacketHandler<T>() where T : INetSerializable {
        _packetProcessor.RemoveSubscription<T>();
    }
    
    public bool SendPacket<T>(T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered, bool queuePacketIfNotConnected = true) where T : INetSerializable {

        if (!IsConnected)
            return false;
        
        _dataWriter.Reset();
        _packetProcessor.WriteNetSerializable(_dataWriter, ref packet);
        _peer?.Send(_dataWriter, deliveryMethod);

        return true;

    }
    
    #endregion

    public void Dispose() {
        OnDestroy();
    }

}