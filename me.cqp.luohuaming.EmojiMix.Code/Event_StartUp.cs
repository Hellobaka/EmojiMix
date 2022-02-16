using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using me.cqp.luohuaming.EmojiMix.Code.OrderFunctions;
using me.cqp.luohuaming.EmojiMix.Sdk.Cqp.EventArgs;
using me.cqp.luohuaming.EmojiMix.Sdk.Cqp.Interface;
using me.cqp.luohuaming.EmojiMix.PublicInfos;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.IO;

namespace me.cqp.luohuaming.EmojiMix.Code
{
    public class Event_StartUp : ICQStartup
    {
        public void CQStartup(object sender, CQStartupEventArgs e)
        {
            MainSave.AppDirectory = e.CQApi.AppDirectory;
            MainSave.CQApi = e.CQApi;
            MainSave.CQLog = e.CQLog;
            MainSave.ImageDirectory = CommonHelper.GetAppImageDirectory();
            string emojiDataPath = Path.Combine(MainSave.AppDirectory, "emojiData.json");
            if (File.Exists(emojiDataPath))
                MainSave.EmojiData = JObject.Parse(File.ReadAllText(emojiDataPath));
            else
                MainSave.CQLog.Warning("缺少emojiData.json");
            //这里写处理逻辑
            //MainSave.Instances.Add(new ExampleFunction());//这里需要将指令实例化填在这里
            foreach (var item in Assembly.GetAssembly(typeof(Event_GroupMessage)).GetTypes())
            {
                if (item.IsInterface)
                    continue;
                foreach (var instance in item.GetInterfaces())
                {
                    if (instance == typeof(IOrderModel))
                    {
                        IOrderModel obj = (IOrderModel)Activator.CreateInstance(item);
                        if (obj.ImplementFlag == false)
                            break;
                        MainSave.Instances.Add(obj);
                    }
                }
            }
        }
    }
}
