using Blazor.GrpcDevTools.Shared.DTO;
using Blazor.GrpcDevTools.Shared.Services;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazor.GrpcDevTools.Client.Features.Conferences.Components
{
    public partial class ConferenceDetails
    {
        [Inject] private IConferencesService _conferencesService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;

        [Parameter] public Guid Id { get; set; }

        private ConferenceDetailModel? _conference;
        protected override async Task OnInitializedAsync()
        {
            _conference = await _conferencesService.GetConferenceDetailsAsync(new ConferenceDetailsRequest { ID = Id });
            await base.OnInitializedAsync();
        }

        private async Task SaveConference()
        {
            await _conferencesService.UpdateConferenceAsync(
                new ConferenceUpdateRequest { ID = Id, Title = _conference?.Title ?? string.Empty });
            _navigationManager.NavigateTo("/");
        }

        private void Cancel() => _navigationManager.NavigateTo("/");
    }
}