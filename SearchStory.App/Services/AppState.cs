using System;

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
        public readonly bool CanQuit =
            #if Windows
            true
            #else
            false
            #endif
        ;
    }
}