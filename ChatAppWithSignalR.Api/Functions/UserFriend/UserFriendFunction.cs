namespace ChatAppWithSignalR.Api.Functions.UserFriend
{
    // В этом коде представлен класс "UserFriendFunction", который является
    // реализацией интерфейса "IUserFriendFunction". Он содержит логику для работы
    // с друзьями пользователя в приложении ChatApp.
    public class UserFriendFunction : IUserFriendFunction
    {
        ChatAppContext _chatAppContext;
        IUserFunction _userFunction;

        /*
            * В конструкторе класса "UserFriendFunction" принимаются два параметра: 
              "chatAppContext" типа "ChatAppContext" и "userFunction" типа "IUserFunction". 
              "chatAppContext" представляет контекст базы данных приложения, а "userFunction" 
              представляет функциональность, связанную с операциями пользователей.
        */
        public UserFriendFunction(ChatAppContext chatAppContext, IUserFunction userFunction)
        {
            _chatAppContext = chatAppContext;
            _userFunction = userFunction;
        }
        /*
            * Метод "GetListUserFriend" используется для получения списка друзей пользователя. 
              Он принимает идентификатор пользователя (userId) и возвращает коллекцию 
              пользователей (IEnumerable<User.User>) в виде асинхронной задачи (Task).

            * Внутри метода происходит следующее:
            
                1. Выполняется запрос к базе данных, используя контекст "chatAppContext". 
                   Метод "Where" фильтрует записи таблицы "TblUserFriends" по полю "UserId", 
                   чтобы найти записи, связанные с указанным идентификатором пользователя.

                2. Результат запроса сохраняется в переменную "entities" в виде списка.
                
                3. Создается переменная "result" для хранения результата - списка пользователей.

                4. Для каждой сущности из списка "entities" вызывается метод "GetUserById" 
                   из "userFunction" с идентификатором друга (FriendId). Этот метод предположительно 
                   возвращает пользователя (User.User) на основе его идентификатора.

                5. Результаты помещаются в переменную "result" с использованием метода "Select".

                6. Если результат равен "null", создается пустой список пользователей.

            * Таким образом, класс "UserFriendFunction" предоставляет логику для получения 
              списка друзей пользователя на основе его идентификатора. Запрос данных выполняется 
              через контекст базы данных, а для каждого друга пользователя вызывается функция 
              "GetUserById" для получения соответствующего пользователя. Возвращается 
              список пользователей или пустой список, если результат равен "null".
        */
        public async Task<IEnumerable<User.User>> GetListUserFriend(int userId)
        {
            var entities = await _chatAppContext.TblUserFriends
                .Where(x => x.UserId == userId)
                .ToListAsync();

            var result = entities.Select(x => _userFunction.GetUserById(x.FriendId));

            if (result == null) result = new List<User.User>();

            return result;
        }
    }
}
