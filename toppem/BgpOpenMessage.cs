﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public class BgpOpenMessage : IBgpMessage
    {
        public BgpOpenMessage(int version, int asNum, int holdTime, int identifier)
        {
        }

        public void Accept(IBgpMessageVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
