using Microsoft.Extensions.Configuration;

namespace SurveryProject.Service.Base
{
    /// <summary>
    /// Data config
    /// </summary>
    public class DataConfig
    {
        private readonly IConfiguration _config;

        public DataConfig(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// 取得JBPM Domain
        /// </summary>
        public string JBPMDomain => _config["AppSettings:JBPMDomain"];

        /// <summary>
        /// 取得ESIP Client ID
        /// </summary>
        public string ESIPClientID => _config["AppSettings:ESIPClientID"];

        /// <summary>
        /// 取得FlowStage Service Domain
        /// </summary>
        public string FlowStageDomain
        {
            get
            {
                var value = _config["AppSettings:FlowStageDomain"];
                Console.WriteLine($"FlowStageDomain = {value}");
                return value;
            }
        }
    }
}
