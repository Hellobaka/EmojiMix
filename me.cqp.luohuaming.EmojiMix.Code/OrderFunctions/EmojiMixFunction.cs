using System;
using System.IO;
using me.cqp.luohuaming.EmojiMix.Sdk.Cqp;
using me.cqp.luohuaming.EmojiMix.Sdk.Cqp.EventArgs;
using me.cqp.luohuaming.EmojiMix.Sdk.Cqp.Model;
using me.cqp.luohuaming.EmojiMix.Tool.Http;
using Newtonsoft.Json.Linq;
using PublicInfos;

namespace me.cqp.luohuaming.EmojiMix.Code.OrderFunctions
{
    public class EmojiMixFunction : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => "";

        public bool Judge(string destStr)
        {
            var b = destStr.Replace("#", "").Replace("＃", "").Replace(" ", "").Split('+');
            return MainSave.EmojiData != null && b.Length == 2 &&
                ((b[0].IsEmojiCQCode() || b[0].Length == 2) && (b[1].IsEmojiCQCode() || b[1].Length == 2)) &&
                (destStr.StartsWith("#") || destStr.StartsWith("＃"));
        }

        public FunctionResult Progress(CQGroupMessageEventArgs e)//群聊处理
        {
            FunctionResult result = new FunctionResult
            {
                Result = true,
                SendFlag = true,
            };
            SendText sendText = new SendText
            {
                SendID = e.FromGroup,
            };
            string baseURL = "https://www.gstatic.com/android/keyboard/emojikitchen";
            string[] emojis = e.Message.Text.Replace("#", "").Replace("＃", "").Replace(" ", "").Split('+');
            string emojiA, emojiB;
            if(emojis[0].IsEmojiCQCode())
            {
                emojiA = emojis[0].GetEmojiID();
            } 
            else
            {
                emojiA = (0x10000 + (emojis[0][0] - 0xD800) * 0x400 + (emojis[0][1] - 0xDC00)).ToString("x0");
            }
            if(emojis[1].IsEmojiCQCode())
            {
                emojiB = emojis[1].GetEmojiID();
            } 
            else
            {
                emojiB = (0x10000 + (emojis[1][0] - 0xD800) * 0x400 + (emojis[1][1] - 0xDC00)).ToString("x0");
            }
            bool flag = false;
            string emojiLeft = "", emojiRight = "", date = "";
            if (MainSave.EmojiData.ContainsKey(emojiA))
            {
                JArray emoji = MainSave.EmojiData[emojiA] as JArray;
                foreach (JObject item in emoji)
                {
                    if (item["rightEmoji"].ToString() == emojiB || item["leftEmoji"].ToString() == emojiB)
                    {
                        flag = true;
                        emojiLeft = item["leftEmoji"].ToString();
                        emojiRight = item["rightEmoji"].ToString();
                        date = item["date"].ToString();
                        break;
                    }
                }
            }
            if (!flag)
            {
                sendText.MsgToSend.Add($"{emojis[0]}+{emojis[1]}=?");
            }
            else
            {
                if (emojiA == emojiB)
                {
                    emojiLeft = emojiA;
                    emojiRight = emojiA;
                }

                string url = $"{baseURL}/{date}/u{emojiLeft}/u{emojiLeft}_u{emojiRight}.png";
                using (HttpWebClient client = new HttpWebClient())
                {
                    string filename = $"u{emojiA}_u{emojiB}.png";
                    Directory.CreateDirectory(Path.Combine(MainSave.ImageDirectory, "EmojiMix"));
                    client.DownloadFile(url, Path.Combine(MainSave.ImageDirectory, "EmojiMix", filename));
                    sendText.MsgToSend.Add(CQApi.CQCode_Image(Path.Combine("EmojiMix", filename)).ToString());
                }
            }
            result.SendObject.Add(sendText);
            return result;
        }

        public FunctionResult Progress(CQPrivateMessageEventArgs e)//私聊处理
        {
            FunctionResult result = new FunctionResult
            {
                Result = true,
                SendFlag = true,
            };
            SendText sendText = new SendText
            {
                SendID = e.FromQQ,
            };

            sendText.MsgToSend.Add("这里输入需要发送的文本");
            result.SendObject.Add(sendText);
            return result;
        }
    }
}
