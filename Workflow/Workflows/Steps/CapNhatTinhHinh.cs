using System.Linq;
using Microsoft.Extensions.Logging;
using Workflow.Workflows.Data;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Workflow.Workflows.Steps
{
    public class CapNhatTinhHinh : StepBody, ICapNhatTinhHinh
    {
        private readonly ILogger<CapNhatTinhHinh> _logger;
        private readonly IWorkflowHost _host;

        public PhanXuLyNhiemVu PhanXuLyNhiemVu { get; set; }

        public CapNhatTinhHinh(ILogger<CapNhatTinhHinh> logger, IWorkflowHost host)
        {
            _logger = logger;
            _host = host;
        }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            var nhiemVu = Database.NhiemVus.First(n => n.Id == PhanXuLyNhiemVu.NhiemVuId);

            if (PhanXuLyNhiemVu.VaiTroXuLy == VaiTroXuLy.PhoiHop)
            {
                _logger.LogWarning($"Phối hợp không được Phân xử lý... {PhanXuLyNhiemVu.Id} - nhiệm vụ {PhanXuLyNhiemVu.NhiemVuId}");
                return ExecutionResult.Next();
            }

            // hoàn thành
            nhiemVu.TrangThai = TrangThaiNhiemVu.DaHoanThanh;

            var nhiemVuWorkflow = _host.PersistenceStore.GetWorkflowInstance(nhiemVu.WorkflowId).GetAwaiter().GetResult();
            var nhiemVuData = nhiemVuWorkflow.Data as NhiemVuData;
            nhiemVuData.TrangThai = TrangThaiNhiemVu.DaHoanThanh;

            _host.PersistenceStore.PersistWorkflow(nhiemVuWorkflow);

            _logger.LogInformation($"Cập nhật... {PhanXuLyNhiemVu.Id} - nhiệm vụ {PhanXuLyNhiemVu.NhiemVuId}");
            return ExecutionResult.Next();
        }
    }

    public interface ICapNhatTinhHinh: IStepBody, INhiemVuStep
    {

    }
}