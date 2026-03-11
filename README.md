# TicTacToe Embedded Module

## 1. Описание задачи

Тестовое задание: реализовать миниигру "Крестики-нолики" как встраиваемый модуль для Unity.

Требования к модулю:
- запуск из внешнего кода, а не только как отдельная стартовая сцена;
- возврат результата наружу после завершения матча;
- простая механика награды;
- использование `Zenject`, `UniTask`/`UniRx`, `Addressables`;
- чистая модульная архитектура без завязки на конкретную сцену.

В проекте модуль реализован как переиспользуемый UI-компонент, который монтируется в переданный контейнер `Transform`/`RectTransform`.

## 2. Архитектура

Модуль разделен на слои:

- `Api`  
  Публичные контракты для внешнего кода: запуск, параметры запуска, результат, награда.
- `Application`  
  Оркестрация одной игровой сессии и фасад модуля.
- `Domain`  
  Чистая игровая логика: поле `3x3`, правила, валидация ходов, AI, расчет награды.
- `Presentation`  
  View и Presenter для UI. Игровая логика не зависит от Unity UI.
- `Infrastructure`  
  Загрузка и создание UI через `Addressables`, управление lifecycle экрана.
- `Config`  
  `ScriptableObject` и сериализуемые настройки наград/таймингов.
- `Installers`  
  Интеграция через `Zenject`.
- `Demo`  
  Демонстрационный host-сценарий, который показывает встраиваемость модуля.

Ключевые принципы:

- внешний код знает только про `ITicTacToeLauncher` и/или `ITicTacToeModule`;
- UI отделен от игровой логики;
- модуль не использует `Service Locator`;
- один запуск модуля соответствует одной игровой сессии;
- сам экран миниигры может загружаться как `Addressable prefab`, но внешний API от этого не меняется.

## 3. Структура проекта

```text
Assets/
  GameModules/
    TicTacToe/
      Content/
        Prefabs/                  # Addressable prefab экрана, создается editor utility
        ScriptableObjects/
          TicTacToeModuleSettings.asset
      Editor/
        TicTacToeAddressablesSetupUtility.cs
      Runtime/
        Api/
        Application/
        Config/
        Demo/
        Domain/
        Infrastructure/
        Installers/
        Presentation/
        TicTacToeModule.Runtime.asmdef
      Tests/
        EditMode/
  Scenes/
    SampleScene.unity
```

Основные runtime-файлы:

- `Runtime/Api/ITicTacToeLauncher.cs`
- `Runtime/Api/ITicTacToeModule.cs`
- `Runtime/Application/TicTacToeModuleService.cs`
- `Runtime/Application/TicTacToeSessionController.cs`
- `Runtime/Domain/TicTacToeRules.cs`
- `Runtime/Domain/SimpleTicTacToeBotStrategy.cs`
- `Runtime/Domain/DefaultTicTacToeRewardCalculator.cs`
- `Runtime/Infrastructure/TicTacToeScreenFactory.cs`
- `Runtime/Presentation/TicTacToeScreenView.cs`
- `Runtime/Presentation/TicTacToePresenter.cs`
- `Runtime/Demo/TicTacToeDemoEntryPoint.cs`
- `Runtime/Demo/TicTacToeDemoInstaller.cs`

## 4. Как запустить проект

### Первый запуск

1. Открыть проект в Unity.
2. Открыть сцену `Assets/Scenes/SampleScene.unity`.
3. Выполнить меню:
   `Tools/GameModules/TicTacToe/Setup Addressables Content`
4. Собрать addressables:
   `Addressables -> Build -> New Build -> Default Build Script`
5. Убедиться, что в сцене есть `SceneContext`, а demo-объекты не удалены.
6. Нажать `Play`.

### Что делает setup utility

Editor utility:

- создает `Assets/GameModules/TicTacToe/Content/Prefabs/TicTacToeScreen.prefab`;
- создает или обновляет `Assets/GameModules/TicTacToe/Content/ScriptableObjects/TicTacToeModuleSettings.asset`;
- добавляет prefab в Addressables;
- назначает ссылку на prefab в `TicTacToeModuleSettings`.

Если addressable prefab еще не создан, модуль умеет собрать runtime UI fallback, но для сдачи тестового рекомендуется выполнить полный setup через utility.

## 5. Как вызвать миниигру из внешнего кода

### Публичная точка входа

Основной внешний контракт:

```csharp
public interface ITicTacToeLauncher
{
    UniTask<TicTacToeResult> LaunchAsync(
        TicTacToeLaunchOptions options,
        CancellationToken cancellationToken = default);
}
```

Расширенный lifecycle API:

```csharp
public interface ITicTacToeModule
{
    bool IsRunning { get; }

    UniTask PreloadAsync(CancellationToken cancellationToken = default);

    UniTask<TicTacToeResult> RunAsync(
        TicTacToeLaunchOptions options,
        CancellationToken cancellationToken = default);

    void Release();
}
```

### Регистрация в Zenject

В хостовом installer:

```csharp
public override void InstallBindings()
{
    TicTacToeModuleBindings.Install(Container, moduleSettings);
}
```

Либо через `TicTacToeModuleInstaller`, если удобнее использовать `MonoInstaller`.

### Пример вызова

```csharp
public sealed class DailyBonusService
{
    private readonly ITicTacToeLauncher _ticTacToeLauncher;

    public DailyBonusService(ITicTacToeLauncher ticTacToeLauncher)
    {
        _ticTacToeLauncher = ticTacToeLauncher;
    }

    public async UniTask<TicTacToeResult> OpenMiniGameAsync(
        RectTransform host,
        CancellationToken cancellationToken = default)
    {
        return await _ticTacToeLauncher.LaunchAsync(
            new TicTacToeLaunchOptions
            {
                Parent = host,
                SessionId = "daily-bonus",
                AutoCloseOnFinish = true,
                AllowRestart = false
            },
            cancellationToken);
    }
}
```

Ключевой параметр запуска: `Parent`.  
Именно в этот контейнер будет встроен UI миниигры.

## 6. Как выглядит возвращаемый результат

Результат возвращается как `UniTask<TicTacToeResult>`.

```csharp
public sealed class TicTacToeResult
{
    public string SessionId { get; }
    public TicTacToeOutcome Outcome { get; }
    public TicTacToeRewardData Reward { get; }
    public int TurnsCount { get; }
    public IReadOnlyList<BoardPosition> WinningCells { get; }
    public bool WasCancelled { get; }
}
```

Исход матча:

```csharp
public enum TicTacToeOutcome
{
    None = 0,
    PlayerWin = 1,
    BotWin = 2,
    Draw = 3,
    Cancelled = 4
}
```

Награда:

```csharp
public sealed class TicTacToeRewardData
{
    public int BaseAmount { get; }
    public int BonusAmount { get; }
    public int TotalAmount { get; }
    public string CurrencyId { get; }
    public TicTacToeRewardDropData Drop { get; }
    public bool HasCurrency { get; }
    public bool HasDrop { get; }
    public bool HasAnyReward { get; }
}
```

Пример обработки результата:

```csharp
var result = await _ticTacToeLauncher.LaunchAsync(options, cancellationToken);

if (result.Outcome == TicTacToeOutcome.PlayerWin && result.Reward.HasCurrency)
{
    wallet.Add(result.Reward.CurrencyId, result.Reward.TotalAmount);
}

if (result.Reward.HasDrop)
{
    inventory.Add(result.Reward.Drop.DropId);
}
```

## 7. Как использованы Zenject, UniRx/UniTask и Addressables

### Zenject

Zenject используется как основной DI-контейнер.

Основные installers:

- `TicTacToeApiInstaller`
- `TicTacToeApplicationInstaller`
- `TicTacToeDomainInstaller`
- `TicTacToeInfrastructureInstaller`
- `TicTacToePresentationInstaller`
- `TicTacToeModuleBindings`
- `TicTacToeModuleInstaller`
- `TicTacToeDemoInstaller`

Что регистрируется:

- публичный API модуля: `ITicTacToeModule`, `ITicTacToeLauncher`;
- сессионный controller;
- доменные сервисы: правила, AI, расчет награды;
- UI factory и presenter factory;
- конфиг `TicTacToeModuleSettings`.

### UniTask

UniTask используется для:

- асинхронного запуска миниигры;
- ожидания завершения матча;
- загрузки `Addressables`;
- preload/release flow;
- обработки отмены через `CancellationToken`;
- задержки хода AI и автозакрытия результата.

### UniRx

UniRx в текущем решении не используется.  
Для данной задачи `UniTask` и событийной модели оказалось достаточно, поэтому дополнительная реактивная обвязка не добавлялась без необходимости.

### Addressables

Addressables используются для загрузки основного UI prefab миниигры.

Текущий flow:

- `TicTacToeModuleSettings` хранит `AssetReferenceGameObject`;
- `TicTacToeScreenFactory` делает `PreloadAsync`, `CreateAsync` и `ReleasePreloaded`;
- основной экран миниигры создается из addressable prefab;
- если prefab еще не сконфигурирован, модуль умеет создать runtime fallback UI.

Addressable asset:

- address: `GameModules/TicTacToe/Screen`

## 8. Краткое описание demo-сценария

Demo находится в `Assets/Scenes/SampleScene.unity`.

Сценарий:

- слева отображается host UI с кнопками `Launch Mini-Game` / `Play Again` и последним полученным результатом;
- справа находится контейнер `Module Host`;
- при запуске внешний caller вызывает модуль через `ITicTacToeLauncher`;
- миниигра встраивается в `Module Host`, а не открывается как отдельная сцена;
- после завершения результат и награда возвращаются наружу и отображаются в host UI.

Ключевые demo-классы:

- `TicTacToeDemoInstaller`
- `TicTacToeDemoEntryPoint`
- `TicTacToeDemoCaller`
- `TicTacToeDemoView`

Назначение demo: показать именно встраиваемость модуля и контракт возврата результата.

## 9. Ограничения и возможные улучшения

Текущие ограничения:

- AI намеренно простой и демонстрационный;
- одновременно поддерживается один активный запуск модуля;
- demo UI собирается runtime-builder’ом и ориентирован на показ интеграции, а не на production-polish;
- автоматические EditMode/PlayMode тесты в модуле пока не добавлены;
- локализация не реализована;
- наградная система демонстрационная и не интегрирована с реальной экономикой проекта.

Что можно улучшить:

- добавить EditMode тесты для правил, AI и reward-calculation;
- добавить PlayMode тесты на запуск/закрытие/перезапуск модуля;
- расширить AI до нескольких уровней сложности;
- добавить анимации, звуки и polish UI;
- выделить отдельные prefab-варианты для embedded и standalone режимов;
- добавить локализацию текстов;
- вынести reward-drop таблицы в более удобные production-конфиги;
- добавить телеметрию сессий и outcome tracking.

## Дополнительные материалы

- UML-диаграмма: [Docs/TicTacToeModule.UML.md](Docs/TicTacToeModule.UML.md)
- Описание использования AI в разработке: [Docs/AI-Usage.md](Docs/AI-Usage.md)

## Видео геймплея

Место под видео с примером работы модуля:

![Gameplay Demo](Assets/Docs/tictactoe-demo.gif)

Если файл еще не добавлен, ссылка начнет работать после помещения видео по указанному пути.

## Итог

Проект реализует миниигру "Крестики-нолики" как переиспользуемый embedded module для Unity:

- с чистым внешним API;
- с интеграцией через Zenject;
- с асинхронным lifecycle через UniTask;
- с загрузкой UI через Addressables;
- с возвратом результата и награды наружу;
- с demo-сценарием, который показывает реальную встраиваемость модуля в host UI.
