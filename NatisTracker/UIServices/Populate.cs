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
        void PopulateData(Driver viewModel, string user);
    }
}
