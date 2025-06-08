

https://oauth.yandex.ru/client/new
https://oauth.yandex.ru/client/cfb473432b8741839f5e3d6d2bfd50b1

https://o2.mail.ru/app/
https://o2.mail.ru/app/edit/41f8c74b720b4dd18d13828e53d4dd41/

в яндекс авторизацию иогда в метод Callback попадаем 2 раза
и если 2 ой раз вызвать метод await httpClient.PostAsync("https://oauth.yandex.ru/token", requestContent); то вызывается исключении из за ошибки badrequest

Нет race condition – все параллельные запросы синхронизируются через TaskCompletionSource.



Todo
добавить авторизацию через телеграмм
госуслуги
tinkof Id
сбер id
alfa id

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

google auth

[HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            TokenResponse tokenResponse = await _authService.HandleCallback(code);

            // Устанавливаем куки с токенами
            /*Response.Cookies.Append("access_token", tokenResponse.AccessToken, new CookieOptions
            {
                HttpOnly = true, // Защита от XSS
                Secure = true,    // Только HTTPS
                SameSite = SameSiteMode.Strict, // Защита от CSRF
                Expires = DateTime.UtcNow.AddHours(1) // Время жизни access-токена
            });

            Response.Cookies.Append("refresh_token", tokenResponse.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7) // Время жизни refresh-токена
            });

            // Редирект на фронтенд
            return Redirect("https://virtual-fit.one");*/

            //Перенаправляем пользователя на фронтенд
            return Redirect($"https://virtual-fit.one?" +
                            $"accessToken={Uri.EscapeDataString(tokenResponse.AccessToken)}&" +
                            $"refreshToken={Uri.EscapeDataString(tokenResponse.RefreshToken)}");

            //return Redirect($"https://localhost:7105/connect/?accessToken={tokenResponse.AccessToken}&refreshToken={tokenResponse.RefreshToken}");            
        }


/////////////////////////////////

vk

/*_logger.LogInformation($"callback code: {code} {Environment.NewLine}" +
                                    $"expires_in: {expires_in}  {Environment.NewLine}" +
                                    $"device_id: {device_id}  {Environment.NewLine}" +
                                    $"state: {state}  {Environment.NewLine}" +
                                    $"ext_id: {ext_id}  {Environment.NewLine}" +
                                    $"type: {type}  {Environment.NewLine}");*/