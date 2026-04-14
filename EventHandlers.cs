using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Cassie;
using PlayerRoles;

namespace CassiePlugin
{
    public class EventHandlers
    {
        private readonly Plugin plugin;
        private int currentReminderIndex = 0;
        private CoroutineHandle reminderCoroutine;
        private List<string> reminderKeys;
        private bool isMtfSpawning = false;
        private bool isChaosSpawning = false;
        private string pendingMtfMessage = "";
        private string pendingChaosMessage = "";
        private bool isPlayingCustomCassie = false;

        public EventHandlers(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            try
            {
                if (ev.NextKnownTeam == Faction.FoundationStaff && plugin.Config.EnableMtfCassie)
                {
                    if (plugin.Config.MtfCassieMessages.Count > 0)
                    {
                        string selectedName;

                        if (!string.IsNullOrEmpty(plugin.Config.SelectedMtfCassie) && 
                            plugin.Config.MtfCassieMessages.ContainsKey(plugin.Config.SelectedMtfCassie))
                        {
                            selectedName = plugin.Config.SelectedMtfCassie;
                            pendingMtfMessage = plugin.Config.MtfCassieMessages[selectedName];
                        }
                        else
                        {
                            var randomKey = plugin.Config.MtfCassieMessages.Keys.ElementAt(
                                UnityEngine.Random.Range(0, plugin.Config.MtfCassieMessages.Count));
                            selectedName = randomKey;
                            pendingMtfMessage = plugin.Config.MtfCassieMessages[randomKey];
                        }

                        isMtfSpawning = true;

                        if (plugin.Config.Debug)
                        {
                            Log.Debug($"Подготовлена кастомная CASSIE для МОГ '{selectedName}': {pendingMtfMessage}");
                        }
                    }
                }
                else if (ev.NextKnownTeam == Faction.FoundationEnemy && plugin.Config.EnableChaosCassie)
                {
                    if (plugin.Config.ChaosInsurgencyCassieMessages.Count > 0)
                    {
                        string selectedName;

                        if (!string.IsNullOrEmpty(plugin.Config.SelectedChaosCassie) && 
                            plugin.Config.ChaosInsurgencyCassieMessages.ContainsKey(plugin.Config.SelectedChaosCassie))
                        {
                            selectedName = plugin.Config.SelectedChaosCassie;
                            pendingChaosMessage = plugin.Config.ChaosInsurgencyCassieMessages[selectedName];
                        }
                        else
                        {
                            var randomKey = plugin.Config.ChaosInsurgencyCassieMessages.Keys.ElementAt(
                                UnityEngine.Random.Range(0, plugin.Config.ChaosInsurgencyCassieMessages.Count));
                            selectedName = randomKey;
                            pendingChaosMessage = plugin.Config.ChaosInsurgencyCassieMessages[randomKey];
                        }

                        isChaosSpawning = true;

                        if (plugin.Config.Debug)
                        {
                            Log.Debug($"Подготовлена кастомная CASSIE для Повстанцев '{selectedName}': {pendingChaosMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка в OnRespawningTeam: {ex}");
            }
        }

        public void OnSendingCassieMessage(SendingCassieMessageEventArgs ev)
        {
            try
            {
                // Если это наша кастомная CASSIE, пропускаем
                if (isPlayingCustomCassie)
                {
                    if (plugin.Config.Debug)
                    {
                        Log.Debug("Пропускаем проверку - это наша кастомная CASSIE");
                    }
                    isPlayingCustomCassie = false;
                    return;
                }

                if (plugin.Config.Debug)
                {
                    Log.Debug($"OnSendingCassieMessage вызван. Words: {ev.Words}, isMtfSpawning: {isMtfSpawning}, isChaosSpawning: {isChaosSpawning}");
                }

                // Проверяем, содержит ли сообщение ключевые слова респавна
                bool isMtfAnnouncement = ev.Words.Contains("CONTAINMENTUNIT") || ev.Words.Contains("MTFUNIT");
                bool isChaosAnnouncement = ev.Words.Contains("CHAOSINSURGENCY");

                if (isMtfSpawning && isMtfAnnouncement && !string.IsNullOrEmpty(pendingMtfMessage))
                {
                    ev.IsAllowed = false;
                    isMtfSpawning = false;
                    string messageToPlay = pendingMtfMessage;
                    pendingMtfMessage = "";

                    if (plugin.Config.Debug)
                    {
                        Log.Debug($"Отменена стандартная CASSIE МОГ. Воспроизводим: {messageToPlay}");
                    }
                    
                    Timing.CallDelayed(0.5f, () =>
                    {
                        isPlayingCustomCassie = true;
                        Exiled.API.Features.Cassie.Message(messageToPlay, false, true, false);
                        if (plugin.Config.Debug)
                        {
                            Log.Debug($"Воспроизведена кастомная CASSIE для МОГ");
                        }
                    });
                }
                else if (isChaosSpawning && isChaosAnnouncement && !string.IsNullOrEmpty(pendingChaosMessage))
                {
                    ev.IsAllowed = false;
                    isChaosSpawning = false;
                    string messageToPlay = pendingChaosMessage;
                    pendingChaosMessage = "";

                    if (plugin.Config.Debug)
                    {
                        Log.Debug($"Отменена стандартная CASSIE Повстанцев. Воспроизводим: {messageToPlay}");
                    }
                    
                    Timing.CallDelayed(0.5f, () =>
                    {
                        isPlayingCustomCassie = true;
                        Exiled.API.Features.Cassie.Message(messageToPlay, false, true, false);
                        if (plugin.Config.Debug)
                        {
                            Log.Debug($"Воспроизведена кастомная CASSIE для Повстанцев");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка в OnSendingCassieMessage: {ex}");
            }
        }

        public void OnRoundStarted()
        {
            if (plugin.Config.EnableReminders && plugin.Config.ReminderCassieMessages.Count > 0)
            {
                reminderKeys = plugin.Config.ReminderCassieMessages.Keys.ToList();
                currentReminderIndex = 0;
                reminderCoroutine = Timing.RunCoroutine(ReminderCoroutine());
                
                if (plugin.Config.Debug)
                {
                    Log.Debug("Запущен корутин напоминаний CASSIE");
                }
            }
        }

        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            if (reminderCoroutine.IsRunning)
            {
                Timing.KillCoroutines(reminderCoroutine);
                
                if (plugin.Config.Debug)
                {
                    Log.Debug("Остановлен корутин напоминаний CASSIE");
                }
            }

            currentReminderIndex = 0;
        }

        private IEnumerator<float> ReminderCoroutine()
        {
            if (plugin.Config.Debug)
            {
                Log.Debug($"Корутин напоминаний запущен. Первая задержка: {plugin.Config.FirstReminderDelay}с");
            }

            yield return Timing.WaitForSeconds(plugin.Config.FirstReminderDelay);

            while (Round.IsStarted)
            {
                if (reminderKeys != null && reminderKeys.Count > 0)
                {
                    string key = reminderKeys[currentReminderIndex];
                    string message = plugin.Config.ReminderCassieMessages[key];
                    
                    if (plugin.Config.Debug)
                    {
                        Log.Debug($"Попытка воспроизвести напоминание '{key}': {message}");
                    }

                    Exiled.API.Features.Cassie.Message(message, false, true, false);

                    currentReminderIndex = (currentReminderIndex + 1) % reminderKeys.Count;

                    if (plugin.Config.Debug)
                    {
                        Log.Debug($"Воспроизведено напоминание CASSIE '{key}'. Следующее через {plugin.Config.ReminderInterval}с");
                    }
                }
                else if (plugin.Config.Debug)
                {
                    Log.Debug("Список напоминаний пуст!");
                }

                yield return Timing.WaitForSeconds(plugin.Config.ReminderInterval);
            }

            if (plugin.Config.Debug)
            {
                Log.Debug("Корутин напоминаний завершен (раунд закончился)");
            }
        }
    }
}
