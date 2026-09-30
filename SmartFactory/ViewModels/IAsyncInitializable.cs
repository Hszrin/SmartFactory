using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFactory.ViewModels
{
    public interface IAsyncInitializable
    {
        Task InitializeAsync(CancellationToken token);
    }
}
