namespace ChatAppWithSignalR.Api.Helpers
{
    /*
        * Класс используется для получения информации о текущем пользователе,
          связанной с текущим HTTP-запросом. HttpContextAccessor предоставляет
          доступ к объекту HttpContext, который содержит сведения о текущем
          запросе и ответе.
    */
    public class UserOperator
    {
        IHttpContextAccessor _httpContext;

        public UserOperator(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public User? GetRequestUser()
        {
            if (_httpContext == null)
                return null;

            return _httpContext.HttpContext?.Items["User"] as User;
        }
    }
}
