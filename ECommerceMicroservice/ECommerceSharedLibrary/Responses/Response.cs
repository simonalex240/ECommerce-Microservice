using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSharedLibrary.Responses
{
    public record Response(bool Flag = false, string Message = null!);
}
