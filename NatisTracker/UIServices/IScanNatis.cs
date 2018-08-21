using NatisTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NatisTracker.ScanNatis
{
    public interface IScanNatis
    {
        bool Scan(NatisDataViewModel viewModel, string name,string department);
    }
}