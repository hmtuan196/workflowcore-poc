using System.Linq;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Workflow.Workflows.Steps
{
    public class TraLai : StepBody, ITraLai
    {
        private readonly ILogger<TraLai> _logger;

        public PhanXuLyNhiemVu PhanXuLyNhiemVu { get; set; }

        public TraLai(ILogger<TraLai> logger) => _logger = logger;

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            var phanXuLy = Database.PhanXuLyNhiemVus.First(p => p.NhiemVuId == PhanXuLyNhiemVu.Id);

            if (phanXuLy.VaiTroXuLy == VaiTroXuLy.PhoiHop)
            {
                _logger.LogWarning($"Phối hợp không được Trả lại... {PhanXuLyNhiemVu.Id} - nhiệm vụ {PhanXuLyNhiemVu.NhiemVuId}");
                return ExecutionResult.Next();
            }

            // trả lại
            phanXuLy.TrangThai = TrangThaiPhanXuLy.DaTraLai;

            _logger.LogInformation($"Trả lại... {PhanXuLyNhiemVu.Id} - nhiệm vụ {PhanXuLyNhiemVu.NhiemVuId}");
            return ExecutionResult.Next();
        }
    }

    public interface ITraLai : IStepBody, INhiemVuStep
    {

    }
}