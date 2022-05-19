using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using MudBlazor;
using Blazor.GrpcDevTools.Client;
using Blazor.GrpcDevTools.Client.Shared;
using Grpc.Net.Client;
using Blazor.GrpcDevTools.Shared.Services;
using Blazor.GrpcWeb.DevTools;
using Grpc.Core;
using ProtoBuf.Grpc;

namespace Blazor.GrpcDevTools.Client.Shared
{
    public partial class MainLayout : IDisposable
    {
        [Inject] private ITimeService _timeService { get; set; } = default!;

        private string _time = "";
        private CancellationTokenSource? _cts;

        protected override async Task OnInitializedAsync()
        {
            await StartTime();
            await base.OnInitializedAsync();
        }

        private async Task StartTime()
        {
            if (_timeService == null)
            {
                return;
            }

            _cts = new CancellationTokenSource();
            var options = new CallOptions(cancellationToken: _cts.Token);

            try
            {
                await foreach (var time in _timeService.SubscribeAsync(new CallContext(options)))
                {
                    _time = time;
                    StateHasChanged();
                }
            }
            catch (RpcException)
            {
            }
            catch (OperationCanceledException)
            {
            }
        }
        private void StopTime()
        {
            _cts?.Cancel();
            _cts = null;
            _time = "";
        }

        public void Dispose()
        {
            StopTime();
        }
    }
}