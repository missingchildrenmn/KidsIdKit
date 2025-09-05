using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KidsIdKit.Shared.Services;

public interface IFileSaverService
{
    Task SaveFileAsync(string filename, string content);
}
