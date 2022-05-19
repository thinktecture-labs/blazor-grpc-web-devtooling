using Blazor.GrpcDevTools.Shared.DTO;
using Blazor.GrpcDevTools.Shared.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Blazor.GrpcDevTools.Client.Features.Conferences
{
    public partial class Conferences
    {
        [Inject] private IConferencesService _conferencesService { get; set; } = default!;
        [Inject] private NavigationManager _navigationManager { get; set; } = default!;

        private int selectedRowNumber = -1;
        private MudTable<ConferenceOverview>? mudTable;
        private List<ConferenceOverview>? _conferences;

        protected override async Task OnInitializedAsync()
        {
            _conferences = (await _conferencesService.ListConferencesAsync()).ToList();
            await base.OnInitializedAsync();
        }

        private void RowClickEvent(TableRowClickEventArgs<ConferenceOverview> tableRowClickEventArgs)
        {
            _navigationManager.NavigateTo($"/conferences/{tableRowClickEventArgs.Item.ID}");
        }

        private string SelectedRowClassFunc(ConferenceOverview conf, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                return string.Empty;
            }
            else if (mudTable?.SelectedItem != null && mudTable.SelectedItem.Equals(conf))
            {
                selectedRowNumber = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}