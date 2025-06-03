

https://oauth.yandex.ru/client/new
https://oauth.yandex.ru/client/cfb473432b8741839f5e3d6d2bfd50b1

в яндекс авторизацию иогда в метод Callback попадаем 2 раза
и если 2 ой раз вызвать метод await httpClient.PostAsync("https://oauth.yandex.ru/token", requestContent); то вызывается исключении из за ошибки badrequest

Нет race condition – все параллельные запросы синхронизируются через TaskCompletionSource.



Todo
добавить авторизацию через телеграмм
госуслуги
tinkof Id
сбер id
alfa id