using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;

namespace CassiePlugin.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CassieCommand : ICommand
    {
        public string Command => "cassiemode";

        public string[] Aliases => new[] { "cmode" };

        public string Description => "Управление режимами кастомных CASSIE для МОГ и Повстанцев";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("cassie.mode"))
            {
                response = "У вас нет прав для использования этой команды!";
                return false;
            }

            var plugin = Plugin.Instance;
            if (plugin == null)
            {
                response = "Плагин не загружен!";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Использование:\n" +
                          "cassiemode mtf <on/off/set/list/random> [имя] - Управление CASSIE для МОГ\n" +
                          "cassiemode chaos <on/off/set/list/random> [имя] - Управление CASSIE для Повстанцев\n" +
                          "cassiemode reminders <on/off/list> - Управление напоминаниями CASSIE\n" +
                          "cassiemode play <mtf/chaos/reminder> <имя> - Принудительно воспроизвести CASSIE\n" +
                          "cassiemode status - Показать текущий статус";
                return false;
            }

            string mode = arguments.At(0).ToLower();

            if (mode == "status")
            {
                response = $"Статус CassiePlugin:\n" +
                          $"МОГ CASSIE: {(plugin.Config.EnableMtfCassie ? "Включено" : "Выключено")} " +
                          $"(Выбрано: {(string.IsNullOrEmpty(plugin.Config.SelectedMtfCassie) ? "случайное" : plugin.Config.SelectedMtfCassie)}, " +
                          $"Всего: {plugin.Config.MtfCassieMessages.Count})\n" +
                          $"Повстанцы CASSIE: {(plugin.Config.EnableChaosCassie ? "Включено" : "Выключено")} " +
                          $"(Выбрано: {(string.IsNullOrEmpty(plugin.Config.SelectedChaosCassie) ? "случайное" : plugin.Config.SelectedChaosCassie)}, " +
                          $"Всего: {plugin.Config.ChaosInsurgencyCassieMessages.Count})\n" +
                          $"Напоминания: {(plugin.Config.EnableReminders ? "Включено" : "Выключено")} " +
                          $"(Всего: {plugin.Config.ReminderCassieMessages.Count}, Интервал: {plugin.Config.ReminderInterval}с)";
                return true;
            }

            if (arguments.Count < 2)
            {
                response = "Недостаточно аргументов! Используйте: cassiemode <режим> <действие> [параметры]";
                return false;
            }

            string action = arguments.At(1).ToLower();

            switch (mode)
            {
                case "mtf":
                    return HandleMtfCommand(plugin, action, arguments, out response);

                case "chaos":
                    return HandleChaosCommand(plugin, action, arguments, out response);

                case "reminders":
                    return HandleRemindersCommand(plugin, action, out response);

                case "play":
                    return HandlePlayCommand(plugin, action, arguments, out response);

                default:
                    response = "Неверный режим! Используйте: mtf, chaos, reminders, play или status";
                    return false;
            }
        }

        private bool HandleMtfCommand(Plugin plugin, string action, ArraySegment<string> arguments, out string response)
        {
            switch (action)
            {
                case "on":
                    plugin.Config.EnableMtfCassie = true;
                    response = "Кастомные CASSIE для МОГ включены!";
                    return true;

                case "off":
                    plugin.Config.EnableMtfCassie = false;
                    response = "Кастомные CASSIE для МОГ выключены!";
                    return true;

                case "list":
                    if (plugin.Config.MtfCassieMessages.Count == 0)
                    {
                        response = "Нет доступных CASSIE для МОГ!";
                        return false;
                    }
                    response = "Доступные CASSIE для МОГ:\n" + 
                              string.Join("\n", plugin.Config.MtfCassieMessages.Keys.Select(k => $"- {k}"));
                    return true;

                case "set":
                    if (arguments.Count < 3)
                    {
                        response = "Укажите имя CASSIE! Используйте: cassiemode mtf set <имя>";
                        return false;
                    }
                    string mtfName = arguments.At(2);
                    if (!plugin.Config.MtfCassieMessages.ContainsKey(mtfName))
                    {
                        response = $"CASSIE с именем '{mtfName}' не найдена! Используйте 'cassiemode mtf list' для просмотра доступных.";
                        return false;
                    }
                    plugin.Config.SelectedMtfCassie = mtfName;
                    response = $"Выбрана CASSIE для МОГ: {mtfName}";
                    return true;

                case "random":
                    plugin.Config.SelectedMtfCassie = "";
                    response = "Установлен случайный выбор CASSIE для МОГ";
                    return true;

                default:
                    response = "Неверное действие! Используйте: on, off, set, list, random";
                    return false;
            }
        }

        private bool HandleChaosCommand(Plugin plugin, string action, ArraySegment<string> arguments, out string response)
        {
            switch (action)
            {
                case "on":
                    plugin.Config.EnableChaosCassie = true;
                    response = "Кастомные CASSIE для Повстанцев включены!";
                    return true;

                case "off":
                    plugin.Config.EnableChaosCassie = false;
                    response = "Кастомные CASSIE для Повстанцев выключены!";
                    return true;

                case "list":
                    if (plugin.Config.ChaosInsurgencyCassieMessages.Count == 0)
                    {
                        response = "Нет доступных CASSIE для Повстанцев!";
                        return false;
                    }
                    response = "Доступные CASSIE для Повстанцев:\n" + 
                              string.Join("\n", plugin.Config.ChaosInsurgencyCassieMessages.Keys.Select(k => $"- {k}"));
                    return true;

                case "set":
                    if (arguments.Count < 3)
                    {
                        response = "Укажите имя CASSIE! Используйте: cassiemode chaos set <имя>";
                        return false;
                    }
                    string chaosName = arguments.At(2);
                    if (!plugin.Config.ChaosInsurgencyCassieMessages.ContainsKey(chaosName))
                    {
                        response = $"CASSIE с именем '{chaosName}' не найдена! Используйте 'cassiemode chaos list' для просмотра доступных.";
                        return false;
                    }
                    plugin.Config.SelectedChaosCassie = chaosName;
                    response = $"Выбрана CASSIE для Повстанцев: {chaosName}";
                    return true;

                case "random":
                    plugin.Config.SelectedChaosCassie = "";
                    response = "Установлен случайный выбор CASSIE для Повстанцев";
                    return true;

                default:
                    response = "Неверное действие! Используйте: on, off, set, list, random";
                    return false;
            }
        }

        private bool HandleRemindersCommand(Plugin plugin, string action, out string response)
        {
            switch (action)
            {
                case "on":
                    plugin.Config.EnableReminders = true;
                    response = "Напоминания CASSIE включены!";
                    return true;

                case "off":
                    plugin.Config.EnableReminders = false;
                    response = "Напоминания CASSIE выключены!";
                    return true;

                case "list":
                    if (plugin.Config.ReminderCassieMessages.Count == 0)
                    {
                        response = "Нет доступных напоминаний CASSIE!";
                        return false;
                    }
                    response = "Доступные напоминания CASSIE:\n" + 
                              string.Join("\n", plugin.Config.ReminderCassieMessages.Keys.Select(k => $"- {k}"));
                    return true;

                default:
                    response = "Неверное действие! Используйте: on, off, list";
                    return false;
            }
        }

        private bool HandlePlayCommand(Plugin plugin, string type, ArraySegment<string> arguments, out string response)
        {
            if (arguments.Count < 3)
            {
                response = "Использование: cassiemode play <mtf/chaos/reminder> <имя>";
                return false;
            }

            string name = arguments.At(2);

            switch (type.ToLower())
            {
                case "mtf":
                    if (!plugin.Config.MtfCassieMessages.ContainsKey(name))
                    {
                        response = $"CASSIE с именем '{name}' не найдена в списке МОГ!";
                        return false;
                    }
                    Exiled.API.Features.Cassie.Message(plugin.Config.MtfCassieMessages[name], false, true, false);
                    response = $"Воспроизведена CASSIE для МОГ: {name}";
                    return true;

                case "chaos":
                    if (!plugin.Config.ChaosInsurgencyCassieMessages.ContainsKey(name))
                    {
                        response = $"CASSIE с именем '{name}' не найдена в списке Повстанцев!";
                        return false;
                    }
                    Exiled.API.Features.Cassie.Message(plugin.Config.ChaosInsurgencyCassieMessages[name], false, true, false);
                    response = $"Воспроизведена CASSIE для Повстанцев: {name}";
                    return true;

                case "reminder":
                    if (!plugin.Config.ReminderCassieMessages.ContainsKey(name))
                    {
                        response = $"Напоминание с именем '{name}' не найдено!";
                        return false;
                    }
                    Exiled.API.Features.Cassie.Message(plugin.Config.ReminderCassieMessages[name], false, true, false);
                    response = $"Воспроизведено напоминание: {name}";
                    return true;

                default:
                    response = "Неверный тип! Используйте: mtf, chaos или reminder";
                    return false;
            }
        }
    }
}
