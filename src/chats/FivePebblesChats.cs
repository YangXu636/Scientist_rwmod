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
                self.dialogBox.Interrupt(self.Translate("�ҡ�����ܾ��ˣ������ġ���"), 60);
                self.dialogBox.NewMessage(self.Translate("�������ͬ�಻ͬ�������ġ�������"), 60);
                self.dialogBox.NewMessage(self.Translate("�������Ѿ������һЩ���ǵĻ������֣���Ȼ����԰���Щ���ݷ�������ͬ��"), 60);
                self.dialogBox.NewMessage(self.Translate("��Ȼ�ݻ�һ����˵���ᵮ�������������ĸ��壬����ĳ��ִ������������"), 60);
                self.dialogBox.NewMessage(self.Translate("���Ҹе�����"), 60);
                self.dialogBox.NewMessage(self.Translate("�Ҳ�֪����Ĵ���Ϊ�λ���˷����������������϶�����ЩĿ��"), 60);
                self.dialogBox.NewMessage(self.Translate("�������������Ϊ����Ԫ�Ļ����Ҳ������������ҵ���ʩ��ȡ"), 60);
                self.dialogBox.NewMessage(self.Translate("�ҵ���������Լ���˵�������ж���⣬�Ҳ�֪���һ���֧�Ŷ��"), 60);
                self.dialogBox.NewMessage(self.Translate("��������"), 60);
                self.dialogBox.NewMessage(self.Translate("�Ҷ��˵��ֽţ���Ӧ������������һ��"), 60);
                self.dialogBox.NewMessage(self.Translate("�һ�����۲���ģ������ڲ�������ʲô���ɣ��뿪"), 60);
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
                self.dialogBox.NewMessage(self.Translate("�ҡ�����ܾ��ˣ������ġ���"), 60);
                self.dialogBox.NewMessage(self.Translate("�������ͬ�಻ͬ�������ġ�������"), 60);
                self.dialogBox.NewMessage(self.Translate("�������Ѿ������һЩ���ǵĻ������֣���Ȼ����԰���Щ���ݷ�������ͬ��"), 60);
                self.dialogBox.NewMessage(self.Translate("��Ȼ�ݻ�һ����˵���ᵮ�������������ĸ��壬����ĳ��ִ������������"), 60);
                self.dialogBox.NewMessage(self.Translate("���Ҹе�����"), 60);
                self.dialogBox.NewMessage(self.Translate("�Ҳ�֪����Ĵ���Ϊ�λ���˷����������������϶�����ЩĿ��"), 60);
                self.dialogBox.NewMessage(self.Translate("�������������Ϊ����Ԫ�Ļ����Ҳ������������ҵ���ʩ��ȡ"), 60);
                self.dialogBox.NewMessage(self.Translate("�ҵ���������Լ���˵�������ж���⣬�Ҳ�֪���һ���֧�Ŷ��"), 60);
                self.dialogBox.NewMessage(self.Translate("��������"), 60);
                self.dialogBox.NewMessage(self.Translate("�Ҷ��˵��ֽţ���Ӧ������������һ��"), 60);
                self.dialogBox.NewMessage(self.Translate("�һ�����۲���ģ������ڲ�������ʲô���ɣ��뿪"), 60);
                return;
            }
        }
        orig(self);
    }

    public static void GetMethodInfo(int index)
    {
        index++;//�������Ƿ�װ�˷������൱���϶���Ҫ��ȡ������ʵ����������ԣ��϶˵ı������������϶ˣ�������Ҫ+1���Դ�����
        var stack = new StackTrace(true);

        //0�Ǳ���1�ǵ��÷���2�ǵ��÷��ĵ��÷�...�Դ�����
        var method = stack.GetFrame(index).GetMethod();//��Ҫ��ȡ���ڷ�������Ϣ�������Լ��ϵ��������

        var dataList = new Dictionary<string, string>();
        var module = method.Module;
        dataList.Add("ģ��", module.Name);
        var deClearType = method.DeclaringType;
        dataList.Add("�����ռ�", deClearType.Namespace);
        dataList.Add("����", deClearType.Name);
        dataList.Add("��������", deClearType.FullName);
        dataList.Add("������", method.Name);
        dataList.Add("����", stack.GetFrame(index).GetFileLineNumber().ToString());
        var stackFrames = stack.GetFrames();
        dataList.Add("������", string.Join("\n -> ", stackFrames.Select((r, i) =>
        {
            if (i == 0) return null;
            var m = r.GetMethod();
            return $"{m.DeclaringType.FullName}.{m.Name} Line {r.GetFileLineNumber()}";
        }).Where(r => !string.IsNullOrWhiteSpace(r)).Reverse()));

        /*dataList.ForeachLingbug(r =>
        {
            Console.WriteLine($"{r.Key}��{r.Value}");
        });*/

        foreach (var item in dataList)
        {
            Console.WriteLine($"{item.Key}��{item.Value}");
        }
    }
}