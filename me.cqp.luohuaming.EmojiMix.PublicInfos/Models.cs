using System.Collections.Generic;
using System.Text;
using me.cqp.luohuaming.EmojiMix.Sdk.Cqp.EventArgs;

namespace me.cqp.luohuaming.EmojiMix.PublicInfos
{
    public interface IOrderModel
    {
        bool ImplementFlag { get; set; }
        string GetOrderStr();
        bool Judge(string destStr, long fromGroup);
        FunctionResult Progress(CQGroupMessageEventArgs e);
        FunctionResult Progress(CQPrivateMessageEventArgs e);
    }
}
