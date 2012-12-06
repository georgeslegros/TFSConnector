using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using TfsConnector.Loaders;

namespace TfsConnector
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly TfsTeamProjectCollection server;
        private readonly WorkItemStore workItemStore;
        private readonly VersionControlServer versionControl;
        private readonly IBuildServer buildServer;
        private readonly HistoryLoader historyLoader;

        private ObservableCollection<Project> projects;
        public ObservableCollection<Project> Projects
        {
            get { return projects; }
            set { Set(ref projects, value, () => Projects); }
        }

        private Project selectedProject;
        public Project SelectedProject
        {
            get { return selectedProject; }
            set
            {
                Set(ref selectedProject, value, () => SelectedProject);
                OnSelectedProjectChanged();
            }
        }

        private ObservableCollection<Dependency> dependencies;
        public ObservableCollection<Dependency> Dependencies
        {
            get { return dependencies; }
            set { Set(ref dependencies, value, () => Dependencies); }
        }

        private ObservableCollection<IBuildDefinition> buildDefinitions;
        public ObservableCollection<IBuildDefinition> BuildDefinitions
        {
            get { return buildDefinitions; }
            set { Set(ref buildDefinitions, value, () => BuildDefinitions); }
        }

        private ObservableCollection<TeamMember> teamMembers;
        public ObservableCollection<TeamMember> TeamMembers
        {
            get { return teamMembers; }
            set { Set(ref teamMembers, value, () => TeamMembers); }
        }

        private ICommand loadHistoryCommand;
        public ICommand LoadHistoryCommand
        {
            get { return loadHistoryCommand; }
            set { Set(ref loadHistoryCommand, value, () => LoadHistoryCommand); }
        }

        private ObservableCollection<Changeset> history;
        public ObservableCollection<Changeset> History
        {
            get { return history; }
            set { Set(ref history, value, () => History); }
        }

        private ObservableCollection<PanoramaGroup> panoramaGroups;
        public ObservableCollection<PanoramaGroup> PanoramaGroups
        {
            get { return panoramaGroups; }
            set { Set(ref panoramaGroups, value, () => PanoramaGroups); }
        }


        private void OnSelectedProjectChanged()
        {
            if (SelectedProject != null)
            {
                var dependenciesLoader = new DependenciesLoader(versionControl, SelectedProject);
                dependenciesLoader.Load(OnDependenciesLoaded);

                var buildLoader = new BuildLoader(buildServer, SelectedProject.Name);
                buildLoader.Load(OnBuildsLoaded);

                var teamMemnberLoader = new TeamMemberLoader(server, SelectedProject);
                teamMemnberLoader.Load(OnTeamLoaded);
            }
        }

        private void OnTeamLoaded(IEnumerable<TeamMember> obj)
        {
            TeamMembers = new ObservableCollection<TeamMember>(obj);
        }

        private void OnBuildsLoaded(IEnumerable<IBuildDefinition> obj)
        {
            BuildDefinitions = new ObservableCollection<IBuildDefinition>(obj);
        }

        private void OnDependenciesLoaded(IEnumerable<Dependency> obj)
        {
            Dependencies = new ObservableCollection<Dependency>(obj);
        }

        private void OnHistoryLoaded(IEnumerable<Changeset> obj)
        {
            History = new ObservableCollection<Changeset>(obj);
        }

        public MainWindowViewModel()
        {
            server = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(
                RegisteredTfsConnections.GetProjectCollections().First().Uri);
            workItemStore = (WorkItemStore)server.GetService(typeof(WorkItemStore));
            versionControl = server.GetService<VersionControlServer>();
            buildServer = (IBuildServer)server.GetService(typeof(IBuildServer));
            historyLoader = new HistoryLoader(versionControl);
            loadHistoryCommand = new DelegateCommand(LoadHistory);
        }

        public void LoadHistory()
        {
            historyLoader.Load(OnHistoryLoaded);
        }

        public void Init()
        {
            ProjectListLoader projectListLoader = new ProjectListLoader(workItemStore);
            projectListLoader.Load(OnProjectLoaded);
        }

        private void OnProjectLoaded(IEnumerable<Project> obj)
        {
            Projects = new ObservableCollection<Project>(obj);
            PanoramaGroups = new ObservableCollection<PanoramaGroup>();
            PanoramaGroup group = new PanoramaGroup("Projects");
         

            PanoramaGroups.Add(group);  
            group.SetSource(Projects.Select(p=>
                                                {
                                                    return new ProjectWrapper {Project = p};
                                                }));
        }
    }

    public class ProjectWrapper
    {
        public Project Project { get; set; }
        private ICommand ClickCommand { get; set; }

        public ProjectWrapper()
        {
            ClickCommand = new DelegateCommand(OnClick);
        }

        private void OnClick()
        {
            
        }
    }
}