using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Workflow.Workflows;
using Workflow.Workflows.Data;
using WorkflowCore.Interface;

namespace Workflow.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NhiemVuController : ControllerBase
    {
        private readonly ILogger<NhiemVuController> _logger;
        private readonly IWorkflowHost _host;

        public NhiemVuController(IWorkflowHost host, ILogger<NhiemVuController> logger)
        {
            _logger = logger;
            _host = host;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var nhiemVu = new NhiemVu
            {
                Id = 1,
                CanBoId = 1,
                DonViId = 1,
                TrangThai = TrangThaiNhiemVu.ChoThucHien
            };

            nhiemVu.WorkflowId = _host.StartWorkflow(nameof(NhiemVuWorkflow), new NhiemVuData { NhiemVuId = nhiemVu.Id, TrangThai = TrangThaiNhiemVu.ChoThucHien }).GetAwaiter().GetResult();

            Database.NhiemVus.Add(nhiemVu);
            Database.PhanXuLyNhiemVus.Add(new PhanXuLyNhiemVu
            {
                Id = 1,
                CanBoId = 1,
                DonViId = 1,
                NhiemVuId = 1,
                VaiTroXuLy = VaiTroXuLy.ChuTri,
                TrangThai = TrangThaiPhanXuLy.DangThucHien
            });

            // terminate workflow if adding db error

            _logger.LogInformation($"============ Started workflow {nhiemVu.WorkflowId} ============");

            return Ok();
        }

        [HttpGet("phan")]
        public IActionResult PhanXuLy(int phanXuLyId)
        {
            var phanXuLy = Database.PhanXuLyNhiemVus.First(n => n.Id == phanXuLyId);
            var nhiemVu = Database.NhiemVus.First(n => n.Id == phanXuLy.NhiemVuId);

            if (phanXuLy.VaiTroXuLy == VaiTroXuLy.PhoiHop)
            {
                return BadRequest();
            }

            _host.PublishEvent(NhiemVuWorkflowEvents.DaPhanXuLy, nhiemVu.WorkflowId, new PhanXuLyNhiemVu
            {
                PhanXuLyNhiemVuChaId = phanXuLy.Id,
                CanBoId = 2,
                DonViId = 2,
                NhiemVuId = nhiemVu.Id,
                TrangThai = TrangThaiPhanXuLy.DangThucHien,
                VaiTroXuLy = VaiTroXuLy.PhoiHop
            });

            return Ok();
        }

        [HttpGet("tra")]
        public IActionResult TraLai(int phanXuLyId)
        {
            var phanXuLy = Database.PhanXuLyNhiemVus.First(p => p.Id == phanXuLyId);
            var nhiemVu = Database.NhiemVus.First(n => n.Id == phanXuLy.NhiemVuId);

            if (phanXuLy.VaiTroXuLy == VaiTroXuLy.PhoiHop)
            {
                return BadRequest();
            }

            _host.PublishEvent(NhiemVuWorkflowEvents.DaTraLai, nhiemVu.WorkflowId, phanXuLy);

            return Ok();
        }

        [HttpGet("xong")]
        public IActionResult CapNhat(int phanXuLyId)
        {
            var phanXuLy = Database.PhanXuLyNhiemVus.First(n => n.Id == phanXuLyId);
            var nhiemVu = Database.NhiemVus.First(n => n.Id == phanXuLy.NhiemVuId);

            if (phanXuLy.VaiTroXuLy == VaiTroXuLy.PhoiHop)
            {
                return BadRequest();
            }

            _host.PublishEvent(NhiemVuWorkflowEvents.DaCapNhatTinhHinh, nhiemVu.WorkflowId, phanXuLy);

            return Ok();
        }
    }
}