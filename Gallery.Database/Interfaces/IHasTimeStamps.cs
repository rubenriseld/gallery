﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.API.Interfaces
{
    public interface IHasTimeStamps
    {
        DateTime CreatedDate { get; set; }
        DateTime? ModifiedDate { get; set; }
    }
}