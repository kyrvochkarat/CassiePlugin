[README.md](https://github.com/user-attachments/files/26701098/README.md)
# CassiePlugin

Плагин для SCP: Secret Laboratory с фреймворком Exiled/ExMod, позволяющий настраивать кастомные CASSIE объявления.

## Возможности

- 🎙️ Кастомные CASSIE для прибытия МОГ (Mobile Task Force)
- 🎙️ Кастомные CASSIE для прибытия Повстанцев (Chaos Insurgency)
- ⏰ Периодические напоминания CASSIE в течение раунда
- 🎯 Именованные CASSIE для удобного выбора
- 🎮 Команды для управления через админ-консоль

## Установка

1. Скачай последнюю версию Exiled/ExMod
2. Скомпилируй плагин или скачай готовый .dll
3. Помести `CassiePlugin.dll` в папку `EXILED/Plugins`
4. Перезапусти сервер

## Конфигурация

Конфиг находится в `EXILED/Configs/{port}-config.yml`

```yaml
cassie_plugin:
  is_enabled: true
  debug: false
  
  # Кастомные CASSIE для МОГ
  mtf_cassie_messages:
    standard: "MtfUnit epsilon 11 designated NineTailedFox HasEntered AllRemaining AwaitingRecontainment"
    evacuation: "MtfUnit epsilon 11 HasEntered . . . Attention AllRemaining . . . Proceed to evacuation shelter"
    containment: "MtfUnit epsilon 11 HasEntered . . . SCPSubjects AwaitingRecontainment"
  
  # Кастомные CASSIE для Повстанцев
  chaos_insurgency_cassie_messages:
    standard: "ChaosInsurgency HasEntered AllRemaining AwaitingRecontainment"
    evacuation: "Attention . . . ChaosInsurgency HasEntered . . . AllRemaining PleaseEvacuate"
    unauthorized: "Unauthorized personnel detected . . . ChaosInsurgency forces in facility"
  
  # Напоминания CASSIE
  reminder_cassie_messages:
    surroundings: "Attention . . . All personnel . . . Remember to check your surroundings"
    lockdown: "Reminder . . . Facility lockdown procedures are in effect"
    containment: "Attention . . . SCPSubjects must be contained at all costs"
  
  enable_mtf_cassie: true
  enable_chaos_cassie: true
  enable_reminders: true
  
  selected_mtf_cassie: ""  # Оставь пустым для случайного выбора
  selected_chaos_cassie: ""  # Оставь пустым для случайного выбора
  
  reminder_interval: 180  # Интервал между напоминаниями (секунды)
  first_reminder_delay: 120  # Задержка перед первым напоминанием (секунды)
```

## Команды

Все команды используются в Remote Admin консоли. Требуется право `cassie.mode`.

### Основные команды

- `cassiemode status` - Показать текущий статус плагина
- `cassiemode mtf <действие>` - Управление CASSIE для МОГ
- `cassiemode chaos <действие>` - Управление CASSIE для Повстанцев
- `cassiemode reminders <действие>` - Управление напоминаниями
- `cassiemode play <тип> <имя>` - Принудительно воспроизвести CASSIE

### Действия для МОГ/Повстанцев

- `on` - Включить кастомные CASSIE
- `off` - Выключить кастомные CASSIE
- `list` - Показать список доступных CASSIE
- `set <имя>` - Выбрать конкретную CASSIE
- `random` - Включить случайный выбор

### Примеры

```
cassiemode status
cassiemode mtf list
cassiemode mtf set evacuation
cassiemode chaos on
cassiemode play mtf standard
cassiemode reminders off
```

## Разработка

### Требования

- Visual Studio 2022
- .NET Framework 4.8
- Exiled/ExMod 9.13.1+

### Компиляция

1. Открой `CassiePlugin.slnx` в Visual Studio 2022
2. Восстанови NuGet пакеты
3. Build → Rebuild Solution
4. Скомпилированный .dll будет в `bin/Debug` или `bin/Release`

## Лицензия

MIT License

## Автор

vityanvsk
