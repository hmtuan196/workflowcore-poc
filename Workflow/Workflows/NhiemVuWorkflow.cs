using System;
using Microsoft.Extensions.Logging;
using Workflow.Workflows.Data;
using Workflow.Workflows.Steps;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Workflow.Workflows
{
    public class NhiemVuWorkflow : IWorkflow<NhiemVuData>
    {
        public string Id => nameof(NhiemVuWorkflow);
        public int Version => 1;

        public void Build(IWorkflowBuilder<NhiemVuData> builder)
        {
            builder
                .StartWith(context => ExecutionResult.Next())
                .Parallel()
                    .Do(then =>
                        then.While(t => t.TrangThai != TrangThaiNhiemVu.DaHoanThanh)
                                .Do(wf => wf
                                            .StartWith(context => ExecutionResult.Next())
                                            .WaitFor(NhiemVuWorkflowEvents.DaPhanXuLy, (data, context) => context.Workflow.Id, data => DateTime.UtcNow)
                                                .Output(data => data.PhanXuLyNhiemVu, step => step.EventData)
                                            .Then<IPhanXuLy>()
                                                .Input(step => step.PhanXuLyNhiemVu, data => data.PhanXuLyNhiemVu)))
                    .Do(then =>
                        then.While(t => t.TrangThai != TrangThaiNhiemVu.DaHoanThanh)
                                .Do(wf => wf
                                            .StartWith(context => ExecutionResult.Next())
                                            .WaitFor(NhiemVuWorkflowEvents.DaTraLai, (data, context) => context.Workflow.Id, data => DateTime.UtcNow)
                                                .Output(data => data.PhanXuLyNhiemVu, step => step.EventData)
                                            .Then<ITraLai>()
                                                .Input(step => step.PhanXuLyNhiemVu, data => data.PhanXuLyNhiemVu)))
                    .Do(then =>
                        then.While(t => t.TrangThai != TrangThaiNhiemVu.DaHoanThanh)
                                .Do(wf => wf
                                            .StartWith(context => ExecutionResult.Next())
                                            .WaitFor(NhiemVuWorkflowEvents.DaCapNhatTinhHinh, (data, context) => context.Workflow.Id, data => DateTime.UtcNow)
                                                .Output(data => data.PhanXuLyNhiemVu, step => step.EventData)
                                            .Then<ICapNhatTinhHinh>()
                                                .Input(step => step.PhanXuLyNhiemVu, data => data.PhanXuLyNhiemVu)))
                .Join()
                .CancelCondition(data => data.TrangThai == TrangThaiNhiemVu.DaHoanThanh, true)
                .Then<Finish>();
        }
    }

    internal class Finish : StepBody
    {
        private readonly ILogger<Finish> _logger;

        public Finish(ILogger<Finish> logger)
        {
            _logger = logger;
        }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            _logger.LogInformation("Kết thúc...");
            return ExecutionResult.Next();
        }
    }
}