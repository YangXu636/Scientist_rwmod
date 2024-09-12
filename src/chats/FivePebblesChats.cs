using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Scientist;

namespace chats;
public class FivePebblesChats
{
    public static void FivePebbles_Update(On.SSOracleBehavior.orig_Update orig, SSOracleBehavior self, bool eu)
    {
        /*Console.WriteLine($"FivePebblesChats_Update  self = {self}, eu = {eu}");
        Console.WriteLine($"FivePebblesChats_Update  self.action = {self.action}");*/
        orig(self, eu);
    }

    public static void FivePebbles_InitateConversationInitiateConversation(On.SSOracleBehavior.orig_InitateConversation orig, SSOracleBehavior self, Conversation.ID convoId, SSOracleBehavior.ConversationBehavior convBehav)
    {
        Console.WriteLine($"FivePebblesChats_InitateConversation  self = {self}, convoId = {convoId}, convBehav = {convBehav}");
        foreach (Player player in self.PlayersInRoom)
        {
            if (player.slugcatStats.name.value == Scientist.Plugin.MOD_ID)
            {
                self.dialogBox.NewMessage(self.Translate("TEST_MESSAGE"), 60);
                self.dialogBox.Interrupt(self.Translate("我■■你很久了，■■的■■"), 60);
                self.dialogBox.NewMessage(self.Translate("你与你的同类不同，你格外的……聪明"), 60);
                self.dialogBox.NewMessage(self.Translate("你甚至已经理解了一些它们的基础文字，当然你可以把这些内容翻译给你的同类"), 60);
                self.dialogBox.NewMessage(self.Translate("自然演化一般来说不会诞生出像你这样的个体，但你的出现打破了这个概念"), 60);
                self.dialogBox.NewMessage(self.Translate("令我感到好奇"), 60);
                self.dialogBox.NewMessage(self.Translate("我不知道你的大脑为何会如此发达，但你现在来这里肯定是有些目的"), 60);
                self.dialogBox.NewMessage(self.Translate("如果你来这里是为了神经元的话，我不建议你深入我的设施里取"), 60);
                self.dialogBox.NewMessage(self.Translate("我的情况连我自己都说不上来有多糟糕，我不知道我还能支撑多久"), 60);
                self.dialogBox.NewMessage(self.Translate("让我想想"), 60);
                self.dialogBox.NewMessage(self.Translate("我动了点手脚，这应该能让你轻松一点"), 60);
                self.dialogBox.NewMessage(self.Translate("我会继续观察你的，但现在不管你有什么理由，离开"), 60);
            }
        }
    }

    public static void FivePebbles_AddEvents(On.SSOracleBehavior.PebblesConversation.orig_AddEvents orig, SSOracleBehavior.PebblesConversation self)
    {
        GetMethodInfo(6);
        Console.WriteLine($"FivePebblesChats_AddEvents  self = {self}");
        foreach (Player player in self.owner.PlayersInRoom)
        {
            if (player.slugcatStats.name.value == Scientist.Plugin.MOD_ID)
            { 
                self.dialogBox.NewMessage(self.Translate("我■■你很久了，■■的■■"), 60);
                self.dialogBox.NewMessage(self.Translate("你与你的同类不同，你格外的……聪明"), 60);
                self.dialogBox.NewMessage(self.Translate("你甚至已经理解了一些它们的基础文字，当然你可以把这些内容翻译给你的同类"), 60);
                self.dialogBox.NewMessage(self.Translate("自然演化一般来说不会诞生出像你这样的个体，但你的出现打破了这个概念"), 60);
                self.dialogBox.NewMessage(self.Translate("令我感到好奇"), 60);
                self.dialogBox.NewMessage(self.Translate("我不知道你的大脑为何会如此发达，但你现在来这里肯定是有些目的"), 60);
                self.dialogBox.NewMessage(self.Translate("如果你来这里是为了神经元的话，我不建议你深入我的设施里取"), 60);
                self.dialogBox.NewMessage(self.Translate("我的情况连我自己都说不上来有多糟糕，我不知道我还能支撑多久"), 60);
                self.dialogBox.NewMessage(self.Translate("让我想想"), 60);
                self.dialogBox.NewMessage(self.Translate("我动了点手脚，这应该能让你轻松一点"), 60);
                self.dialogBox.NewMessage(self.Translate("我会继续观察你的，但现在不管你有什么理由，离开"), 60);
                return;
            }
        }
        orig(self);
    }

    public static void GetMethodInfo(int index)
    {
        index++;//由于我是封装了方法，相当于上端想要获取本身，其实对于这里而言，上端的本身就是这里的上端，所以需要+1，以此类推
        var stack = new StackTrace(true);

        //0是本身，1是调用方，2是调用方的调用方...以此类推
        var method = stack.GetFrame(index).GetMethod();//想要获取关于方法的信息，可以自己断点调试这里

        var dataList = new Dictionary<string, string>();
        var module = method.Module;
        dataList.Add("模块", module.Name);
        var deClearType = method.DeclaringType;
        dataList.Add("命名空间", deClearType.Namespace);
        dataList.Add("类名", deClearType.Name);
        dataList.Add("完整类名", deClearType.FullName);
        dataList.Add("方法名", method.Name);
        dataList.Add("行数", stack.GetFrame(index).GetFileLineNumber().ToString());
        var stackFrames = stack.GetFrames();
        dataList.Add("调用链", string.Join("\n -> ", stackFrames.Select((r, i) =>
        {
            if (i == 0) return null;
            var m = r.GetMethod();
            return $"{m.DeclaringType.FullName}.{m.Name} Line {r.GetFileLineNumber()}";
        }).Where(r => !string.IsNullOrWhiteSpace(r)).Reverse()));

        /*dataList.ForeachLingbug(r =>
        {
            Console.WriteLine($"{r.Key}：{r.Value}");
        });*/

        foreach (var item in dataList)
        {
            Console.WriteLine($"{item.Key}：{item.Value}");
        }
    }
}