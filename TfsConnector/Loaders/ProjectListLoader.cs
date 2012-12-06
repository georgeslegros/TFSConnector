using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TfsConnector.Loaders
{
    public class ProjectListLoader : ThreadedCollectionLoader<Project>
    {
        private readonly WorkItemStore workItemStore;

        public ProjectListLoader(WorkItemStore workItemStore)
        {
            this.workItemStore = workItemStore;
        }

        protected override IEnumerable<Project> LoadData()
        {
            var projects = workItemStore.Projects.Cast<Project>();
            return projects.ToList();
        }
    }
}