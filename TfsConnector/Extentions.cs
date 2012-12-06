using System.Xml;
using System.Xml.Linq;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TfsConnector
{
    public static class Extentions
    {
        public static XDocument Read(this Item item)
        {
            using (XmlReader repositoryReader = new XmlTextReader(item.DownloadFile()))
            {
                return XDocument.Load(repositoryReader);
            }
        }
    }
}