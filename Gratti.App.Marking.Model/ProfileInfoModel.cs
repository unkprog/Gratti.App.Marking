namespace Gratti.App.Marking.Model
{
    public class ProfileInfoModel
    {
        public string Name { get; set; }

        public string FilePath { get; set; } = null;


        public string GisUri { get; set; }

        public string OmsUri { get; set; }

        public string CmgUri { get; set; }

        public string OmsId { get; set; }

        public string ConnectionId { get; set; }
        public string ApiKey { get; set; }

        public string ThumbPrint { get; set; }

        public string SqlConnectionString { get; set; }

    }
}
