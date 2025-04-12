https://v2.d-f.pw/app/application/30944
https://a30944-8332.x.d-f.pw/swagger/index.html
https://huggingface.co/spaces/yisol/IDM-VTON     free price 

https://replicate.com/account/billing
текущего баланса хватит до 10.5$

https://www.ifoto.ai/change-clothes-ai

ResizeImageFromStream в этом методе всегда приводим к jpeg

для каждого сервиса делать свой проект в котором будут сервисы модельки и контроллеры и потом подклчать его к главному проекту Meta.Api
 
1. вручную создал папку wwwroot по пути C:\DVV\GitHub\meta-api2\meta-api\MetaPlatform\MetaApi
чтобы переменная _env.WebRootPath не была null

1. правой кнопкой мыши по проекту и  Publish и создаем архив папки publish 

Todo: задеплоить приложение в deployf

добавить ratelimit

Сделать вход по промокоду и админка по созданию промокодов и чтоб посмотреть все фотки как для пользователя так и для админа
Проверку промокода сделать выше чем в контроллере

промокод нужно хранить на клиенте

deployf

https://a30760-e5fe.u.d-f.pw/swagger/index.html

добавил порт 8080 для https через их интерфейс

развернутое приложение теперь не удаляем а обновляем через файловый менеджер
загружаем туда архив но в этом архиве не должно быть папки wwwroor так как она уже есть там и не пустая

тут развернута клиентская часть и почему то она работает только в режиме  инкогнито


/////////
моделька для восстановления фото
https://replicate.com/microsoft/bringing-old-photos-back-to-life?input=json
microsoft/bringing-old-photos-back-to-life
улучшает разреешение и качество

https://replicate.com/lucataco/remove-bg

//rename project to Meta.Api
Meta.Api.Data.SqlServer

/////////////////////////

Замена текста в аудиофайлах (с вокалом)
a. Разделение вокала и инструментала

Перед заменой слов нужно выделить вокал из аудиофайла. Используются инструменты:

    Spleeter: инструмент для разделения вокала и фоновой музыки.

    spleeter separate -i input_audio.mp3 -o output/

b. Генерация речи для замены

Используйте TTS (Text-to-Speech) модели, чтобы сгенерировать новую фразу для замены:

    Google TTS
    Microsoft Azure Speech Service
    Tacotron 2 или FastSpeech 2

Сгенерированный текст можно вставить обратно в трек с помощью звукового редактора.
c. Fine-tuned нейросети для замены текста в аудио

    So-Vits-SVC: позволяет менять текст в вокале, сохраняя голос певца.
    RVC (Retrieval-based Voice Conversion): изменяет вокал или текст на основе выбранной модели.
 
 

 //////////////////////////

 возможно сделать таблицу с картинками
 id imgUrl crcImg


 все промокоды должны быть загружены в память чтоб можно бэло быстро проверять есть ли такой промокод (ConcurrentDictionary)

 хранить промокоды в бд в закрытом виде

 ////////////////////////////

 нажали try on 
 c 3 какими url
 первые 2 такие же как при отправке запроса try_on тоесть могут быть с падингами и нужно будет сделать replace('_p', '_v') в методе showResult
 результат
 при try_on создается 2 файла 
 https://a30944-8332.x.d-f.pw/result/guid_v.png
 https://a30944-8332.x.d-f.pw/result/guid_t.png

 Url с историей всегда возвращается на конце с _t
 Большое изображение возвращается на конце с _v
 И реультирующее также

 при upload должна возвращаться полноразмерная фото с падингом для отправки в модель ИИ
 если фото с андроид телефона в формате
 https://a30944-8332.x.d-f.pw/uploads/guid_1,234_p.png               в кеш это фото будет добавляться? и при начальной загрузке это фото будет загружаться?
 также создается уменьшенная копия для отображения в истории
 https://a30944-8332.x.d-f.pw/uploads/guid_1,234_t.png
 и также полноразмерная фото для просмотра
 https://a30944-8332.x.d-f.pw/uploads/guid_1,234_v.png

 _p - padding
 _t - thumbnails
 _v - view

 если с айфона то без падинга
 для отправки в модель ии и для просмотра
 https://a30944-8332.x.d-f.pw/uploads/guid_v.png
 также создается уменьшенная комия для отображения в истории
 https://a30944-8332.x.d-f.pw/uploads/guid_t.png

 ///////////////////////////

 Общая инфа о минрации:

Package Manager Console
DropDown -> Data Project
Startup project -> API Project

Add-Migration InitialCreate
Update-Database

Remove-Migration first_migration

Тонкости миграции:

Update-Database не обязательно выполнять можно просто запустить приложение
app.UseSwaggerUI();

// Применение миграций автоматически при старте приложения
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MetaDbContext>();
    context.Database.Migrate();
}

если в методе OnModelCreating вызывается метод Seed c добавлением данных
команда Add-Migration InitialCreate выполнится нормально
команда Update-Database выполнитмя с ошибкой
открываем автосгенеренную миграцию и копируем оттуда код по добавлению сущностей
далее отменяем (удаляем миграции) коменнтим метод Seed и заново выполняем команду
Add-Migration InitialCreate
потом вручную добавляем в код миграции то что скопировали и далее Update-Database
миграция должна успешно примениться