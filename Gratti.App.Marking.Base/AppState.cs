using Gratti.App.Marking.Api;
using Gratti.App.Marking.Core.Interfaces;
using Gratti.App.Marking.Model;
using Gratti.App.Marking.Services;

namespace Gratti.App.Marking
{
    public class AppState
    {
        public AuthService Auth { get; private set; }
        public Oms OmsApi { get; private set; }
        public Cmg CmgApi { get; private set; }


        public void SetProfile(ProfileInfoModel profile, ILoggerOutput logger = null)
        {
            Auth = new AuthService(profile, logger);
            OmsApi = new Oms(profile.OmsUri, profile.OmsId);
            CmgApi = new Cmg(profile.CmgUri, profile.ApiKey);
        }

    }
}
