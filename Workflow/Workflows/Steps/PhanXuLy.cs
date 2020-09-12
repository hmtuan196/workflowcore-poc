using System.Linq;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Workflow.Workflows.Steps
{
    public class PhanXuLy : StepBody, IPhanXuLy
    {
        private readonly ILogger<PhanXuLy> _logger;

        public PhanXuLyNhiemVu PhanXuLyNhiemVu { get; set; }

        public PhanXuLy(ILogger<PhanXuLy> logger) => _logger = logger;

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            var phanXuLyCha = Database.PhanXuLyNhiemVus.First(p => p.Id == PhanXuLyNhiemVu.PhanXuLyNhiemVuChaId);

            if (phanXuLyCha.VaiTroXuLy == VaiTroXuLy.PhoiHop)
            {
                _logger.LogWarning($"Phối hợp không được Phân xử lý... {PhanXuLyNhiemVu.Id} - nhiệm vụ {PhanXuLyNhiemVu.NhiemVuId}");
                return ExecutionResult.Next();
            }

            Database.PhanXuLyNhiemVus.Add(PhanXuLyNhiemVu);

            _logger.LogInformation($"Phân xử lý... {PhanXuLyNhiemVu.Id} - nhiệm vụ {PhanXuLyNhiemVu.NhiemVuId}");
            return ExecutionResult.Next();
        }
    }

    public interface IPhanXuLy : IStepBody, INhiemVuStep
    {
    }
}