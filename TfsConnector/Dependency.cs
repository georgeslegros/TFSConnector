using System;

namespace TfsConnector
{
    public class Dependency : IEquatable<Dependency>
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string Project { get; set; }

        public bool Equals(Dependency other)
        {
            //return this.Id == other.Id && this.Version == other.Version;
            return GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var test = obj as Dependency;

            return test != null && Equals(test);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ (Version == null ? 0:Version.GetHashCode());
        }

        public override string ToString()
        {
            return Id + " -- " + Version;
        }
    }
}