using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;

namespace CassiePlugin
{
    public class Config : IConfig
    {
        [Description("Включен ли плагин?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Показывать ли отладочные сообщения?")]
        public bool Debug { get; set; } = false;

        [Description("Словарь кастомных CASSIE для МОГ (Имя -> Сообщение)")]
        public Dictionary<string, string> MtfCassieMessages { get; set; } = new Dictionary<string, string>
        {
            { "standard", "MtfUnit epsilon 11 designated NineTailedFox HasEntered AllRemaining AwaitingRecontainment" },
            { "evacuation", "MtfUnit epsilon 11 HasEntered . . . Attention AllRemaining . . . Proceed to evacuation shelter" },
            { "containment", "MtfUnit epsilon 11 HasEntered . . . SCPSubjects AwaitingRecontainment" }
        };

        [Description("Словарь кастомных CASSIE для Повстанцев (Имя -> Сообщение)")]
        public Dictionary<string, string> ChaosInsurgencyCassieMessages { get; set; } = new Dictionary<string, string>
        {
            { "standard", "ChaosInsurgency HasEntered AllRemaining AwaitingRecontainment" },
            { "evacuation", "Attention . . . ChaosInsurgency HasEntered . . . AllRemaining PleaseEvacuate" },
            { "unauthorized", "Unauthorized personnel detected . . . ChaosInsurgency forces in facility" }
        };

        [Description("Словарь напоминаний CASSIE (Имя -> Сообщение)")]
        public Dictionary<string, string> ReminderCassieMessages { get; set; } = new Dictionary<string, string>
        {
            { "surroundings", "Attention . . . All personnel . . . Remember to check your surroundings" },
            { "lockdown", "Reminder . . . Facility lockdown procedures are in effect" },
            { "containment", "Attention . . . SCPSubjects must be contained at all costs" }
        };

        [Description("Включить ли кастомные CASSIE для МОГ?")]
        public bool EnableMtfCassie { get; set; } = true;

        [Description("Включить ли кастомные CASSIE для Повстанцев?")]
        public bool EnableChaosCassie { get; set; } = true;

        [Description("Включить ли напоминания CASSIE?")]
        public bool EnableReminders { get; set; } = true;

        [Description("Текущее выбранное имя CASSIE для МОГ (оставьте пустым для случайного выбора)")]
        public string SelectedMtfCassie { get; set; } = "";

        [Description("Текущее выбранное имя CASSIE для Повстанцев (оставьте пустым для случайного выбора)")]
        public string SelectedChaosCassie { get; set; } = "";

        [Description("Интервал между напоминаниями CASSIE (в секундах)")]
        public float ReminderInterval { get; set; } = 180f;

        [Description("Задержка перед первым напоминанием после начала раунда (в секундах)")]
        public float FirstReminderDelay { get; set; } = 120f;
    }
}
