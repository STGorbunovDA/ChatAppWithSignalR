namespace ChatAppWithSignalR.Api.Functions.Message
{
    // Этот класс "MessageFunction" представляет собой реализацию интерфейса
    // "IMessageFunction" и содержит логику связанную с обработкой и управлением
    // сообщениями в приложении ChatApp с использованием SignalR.
    public class MessageFunction : IMessageFunction
    {
        ChatAppContext _chatAppContext;
        IUserFunction _userFunction;


        /*
            * В конструкторе класса "MessageFunction" принимаются два параметра:
              "chatAppContext" типа "ChatAppContext" и "userFunction" типа "IUserFunction".
              "ChatAppContext" представляет контекст базы данных приложения, а "IUserFunction"
              представляет другой интерфейс или класс, связанный с управлением пользователями.
        */
        public MessageFunction(ChatAppContext chatAppContext, IUserFunction userFunction)
        {
            _chatAppContext = chatAppContext;
            _userFunction = userFunction;
        }


        /*
            * Метод "AddMessage" используется для добавления нового сообщения в базу данных.
              Он принимает идентификатор отправителя сообщения (fromUserId), идентификатор
              получателя сообщения (toUserId) и текст сообщения (message).
              Внутри метода создается новый объект "TblMessage", заполняются
              его свойства данными из параметров, а затем объект добавляется
              в контекст базы данных и сохраняется с помощью метода "SaveChangesAsync".
              Метод возвращает результат сохранения.
        */
        public async Task<int> AddMessage(int fromUserId, int toUserId, string message)
        {
            var entity = new TblMessage
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                Content = message,
                SendDateTime = DateTime.Now,
                IsRead = false
            };

            _chatAppContext.TblMessages.Add(entity);
            var result = await _chatAppContext.SaveChangesAsync();

            return result;
        }

        /*
            * Метод "GetLastestMessage" используется для получения последних сообщений
              между пользователем и его друзьями. Он принимает идентификатор пользователя
              (userId). Внутри метода сначала получается список друзей пользователя
              из базы данных. Затем для каждого друга из списка выполняется запрос
              к базе данных для получения последнего сообщения между пользователем
              и другом. Если последнее сообщение существует, создается объект
              "LastestMessage" и добавляется в список "result".
              В конечном итоге метод возвращает список "result".
        */
        public async Task<IEnumerable<LastestMessage>> GetLastestMessage(int userId)
        {
            var result = new List<LastestMessage>();

            var userFriends = await _chatAppContext.TblUserFriends
                .Where(x => x.UserId == userId).ToListAsync();

            foreach (var userFriend in userFriends)
            {
                var lastMessage = await _chatAppContext.TblMessages
                    .Where(x => (x.FromUserId == userId && x.ToUserId == userFriend.FriendId)
                             || (x.FromUserId == userFriend.FriendId && x.ToUserId == userId))
                    .OrderByDescending(x => x.SendDateTime)
                    .FirstOrDefaultAsync();

                if (lastMessage != null)
                {
                    result.Add(
                        new LastestMessage
                        {
                            UserId = userId,
                            Content = lastMessage.Content,
                            UserFriendInfo = _userFunction.GetUserById(userFriend.FriendId),
                            Id = lastMessage.Id,
                            IsRead = lastMessage.IsRead,
                            SendDateTime = lastMessage.SendDateTime
                        });
                }
            }
            return result;
        }


        /*
            * Метод "GetMessages" используется для получения всех сообщений между двумя
              пользователями. Он принимает идентификатор отправителя сообщений (fromUserId)
              и идентификатор получателя сообщений (toUserId). Внутри метода выполняется
              запрос к базе данных для получения всех сообщений, которые относятся к указанным
              пользователям. Результат запроса сортируется по времени отправки и преобразуется
              в список объектов "Message", каждый из которых представляет одно сообщение.
        */
        public async Task<IEnumerable<Message>> GetMessages(int fromUserId, int toUserId)
        {
            var entities = await _chatAppContext.TblMessages
                .Where(x => (x.FromUserId == fromUserId && x.ToUserId == toUserId)
                    || (x.FromUserId == toUserId && x.ToUserId == fromUserId))
                .OrderBy(x => x.SendDateTime)
                .ToListAsync();

            return entities.Select(x =>
                new Message
                {
                    Id = x.Id,
                    Content = x.Content,
                    FromUserId = x.FromUserId,
                    ToUserId = x.ToUserId,
                    SendDateTime = x.SendDateTime,
                    IsRead = x.IsRead,
                });
        }
    }
}
