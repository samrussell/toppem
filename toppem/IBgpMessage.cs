﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public interface IBgpMessage
    {
        void Accept(IBgpMessageVisitor visitor);
    }
}
