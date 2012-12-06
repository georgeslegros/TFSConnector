using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TfsConnector.Loaders
{
    public class DependenciesLoader : ThreadedCollectionLoader<Dependency>
    {
        private readonly VersionControlServer versionControl;
        private readonly Project project;

        public DependenciesLoader(VersionControlServer versionControl, Project project)
        {
            this.versionControl = versionControl;
            this.project = project;
        }

        protected override IEnumerable<Dependency> LoadData()
        {
            string projectName = "$/" + project.Name;
            string branchPath = "/Dev";
            string packageFolderPath = "/packages";

            var dependencies = new List<Dependency>();

            Item repositoriesFile;
            if (!TryGetItem(projectName + branchPath + packageFolderPath + "/repositories.config", out repositoriesFile))
            {
                return new List<Dependency> { new Dependency { Id = string.Format("The project {0} seem to not rely on Nuget or the targetted branch does not exist", project.Name) } };
            }
            else
            {

                var document = repositoriesFile.Read();

                foreach (var path in document.Descendants(XName.Get("repository")).Select(repository => repository.Attribute(XName.Get("path"))))
                {
                    Item repositoryFile;
                    if (TryGetItem(projectName + branchPath + packageFolderPath + "/" + path.Value, out repositoryFile))
                        ReadRepository(path, repositoryFile, dependencies);
                    else
                        dependencies.Add(new Dependency { Id = "File not found", Version = path.Value });
                }

                return dependencies.Distinct();
                //TODO
                //Filter();
            }
        }



        private static void ReadRepository(XAttribute path, Item repositoryFile, List<Dependency> dependencies)
        {

            var repositoryDocument = repositoryFile.Read();

            dependencies.AddRange(from package in repositoryDocument.Descendants(XName.Get("package"))
                                  let packageId = package.Attribute(XName.Get("id"))
                                  let packageVersion = package.Attribute(XName.Get("version"))
                                  select new Dependency
                                             {
                                                 Id = packageId.Value,
                                                 Version = packageVersion.Value,
                                                 Project = path.Value.Split('\\')[1]
                                             });
        }

        private bool TryGetItem(string path, out Item item)
        {
            var itemExists = versionControl.ServerItemExists(path, VersionSpec.Latest, DeletedState.NonDeleted, ItemType.Any);
            if (itemExists)
            {
                item = versionControl.GetItem(path, VersionSpec.Latest);
                return true;
            }
            item = null;
            return false;
        }
    }
}