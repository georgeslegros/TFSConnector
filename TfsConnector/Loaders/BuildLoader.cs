using System.Collections.Generic;
using Microsoft.TeamFoundation.Build.Client;

namespace TfsConnector.Loaders
{
    public class BuildLoader : ThreadedCollectionLoader<IBuildDefinition>
    {
        private readonly IBuildServer buildServer;
        private readonly string projectName;

        public BuildLoader(IBuildServer buildServer, string projectName)
        {
            this.buildServer = buildServer;
            this.projectName = projectName;
        }

        protected override IEnumerable<IBuildDefinition> LoadData()
        {
            return buildServer.QueryBuildDefinitions(projectName);
        }
    }
}