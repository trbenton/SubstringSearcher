using System.ServiceProcess;

namespace SubstringSearch
{
    public partial class SubstringSearchService : ServiceBase
    {
        private static SubstringServer _server;

        public SubstringSearchService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _server = new SubstringServer();
            _server.StartListener();
        }

        protected override void OnStop()
        {
        }
    }
}
