using CDD.Web.Models.DTO;

namespace CDD.Web.Models.APIResponse
{
    /// <summary>
    /// 取得停止登入通知等
    /// </summary>
    public class API_GetServiceAlertResp
    {
        public string api { get; set; }

        public string code { get; set; }

        public List<AlertMessage> rows { get; set; }

        public bool IsSuccess()
        {
            return (this.code == "0");
        }
    }

    public class AlertMessage
    {
        // 9 通過 // 1 跳警告通知 // 0 阻擋進入
        public string pass { get; set; }

        // 測試系統維護暫停登入
        public string subject { get; set; }

        // 親愛的顧客您好，為提供您更好的服務，將於2022-06-22 08:43~08:45進行系統維護作業，屆時暫停登入，造成您的不便，敬請見諒。
        public string notes { get; set; }
    }


}
