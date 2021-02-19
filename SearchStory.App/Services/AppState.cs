using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using SearchStory.App.Shared.Components;

namespace SearchStory.App.Services
{
    public class StateContainer
    {
        public AppState Value { get; set; }
        public event Action? OnChange;

        public StateContainer() : this(new()) { }
        public StateContainer(AppState initialState)
        {
            Value = initialState;
        }

        public void Update(Action update)
        {
            update();
            OnChange?.Invoke();
        }
    }

    public class AppState
    {
        public string? ErrorMessage { get; set; }
    }
}