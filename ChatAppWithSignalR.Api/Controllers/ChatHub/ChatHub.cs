namespace ChatAppWithSignalR.Api.Controllers.ChatHub
{
    // класс "ChatHub" представляет собой реализацию SignalR хаба (Hub)
    // для обработки взаимодействия между клиентами и сервером в приложении ChatApp.
    public class ChatHub : Hub
    {
        UserOperator _userOperator;
        IMessageFunction _messageFunction;
        private static readonly Dictionary<int, string> _connectionMapping
            = new Dictionary<int, string>();


        /*
            * В конструкторе класса "ChatHub" принимаются параметры "userOperator"
              типа "UserOperator" и "messageFunction" типа "IMessageFunction".
              Параметр "userOperator" используется для выполнения операций
              с пользователями, а параметр "messageFunction" используется
              для выполнения операций с сообщениями.
        */
        public ChatHub(UserOperator userOperator, IMessageFunction messageFunction)
        {
            _userOperator = userOperator;
            _messageFunction = messageFunction;
        }

        /*
            * Метод "SendMessage" вызывается клиентом для отправки сообщения на сервер.
              Он принимает строку "message" в качестве аргумента. Внутри метода вызывается
              метод "SendAsync" с именем "ReceiveMessage" и передается полученное сообщение.
              Этот метод отправляет сообщение всем подключенным клиентам через асинхронное
              соединение "Clients.All".
        */
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        /*
            * Метод "SendMessageToUser" вызывается клиентом для отправки сообщения конкретному 
              пользователю. Он принимает идентификатор отправителя (fromUserId), 
              идентификатор получателя (toUserId) и сообщение (message).
              Внутри метода происходит следующее:
                
                1. Получаются все идентификаторы соединений (connectionIds) для указанного 
                   идентификатора получателя (toUserId) из _connectionMapping (словарь, 
                   содержащий соответствие между идентификаторами пользователей 
                   и идентификаторами подключений).

                2. Вызывается метод "AddMessage" из "_messageFunction" для добавления 
                   сообщения в базу данных или выполнения других необходимых операций 
                   с сообщениями.

                3. С помощью метода "Clients.Clients(connectionIds)" указывается, 
                   кому нужно отправить сообщение, а методу "SendAsync" с именем "ReceiveMessage" 
                   передаются идентификатор отправителя (fromUserId) и сообщение (message).
        */
        public async Task SendMessageToUser(int fromUserId, int toUserId, string message)
        {
            var connectionIds = _connectionMapping.Where(x => x.Key == toUserId)
                                                    .Select(x => x.Value).ToList();

            await _messageFunction.AddMessage(fromUserId, toUserId, message);

            await Clients.Clients(connectionIds)
                .SendAsync("ReceiveMessage", fromUserId, message);
        }

        /*
            * Метод "OnConnectedAsync" вызывается при подключении клиента к хабу. 
              Внутри метода происходит следующее:

                1. Получается идентификатор пользователя (userId) из "userOperator" 
                   на основе текущего запроса.
                2. Если идентификатор пользователя отсутствует в _connectionMapping, 
                   добавляется новая запись в словарь _connectionMapping, где ключом 
                   является идентификатор пользователя, а значением - идентификатор 
                   соединения (Context.ConnectionId).
        */
        public override Task OnConnectedAsync()
        {
            var userId = _userOperator.GetRequestUser().Id;
            if (!_connectionMapping.ContainsKey(userId))
                _connectionMapping.Add(userId, Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        /*
            * Метод "OnDisconnectedAsync" вызывается при отключении клиента от хаба. 
              Внутри метода происходит следующее:

                1. Получается идентификатор пользователя из "userOperator" 
                   на основе текущего запроса.
                2. Если идентификатор пользователя присутствует в _connectionMapping, 
                   выполняется удаление записи с этим идентификатором из _connectionMapping.
        */
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _connectionMapping.Remove(_userOperator.GetRequestUser().Id);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
