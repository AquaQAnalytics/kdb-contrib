using kx;

namespace KdbConnections
{
    public static class DBConnection
    {
        private static c _connection;
        public static c Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        public static bool ConnectToKDB(string host, int port)
        {
            bool connected = false;

            _connection = new c(host, port);

            if (_connection.Connected)
            {
                connected = true;
            }

            return connected;
        }
    }
}
