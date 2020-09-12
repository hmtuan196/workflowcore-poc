using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Workflow.Workflows;
using Workflow.Workflows.Data;
using WorkflowCore.Interface;

namespace Workflow.Controllers
{
    [ApiController]
    [Route("nhiem-vu")]
    public class NhiemVuController : ControllerBase
    {
        private readonly ILogger<NhiemVuController> _logger;
        private readonly IWorkflowHost _host;

        public NhiemVuController(IWorkflowHost host, ILogger<NhiemVuController> logger)
        {
            _logger = logger;
            _host = host;
        }

        [HttpPost("tao")]
        public IActionResult TaoNhiemVu()
        {
            var id = Database.NhiemVus.Any() ? Database.NhiemVus.Max(n => n.Id) + 1 : 1;
            var nhiemVu = new NhiemVu
            {
                Id = id,
                CanBoId = 1,
                DonViId = 1,
                TrangThai = TrangThaiNhiemVu.ChoThucHien
            };

            nhiemVu.WorkflowId = _host.StartWorkflow(nameof(NhiemVuWorkflow), new NhiemVuData { NhiemVuId = nhiemVu.Id, TrangThai = TrangThaiNhiemVu.ChoThucHien }).GetAwaiter().GetResult();

            // terminate workflow if adding db error
            Database.NhiemVus.Add(nhiemVu);
            Database.PhanXuLyNhiemVus.Add(new PhanXuLyNhiemVu
            {
                Id = 1,
                CanBoId = 1,
                DonViId = 1,
                NhiemVuId = id,
                VaiTroXuLy = VaiTroXuLy.ChuTri,
                TrangThai = TrangThaiPhanXuLy.DangThucHien,
                WorkflowId = nhiemVu.WorkflowId
            });

            _logger.LogInformation($"============ Started workflow {nhiemVu.WorkflowId} for Nhiem vu {id} ============");

            return Ok();
        }

        [HttpPost("phan")]
        public IActionResult PhanXuLy(NhiemVuModel request)
        {
            var phanXuLy = Database.PhanXuLyNhiemVus.First(p => p.Id == request.PhanXuLyId && p.NhiemVuId == request.NhiemVuId);

            var id = Database.PhanXuLyNhiemVus.Where(p => p.NhiemVuId == request.NhiemVuId).Max(n => n.Id) + 1;

            _host.PublishEvent(NhiemVuWorkflowEvents.DaPhanXuLy, phanXuLy.WorkflowId, new PhanXuLyNhiemVu
            {
                Id = id,
                PhanXuLyNhiemVuChaId = phanXuLy.Id,
                CanBoId = 2,
                DonViId = 2,
                NhiemVuId = request.NhiemVuId,
                TrangThai = TrangThaiPhanXuLy.DangThucHien,
                VaiTroXuLy = VaiTroXuLy.PhoiHop,
                WorkflowId = phanXuLy.WorkflowId
            });

            return Ok();
        }

        [HttpPost("tra")]
        public IActionResult TraLai(NhiemVuModel request)
        {
            var phanXuLy = Database.PhanXuLyNhiemVus.First(p => p.Id == request.PhanXuLyId && p.NhiemVuId == request.NhiemVuId);

            _host.PublishEvent(NhiemVuWorkflowEvents.DaTraLai, phanXuLy.WorkflowId, phanXuLy);

            return Ok();
        }

        [HttpPost("xong")]
        public IActionResult CapNhat(NhiemVuModel request)
        {
            var phanXuLy = Database.PhanXuLyNhiemVus.First(p => p.Id == request.PhanXuLyId && p.NhiemVuId == request.NhiemVuId);

            _host.PublishEvent(NhiemVuWorkflowEvents.DaCapNhatTinhHinh, phanXuLy.WorkflowId, phanXuLy);

            return Ok();
        }
    }

    public class NhiemVuModel
    {
        public int NhiemVuId { get; set; }
        public int PhanXuLyId { get; set; }
    }
}