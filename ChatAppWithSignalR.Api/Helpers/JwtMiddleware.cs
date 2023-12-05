using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ChatAppWithSignalR.Api.Helpers
{
    /*
        * Предназначен для обработки JWT-токена в промежутке между приходом
          HTTP-запроса и его обработкой в следующем обработчике запроса.

            1. Получение JWT-токена из заголовка запроса: Middleware проверяет заголовок запроса 
               "Authorization" для наличия JWT-токена. Если он отсутствует, Middleware также проверяет 
               заголовок "ChatHubBearer" для наличия токена.
            2. Присоединение информации о пользователе к контексту запроса: 
               Если JWT-токен успешно получен, Middleware вызывает метод "AttachUserToContext" 
               для проверки и валидации токена. Если токен действителен, Middleware извлекает 
               идентификатор пользователя из токена и использует объект "userFunction", реализующий 
               интерфейс "IUserFunction", для получения информации о пользователе по его идентификатору. 
               Затем Middleware добавляет объект пользователя в свойство "Items" контекста запроса 
               под ключом "User".
            3. Продолжение обработки запроса: В конце Middleware вызывает следующий обработчик запроса 
               в цепочке обработчиков, используя делегат "next".
        
        
        * С помощью JwtMiddleware можно реализовать процесс аутентификации 
          и авторизации на основе JWT-токенов в приложении. Он позволяет извлекать 
          и верифицировать токены, а также связывать информацию о пользователе 
          с контекстом запроса, чтобы она была доступна в следующих обработчиках запроса.
    */

    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserFunction userFunction)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token == null)
                token = context.Request.Headers["ChatHubBearer"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                AttachUserToContext(context, userFunction, token);

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, IUserFunction userFunction, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("1234567890123456");
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                context.Items["User"] = userFunction.GetUserById(userId);
            }
            catch
            {

            }
        }
    }
}
