using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set;}
    }
}
