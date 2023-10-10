// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Dashboard.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Fast.Components.FluentUI;
using Microsoft.JSInterop;

namespace Aspire.Dashboard.Components.Dialogs;
public partial class EnvironmentVariables
{

    [Parameter]
    public List<EnvironmentVariableViewModel>? Content { get; set; }

    private IQueryable<EnvironmentVariableViewModel>? FilteredItems =>
        Content?.Where(vm =>
            vm.Name.Contains(_filter, StringComparison.CurrentCultureIgnoreCase) ||
            vm.Value?.Contains(_filter, StringComparison.CurrentCultureIgnoreCase) == true
        )?.AsQueryable();

    private const string PreCopyText = "Copy to clipboard";
    private const string PostCopyText = "Copied!";

    private string _filter = "";
    private bool _defaultMasked = true;

    private readonly Icon _maskIcon = new Icons.Regular.Size16.EyeOff();
    private readonly Icon _unmaskIcon = new Icons.Regular.Size16.Eye();

    private readonly GridSort<EnvironmentVariableViewModel> _nameSort = GridSort<EnvironmentVariableViewModel>.ByAscending(vm => vm.Name);
    private readonly GridSort<EnvironmentVariableViewModel> _valueSort = GridSort<EnvironmentVariableViewModel>.ByAscending(vm => vm.Value);

    private void ToggleMaskState()
    {
        _defaultMasked = !_defaultMasked;
        if (Content is not null)
        {
            foreach (var vm in Content)
            {
                vm.IsValueMasked = _defaultMasked;
            }
        }
    }

    private void ToggleMaskState(EnvironmentVariableViewModel vm)
    {
        vm.IsValueMasked = !vm.IsValueMasked;
        CheckAllMaskStates();
    }

    private void HandleFilter(ChangeEventArgs args)
    {
        if (args.Value is string newFilter)
        {
            _filter = newFilter;
        }
    }

    private void HandleClear(string? value)
    {
        _filter = value ?? string.Empty;
    }

    private void CheckAllMaskStates()
    {
        if (Content is not null)
        {
            bool foundMasked = false;
            bool foundUnmasked = false;
            foreach (var vm in Content)
            {
                foundMasked |= vm.IsValueMasked;
                foundUnmasked |= !vm.IsValueMasked;
            }

            if (!foundMasked && foundUnmasked)
            {
                _defaultMasked = false;
            }
            else if (foundMasked && !foundUnmasked)
            {
                _defaultMasked = true;
            }
        }
    }

    private async Task CopyTextToClipboardAsync(string? text, string id)
    {
        await JS.InvokeVoidAsync("copyTextToClipboard", id, text, PreCopyText, PostCopyText);
    }
}