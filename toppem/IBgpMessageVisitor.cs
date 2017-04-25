using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toppem
{
    public interface IBgpMessageVisitor
    {
        void Visit(BgpOpenMessage openMessage);
        void Visit(BgpUpdateMessage openMessage);
        void Visit(BgpKeepaliveMessage keepaliveMessage);
        void Visit(BgpNotificationMessage notificationMessage);
    }
}
