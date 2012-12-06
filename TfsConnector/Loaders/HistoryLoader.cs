using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TfsConnector.Loaders
{
    public class HistoryLoader : ThreadedCollectionLoader<Changeset>
    {
        private readonly VersionControlServer versionControl;

        public HistoryLoader(VersionControlServer versionControl)
        {
            this.versionControl = versionControl;
        }

        private string GetUserNameWithoutDomain()
        {
            var fullName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            return fullName.Split('\\', '\\').Last();
        }

        protected override IEnumerable<Changeset> LoadData()
        {
            string userName = GetUserNameWithoutDomain();

            IEnumerable changesets = versionControl.QueryHistory("$/", VersionSpec.Latest, 0, RecursionType.Full, userName,
                                                                 new DateVersionSpec(DateTime.Today.AddDays(-10)), VersionSpec.Latest, 1000, false, false, false);
            
            List<Changeset> css = new List<Changeset>();
            
            //Enumerate on the thread to avoid blocking the UI
            var enumerator = changesets.GetEnumerator();
            while (enumerator.MoveNext())
            {
                css.Add(enumerator.Current as Changeset);
            }

            return css;
        }
    }
}