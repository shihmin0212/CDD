using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using CDD.Api.Libs; // 你自己的 IRequest 介面

#region DTOs

#region FlowCreateReq
public class FlowCreateReq
{
    public string formId { get; set; }
}
#endregion

#region 第一支 API Response
public class StartProcessResp
{
    public string tid { get; set; }
    public string code { get; set; }
    public string message { get; set; }
    public StartProcessData data { get; set; }
}
public class StartProcessData
{
    public string uid { get; set; }
}
#endregion

#region 第二支 API Request/Response
public class UpdateFlowStatusReq
{
    public int AdditionStage { get; set; }
    public string CurrentCustomFlowKey { get; set; }
    public int CurrentStep { get; set; }
    public bool CustomFlowFromView { get; set; }
    public string CustomFlowKey { get; set; }
    public string CustomNextENO { get; set; }
    public string CustomNextName { get; set; }
    public string DocCode { get; set; }
    public int FlowBasicInfoID { get; set; }
    public FlowLog FlowLog { get; set; }
    public FlowStatus FlowStatus { get; set; }
    public string LoginENO { get; set; }
    public bool MultiSign { get; set; }
    public string NextCustomFlowKey { get; set; }
    public int NextStep { get; set; }
    public int SendCase { get; set; }
    public string SignedID { get; set; }
    public string SignedName { get; set; }
    public string SignID { get; set; }
    public int StageDesignate { get; set; }
    public int StepFrom { get; set; }
}
public class FlowLog
{
    public string DocCode { get; set; }
    public bool IsAgent { get; set; }
    public string SignedID { get; set; }
    public string SignedName { get; set; }
}
public class FlowStatus
{
    public bool AllowAgent { get; set; }
    public bool AuthAddCSUnit { get; set; }
    public string Button { get; set; }
    public int CounterSignedExit { get; set; }
    public List<object> CounterSignList { get; set; }
    public int CurrentStep { get; set; }
    public string CustomFlowKey { get; set; }
    public bool GoCounterSigned { get; set; }
    public List<object> HistoryFlow { get; set; }
    public string JBPMUID { get; set; }
    public string SignID { get; set; }
    public string StageAction { get; set; }
    public int Status { get; set; }
}
public class UpdateFlowResp
{
    public bool IsSuccess { get; set; }
    public List<string> ValidationMsg { get; set; }
}
#endregion

#region 第三支 API Request/Response
public class ImportPaperReq
{
    public string PaperID { get; set; }
    public string DocNo { get; set; }
    public string DocID { get; set; }
    public DateTime ApplyDateUTC { get; set; }
    public string ApplyEmpName { get; set; }
    public string ApplyEmpNo { get; set; }
    public string ModifyBy { get; set; }
    public int FlowStatus { get; set; }
    public string JbpmUID { get; set; }
    public string CurrentENO { get; set; }
    public string CurrentName { get; set; }
    public DateTime ExpireTime { get; set; }
    public int AlertLevel { get; set; }
    public string ApplyItem { get; set; }
    public string URL { get; set; }
}
#endregion

#endregion

//#region Repository (Mock)
//public interface IFormDetailRepository
//{
//    Task UpdateJbpmUuidAsync(string formId, string jbpmUuid);
//}

//// 假的 Repository 實作
//public class FormDetailRepository : IFormDetailRepository
//{
//    public Task UpdateJbpmUuidAsync(string formId, string jbpmUuid)
//    {
//        // 這裡僅模擬，實際請改為 DB 操作
//        Console.WriteLine($"[Repository] Update FormID={formId} 的 JBPMUUID={jbpmUuid}");
//        return Task.CompletedTask;
//    }
//}
//#endregion

#region Service

public class FlowService
{
    private readonly ILogger _logger;
    private readonly IConfiguration _config;
    private readonly IRequest _request;
    //private readonly IFormDetailRepository _formDetailRepository;

    public FlowService(
        ILogger<FlowService> logger,
        IConfiguration config,
        IRequest request
        //IFormDetailRepository formDetailRepository
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _request = request ?? throw new ArgumentNullException(nameof(request));
        //_formDetailRepository = formDetailRepository ?? throw new ArgumentNullException(nameof(formDetailRepository));
    }

    /// <summary>
    /// 起單流程（依序串接三支外部API）
    /// </summary>
    public async Task<Tuple<bool, string, Guid>> CreateNewProcess(FlowCreateReq req)
    {
        Guid caseID = Guid.NewGuid();

        #region 1. JBPM API 存入起單資訊 StartProcess
        // 呼叫 JBPM API，建立起單流程
        var startProcessUrl = "https://api-esipt.testesunsec.com.tw/esunbankt/bankapi/process/v1/tw/up0051/post_process_start/OA.ESUNSECOA.COMMON";
        var startProcessPayload = new
        {
            assignDescription = "信件會用到喔，現在還沒用!",
            executorId = "71509",
            formData = new
            {
                applicantId = "71509",
                applicantName = "○○融",
                applyTime = "2025-09-18T02:15:32Z",
                createTime = (string)null,
                description = "123456",
                executorId = "71509",
                executorName = "○○融",
                formId = req.formId,
                formTypeId = "OA.ESUNSECOA.COMMON",
                formTypeName = "顧客盡職審查",
                processInfo = "",
                status = "",
                uid = (string)null
            },
            mailData = new
            {
                bccRole = (string)null,
                bccUser = (string)null,
                ccRole = (string)null,
                ccUser = (string)null,
                content = "<div>親愛的主管/同仁您好：<br /><br />請您處理【${formTypeName}】單號：${formId}<br /><br/>請用滑鼠左鍵點一下連結即可開啟該份文件進行後續處理：<a href=${url}>連結</a><br /><br />※備註：<br/>${descritpion}<br /><br /></div>",
                from = (string)null,
                isNotify = true,
                mailParams = new
                {
                    descritpion = "申請日期 : 2019/12/17 上午 10:15:31  <br />表單概述 : ",
                    formId = req.formId,
                    formTypeName = "顧客盡職審查",
                    url = $"http://localhost:5173/customer-review/{req.formId}",
                    applyItem = (string)null
                },
                subject = "請您處理【${formTypeName}】${applyItem}，單號：${formId}",
                toRole = (string)null,
                toUser = new List<string>(),
                Uid = (string)null
            },
            parameter = "{\"applicant\":\"71509\",\"autoStart\":\"N\",\"send\":\"Y\",\"sendType\":\"people\",\"signer\":\"90597\",\"expiretime\":\"2025-09-20\",\"role\":\"\"}",
            signAction = "起單，○○融(71509)傳送至受稽核部門主管[2] ○○嶽(90597)",
            signComment = "來瞜",
            taskId = 0,
            taskOwnerId = (string)null,
            url = $"http://localhost:5173/customer-review/{req.formId}",
            viewUrl = "",
            isAgent = false
        };
        string startBody = JsonConvert.SerializeObject(startProcessPayload);
        var headers = new Dictionary<string, string> { { "user-id", "71509" } };
        var startResp = await _request.PostJSON<StartProcessResp>(startProcessUrl, startBody, headers);

        if (startResp == null || startResp.data == null || string.IsNullOrEmpty(startResp.data.uid))
        {
            string errMsg = startResp?.message ?? "第一支 API 失敗";
            return Tuple.Create(false, $"JBPM API 失敗: {errMsg}", caseID);
        }

        string jbpmUid = startResp.data.uid;
        #endregion

        //#region 1.5 Repository 更新 JBPMUUID
        //await _formDetailRepository.UpdateJbpmUuidAsync(req.formId, jbpmUid);
        //#endregion

        #region 2. FlowStage API 紀錄關卡資訊 UpdateFlowStatus
        var updateFlowUrl = "https://FlowStageAPISIT.testesunsec.com.tw/FlowStageVer2/UpdateFlowStatus";
        var updateFlowPayload = new UpdateFlowStatusReq
        {
            AdditionStage = 1,
            CurrentCustomFlowKey = "RM002_Ver1",
            CurrentStep = 5,
            CustomFlowFromView = false,
            CustomFlowKey = null,
            CustomNextENO = null,
            CustomNextName = null,
            DocCode = null,
            FlowBasicInfoID = 0,
            FlowLog = new FlowLog
            {
                DocCode = "RM002",
                IsAgent = false,
                SignedID = "00071",
                SignedName = "林?輝"
            },
            FlowStatus = new FlowStatus
            {
                AllowAgent = false,
                AuthAddCSUnit = false,
                Button = null,
                CounterSignedExit = 0,
                CounterSignList = new List<object>(),
                CurrentStep = 0,
                CustomFlowKey = null,
                GoCounterSigned = false,
                HistoryFlow = new List<object>(),
                JBPMUID = jbpmUid,
                SignID = null,
                StageAction = null,
                Status = 3
            },
            LoginENO = null,
            MultiSign = false,
            NextCustomFlowKey = "RM002_Ver1",
            NextStep = 5,
            SendCase = 0,
            SignedID = null,
            SignedName = null,
            SignID = "RM002202508260001",
            StageDesignate = 0,
            StepFrom = 0
        };
        string updateBody = JsonConvert.SerializeObject(updateFlowPayload);
        var updateResp = await _request.PostJSON<UpdateFlowResp>(updateFlowUrl, updateBody);

        if (updateResp == null || updateResp.IsSuccess != true)
            return Tuple.Create(false, "FlowStage API 失敗", caseID);
        #endregion

        #region 3. OAPortal API 匯入 Import
        var importPaperUrl = "https://oaportalapisit.testesunsec.com.tw/PaperApi/ImportPaper";
        var importPaperPayload = new ImportPaperReq
        {
            PaperID = "VM",
            DocNo = req.formId,
            DocID = caseID.ToString(),
            ApplyDateUTC = DateTime.UtcNow,
            ApplyEmpName = "○○融",
            ApplyEmpNo = "71509",
            ModifyBy = "○○融(71509)",
            FlowStatus = 0,
            JbpmUID = jbpmUid,
            CurrentENO = "71509",
            CurrentName = "○○融",
            ExpireTime = DateTime.UtcNow.AddDays(7),
            AlertLevel = 0,
            ApplyItem = "顧客盡職表達審查ROUND2",
            URL = "http://localhost:5173/customer-review"
        };
        string importBody = JsonConvert.SerializeObject(importPaperPayload);
        var importResp = await _request.PostJSON<bool>(importPaperUrl, importBody);

        if (importResp != true)
            return Tuple.Create(false, "OAPortal API 匯入失敗", caseID);
        #endregion

        return Tuple.Create(true, "表單流程新增成功", caseID);
    }
}

#endregion
