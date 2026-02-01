
using System;
using System.Collections.Generic;
using System.Text;

namespace LibrarySystem.Services.Interfaces
{
    public interface ICurrentUserContext
    {
        int ExternalUserId { get; }
        int LocalUserId { get; }
    }

}
