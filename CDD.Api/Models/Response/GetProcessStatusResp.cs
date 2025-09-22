namespace CDD.API.Models.Response
{
    public class ProcessStatusResult
    {
        public List<ProcessStage> Stages { get; set; }
        public bool HasNextFlow { get; set; }
        public bool IsInCounterSign { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> ValidationMsg { get; set; }
    }

    public class ProcessStage
    {
        public int Index { get; set; }
        public decimal StepSequence { get; set; }           // 1.0 / 2.0
        public string? CustomFlowKey { get; set; }
        public string? SignedTitle { get; set; }
        public string? SignedEmpName { get; set; }
        public string? SignedEmpNum { get; set; }
        public string? SignedTodo { get; set; }
        public string? SignedDate { get; set; }             // 例："2025/09/19"（為避免格式差異，先用字串）
        public int Status { get; set; }
    }
}