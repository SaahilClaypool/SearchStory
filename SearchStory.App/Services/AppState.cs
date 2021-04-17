using System;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace SearchStory.App.Services
{
    public class StateContainer
    {
        public AppState Value { get; set; }
        public event Action<string?>? OnChange;

        public StateContainer() : this(new()) { }
        public StateContainer(AppState initialState)
        {
            Value = initialState;
        }

        public void Update(Action update, string? key = null)
        {
            update();
            OnChange?.Invoke(key);
        }
        
        public void Update(Action<AppState> update, string? key = null)
        {
            update(Value);
            OnChange?.Invoke(key);
        }
    }

    public class AppState
    {
        public string? ErrorMessage { get; set; }
        public enum VisibleControlType {
            None,
            AddFile,
            AddDirectory
        }
        public VisibleControlType VisibleControl = VisibleControlType.None;
        public string? PreviewHref { get; set; } = null;
        public string? OriginalHref { get; set; } = null;
        public bool LoggedIn = false;
        public ClaimsPrincipal? User = null;
        public bool AutoPreviewFirstResult = true;
        public readonly bool CanQuit =
            #if Windows
            true
            #else
            false
            #endif
        ;
    }
}