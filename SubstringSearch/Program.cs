using System.ServiceProcess;

namespace SubstringSearch
{
    public static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new SubstringSearchService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
