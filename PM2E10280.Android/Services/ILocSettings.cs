using System;
using System.Collections.Generic;
using System.Text;

namespace PM2E10280.Services
{
    public interface ILocSettings
    {
        void OpenSettings();

        bool isGpsAvailable();

    }
}
