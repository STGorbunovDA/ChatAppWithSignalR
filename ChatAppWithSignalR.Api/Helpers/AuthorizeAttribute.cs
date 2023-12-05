namespace ChatAppWithSignalR.Api.Helpers
{
    /*
        * Этот класс "AuthorizeAttribute" предназначен для реализации фильтра авторизации в ASP.NET Core.

        * Фильтры авторизации являются частью пайплайна обработки запросов в ASP.NET Core 
          и позволяют проверять авторизацию пользователя перед выполнением определенных действий 
          или обработкой запроса.

        * В данном случае, класс "AuthorizeAttribute" является пользовательским атрибутом, 
          который может быть применен к контроллерам или их методам для проверки авторизации 
          пользователя перед выполнением действия.

        * Метод "OnAuthorization" реализует интерфейс "IAuthorizationFilter" и вызывается 
          перед выполнением действия контроллера. В методе производится проверка наличия 
          объекта пользователя в свойстве "Items" контекста запроса. Если объект пользователя 
          отсутствует (то есть пользователь не авторизован), то устанавливается результат 
          выполнения запроса в виде JSON-ответа с кодом состояния 401 (Unauthorized), 
          указывающего на отсутствие авторизации.

        * Таким образом, используя "AuthorizeAttribute" на контроллерах или методах контроллеров, 
          можно осуществлять авторизацию пользователей и ограничивать доступ к определенным действиям 
          или ресурсам в приложении.
    */
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext?.Items["User"] as User;
            if (user == null)
            {
                context.Result = new JsonResult(new { StatusMessage = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
