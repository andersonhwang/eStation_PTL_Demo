﻿namespace eStation_PTL_Demo.Model
{
    /// <summary>
    /// Connect information
    /// </summary>
    internal class ConnInfo:BaseModel
    {
        private int _port = 9071;
        private int _connType = 0;
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private bool _securtiy = false;
        private string _certificate = string.Empty;
        private string _token = string.Empty;
        /// <summary>
        /// Port
        /// </summary>
        public int Port { get => _port; set { _port = value; NotifyPropertyChanged(nameof(Port)); } }
        /// <summary>
        /// Connect type: 0-MQTT, 1-WebSocket
        /// </summary>
        public int ConnType { get => _connType; set { _connType = value; NotifyPropertyChanged(nameof(ConnType)); } }
        /// <summary>
        /// User name, default is test
        /// </summary>
        public string UserName { get => _userName; set { _userName = value; NotifyPropertyChanged(nameof(UserName)); } }
        /// <summary>
        /// Password, default is 123456
        /// </summary>
        public string Password { get => _password; set { _password = value; NotifyPropertyChanged(nameof(Password)); } }
        /// <summary>
        /// Security, default is true
        /// </summary>
        public bool Security { get => _securtiy; set { _securtiy = value; NotifyPropertyChanged(nameof(Security)); } }
        /// <summary>
        /// Certificate path
        /// </summary>
        public string Certificate { get => _certificate; set { _certificate = value; NotifyPropertyChanged(nameof(Certificate)); } }
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get => _token; set { _token = value; NotifyPropertyChanged(nameof(Token)); } }
    }
}
