using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TfsConnector.Loaders
{
    public class TeamMemberLoader : ThreadedCollectionLoader<TeamMember>
    {
        private readonly TfsTeamProjectCollection server;
        private readonly Project project;

        public TeamMemberLoader(TfsTeamProjectCollection server, Project project)
        {
            this.server = server;
            this.project = project;
        }

        protected override IEnumerable<TeamMember> LoadData()
        {
            var gss = server.GetService<IGroupSecurityService>();

            Identity SIDS = gss.ReadIdentity(SearchFactor.EveryoneApplicationGroup, null, QueryMembership.Expanded);
            Identity[] GroupIds = gss.ReadIdentities(SearchFactor.Sid, SIDS.Members, QueryMembership.None);

            var Groups = GroupIds.Where(u => u.Domain == project.Uri.ToString()).ToArray();

            List<TeamMember> members = new List<TeamMember>();

            foreach (var Group in Groups)
            {
                Identity SubSIDS = gss.ReadIdentity(SearchFactor.Sid,
                Group.Sid,
                QueryMembership.Expanded);

                if (SubSIDS.Members.Length == 0)
                {
                    continue;
                }

                Identity[] MemberIds = gss.ReadIdentities(SearchFactor.Sid, SubSIDS.Members, QueryMembership.None);

                var Members = MemberIds.Where(u => !u.SecurityGroup).ToArray();
                foreach (var member in Members)
                {
                    members.Add(new TeamMember{DisplayName = member.DisplayName,Description = member.Description, Group = Group.DisplayName});

                    //if (member.AccountName != "Kristof Mattei") 
                        continue;

                    GetSubscriptions(member);
                }
            }

            return members;
        }

        private void GetSubscriptions(Identity member)
        {
            var ims = server.GetService<IIdentityManagementService>();
            // Read out the identity of the user we want to impersonate
            TeamFoundationIdentity identity = ims.ReadIdentity(IdentitySearchFactor.AccountName,
                member.AccountName,
                MembershipQuery.None,
                ReadIdentityOptions.None);

            var tfs_impersonated = new TfsTeamProjectCollection(RegisteredTfsConnections.GetProjectCollections().First().Uri, identity.Descriptor);

            var eventService = (IEventService)tfs_impersonated.GetService(typeof(IEventService));

            try
            {
                var events = eventService.GetEventSubscriptions(member.Domain + @"\" + member.AccountName);
                foreach (Subscription subscription in events)
                {
                    Debug.WriteLine(subscription.ID);
                    Debug.WriteLine(subscription.ConditionString);
                    Debug.WriteLine(subscription.DeliveryPreference.Type);
                    Debug.WriteLine(subscription.Device);
                    Debug.WriteLine(subscription.Subscriber);
                    Debug.WriteLine(subscription.Tag);
                }
            }
            catch (System.Exception)
            {
                //Swalow the security exception
                //TODO: catch the correct exception
            }
        }

        protected override void PostBindingAction(Selector selector)
        {
            if (selector.ItemsSource == null)
            {
                return;
            }

            CollectionView myView = (CollectionView)CollectionViewSource.GetDefaultView(selector.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Group");
            myView.GroupDescriptions.Add(groupDescription);
        }
    }
    public class TeamMember
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
    }
    
}