using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NatisTracker.ViewModels;

namespace NatisTracker.UIServices
{
    interface Populate
    {
        void PopulateSendToDriver(DriverDocsViewModel viewModel, string user, string department);
        void PopulateSendToUser(SendToUserViewModel viewModel, string user, string department);
    }
}
