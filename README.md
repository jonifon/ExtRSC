# ExtRSC - Инструмент удаленного выполнения команд 

ExtRSC - это проект на C#, который реализует выполнение команд на удаленном устройстве. Он состоит из двух частей: клиентского приложения и серверного приложения. Клиентское приложение устанавливается на удаленных компьютерах и позволяет выполнять команды, отправляемые с сервера. Серверное приложение управляет подключенными клиентами и отправляет команды на выбранные клиентские машины.

## Особенности

- Безопасное шифрование команд и ответов с использованием алгоритма AES-256.
- Поддержка множества одновременно подключенных клиентов.
- Интерактивная консоль на стороне сервера для управления клиентами.
- Возможность выбора конкретного клиента для отправки команд.

## Требования

- .NET 8.0 или выше.
- Windows-совместимая операционная система.

## Установка и запуск

1. Клонируйте репозиторий:
   ```
   git clone https://github.com/jonifon/ExtRSC
   ```

2. Откройте проект в Visual Studio.

3. Соберите проект, чтобы восстановить зависимости и скомпилировать приложения.

4. Настройте ключ шифрования и вектор инициализации (IV) в файлах `Server/Server.cs` и `Client/Service1.cs`. Убедитесь, что они одинаковы на клиенте и сервере.

5. Запустите серверное приложение.

6. Установите и запустите клиентское приложение на удаленных компьютерах.

## Использование

1. После запуска серверного приложения вы увидите интерактивную консоль.

2. Используйте следующие команды для управления клиентами:
   - `help` - Отображает справку по доступным командам.
   - `clients` - Выводит список подключенных клиентов.
   - `use <IP>` - Выбирает клиента с указанным IP-адресом для отправки команд.
   - `cmd` - Отправляет команду выбранному клиенту.
   - `exit` - Завершает работу серверного приложения.

3. Для отправки команды конкретному клиенту, сначала выберите клиента с помощью команды `use <IP>`, а затем используйте команду `cmd` для ввода и отправки команды.

4. Ответы от клиентов будут отображаться в консоли сервера.

## Внесение изменений и улучшений

Если вы хотите внести изменения или улучшения в проект, пожалуйста, следуйте этим шагам:

1. Создайте форк репозитория.

2. Внесите необходимые изменения в коде.

3. Протестируйте изменения, чтобы убедиться в их корректной работе.

4. Отправьте pull request с подробным описанием внесенных изменений.

## Лицензия

Этот проект распространяется под лицензией [MIT License](LICENSE).

## Контакты

Если у вас есть вопросы, предложения или проблемы, связанные с проектом, пожалуйста, свяжитесь с нами по адресу jf-github-communication@bastardi.net.
