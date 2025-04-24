

Перенести сервисы из апи в core


заюзать  cancellationToken в Task StartAsync(CancellationToken cancellationToken)

выделить в отдельный ммикросервис функционал по сохранению и получению ссылки на файл

выделить в отдельный ммикросервис функционал с авторизацией

и микросервис с основным функционалом примерка + ограничение на кол-во примерок

FileCrcHostedService нужно как-то отделить от других серввисов. Возможно другие сервисы переедут
в Core а этот тут останется

на вход и выход в репозиторий должны поступать доменные объекты
и мапинг внутри репозитория

на вход и выход в сервис должны поступать дто объекты

на вход контроллера должны поступать реквесты а на выходе респонс или дто

в контроллере не нужно выполнять мапинг
дто в доменную модель и наоборот происходит вннутри сервиса

доменные объекты могут содержать логику
Доменные модели часто содержат методы и правила, которые должны быть выполнены при создании или изменении объекта. Эти правила лучше всего соблюдать внутри сервиса, а не в контроллере.

доменные модели луччше так описывать
public class User
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    public User(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.");
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.");

        Name = name;
        Email = email;
    }

    public void ChangeEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
            throw new ArgumentException("Email cannot be empty.");

        Email = newEmail;
    }
}

а дто так
public class UserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class UserFactory
{
    public static User CreateFromDto(UserDto userDto)
    {
        return new User(userDto.Name, userDto.Email);
    }
}
var user = UserFactory.CreateFromDto(userDto);

на верхнем уровне сделать 2 папки src, tests

добавить интерфейс дляя добавления файлов    чтоб можно было подменить на амазон s3

в Core не должно быть Entity сущностей
только доменные объекты
внутри метода TryOnClothesAsync создается FittingResultEntity
вместо этого создать доменный объект FittingResult и смапить его в FittingResultEntity

отрефачить метод в ReplicateClientService ProcessPredictionAsync
название метода другое

подумать куда добавить SystemTime и ISystemTime

вынести в конфиг
AccountId = userId,
                    MaxAttempts = 3, // Дефолтное значение (можно вынести в конфиг)
                    AttemptsUsed = 0,
                    LastResetTime = _systemTime.UtcNow,
                    ResetPeriod = TimeSpan.FromDays(1) // Дефолтный период (1 день)

добавить интерфейс который возвращает текущее время в TryOnLimitService

транзакцию добавить в TryOnClothesAsync и нужно ли в транзакцию добавлять операцию добавления в кеш? возможмно нет. просто если в кеш не удалось положить возможнмо его нужно инвалидировать.

добавить вермя жизни к объектам в кеше

вынессти код по авторизации в отдельный проект.

Посмотреть код с кешами все ли верно 
хорошобы тесты написать которые проверяют что лимиты изменяются ккорректно

todo rename FittingResults to FittingHistory

нужно добавить транзакцию на try on
и скорее всего метод по уменьшению кол-ва числа попыток убрать из мидлваре 
и эта надпись уйдет return limit.MaxAttempts - limit.AttemptsUsed - 1;
если не получилось добавить в бд то в др хранилище отправляем типа на диск и будет воркер который будет см это хранилище

добавить тест на rate limit

healthcheck на доступность бд и если что-то не так то отправлять уведомление в телеграмм

сделать чтоб была статистика кто сколько раз нажал примерить

добавить тест что после каждого изменения будет работать авторизация
метод try on история

кеширующий слой добавить

кеши добавить

возможно в эндпоинты добавить CancelationToken в try on и в загрузку изображений и проверить что если закрываем страницу то cancelation сработает

после долгого неиспользования сервиса когда нажимаю photo from the collection почему-то не может обновить токен и сбрасывает его

LastResetTime maybe rename to LastResetUtcDateTime

Seq можно развернуть и сделать структурные логи

ситуация изображение посчиталось но бд недоступна. и кол-во использованных попыток не обновится. и результат не запишется.
а нужно чтобы он хотяб чуть позже но записался.
как решение это в памяти хранить и выгружать на диск состояние или в редис.

написать тест чтоб отрабатывала бд
чтоб можно было протестить без бд и с ней

Добавить ограничение на размер загружаемого файла 10 мб.

класс Result уже реализован в асп

добавить  в таблицу акаунт поле email

добавить глобальный обработчик исключений

добавление оплаты

FluentMigrator

кеш:
Ограничить размер MemoryCache
Реализовать LRU политику вытеснения

как сделать чтоб бэr разворачивался по моему домену virtual-fit.one

в гугл и вконтакте добавить ключ для дебага

см плюсы медиатра и переписать на медиатр и vitrticalSlice architecture

vertical slice https://www.youtube.com/live/dnvi0B76ekg

 хедер Retry-After, чтобы указать клиенту, когда он может повторить запрос.
 app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (RateLimitException)
    {
        context.Response.Headers.Append("Retry-After", "10");
        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.Response.WriteAsync("Too many requests. Please try again later.");
    }
});

https://www.youtube.com/live/vAN_EogbO6s

добавить авторизацию череез яндекс