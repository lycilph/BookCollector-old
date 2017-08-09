﻿namespace Core
{
    public interface IScreen
    {
        string DisplayName { get; set; }
        bool IsActive { get; }

        void Activate();
        void Deactivate();
    }
}