using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit;
using Res = Caredev.MegoTools.Properties.Resources;

namespace Caredev.MegoTools.Core
{
    public class ConnectionInfo
    {
        public string ConnectionString { get; set; }

        public string ProviderName { get; set; }

        public string Title { get; set; }

        private static HashSet<ConnectionInfo> _Connections = new HashSet<ConnectionInfo>();
        private static System.IO.FileInfo _FileInfo;

        public static IEnumerable<ConnectionInfo> Data => _Connections;

        static ConnectionInfo()
        {
            var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var path = System.IO.Path.Combine(root, "Caredev", "MegoTools", "Connections.xml");
            _FileInfo = new System.IO.FileInfo(path);
            Reload();
        }

        public static void Reload()
        {
            try
            {
                if (_FileInfo.Exists)
                {
                    var serializer = new XmlSerializer(typeof(ConnectionInfo[]));
                    using (var stream = _FileInfo.OpenRead())
                    {
                        var data = (ConnectionInfo[])serializer.Deserialize(stream);
                        _Connections = new HashSet<ConnectionInfo>(data);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Res.ConnectionInfo_ReloadError, ex.Message));
            }
        }

        public static void Save()
        {
            try
            {
                if (!_FileInfo.Directory.Exists)
                {
                    _FileInfo.Directory.Create();
                }
                XmlSerializer serializer = new XmlSerializer(typeof(ConnectionInfo[]));
                using (var stream = _FileInfo.Open(System.IO.FileMode.OpenOrCreate))
                {
                    serializer.Serialize(stream, _Connections.ToArray());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Res.ConnectionInfo_SaveError, ex.Message));
            }
        }

        public static void Add(ConnectionInfo connectionInfo)
        {
            _Connections.Add(connectionInfo);
        }

        public static void Remove(ConnectionInfo connectionInfo)
        {
            _Connections.Remove(connectionInfo);
        }
    }
}
