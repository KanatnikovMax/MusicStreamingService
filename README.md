# Музыкальный стриминговый сервис

## Описание
Сервис представляет из&nbsp;себя веб-приложение для прослушивания музыки по&nbsp;сети
с&nbsp;возможностью сохранения любимых песен и&nbsp;альбомов в&nbsp;своём аккаунте.
___
## Используемые технологии
Для реализации использовались следующие технологии:
- Microsoft.NET Core
- Entity Framework Core
- PostgreSQL
- Apache Cassandra
- Duende Identity Server
- React
- Docker
- Redis
___
## Авторизация
В&nbsp;зависимости от&nbsp;своей роли пользователь может:
- Не&nbsp;авторизован 
  - Смотреть список всех песен/альбомов/исполнителей
  - Производить поиск песен/альбомов по&nbsp;названию и&nbsp;исполнителей по&nbsp;имени
  - Просматривать страницы конкретных альбомов и&nbsp;исполнителей
  - Прослушивать аудиозаписи
- Авторизован с&nbsp;ролью _&quot;пользователь&quot;_
  - Весь функционал не&nbsp;авторизованного пользователя
  - Добавлять песни и&nbsp;альбомы в&nbsp;свой аккаунт, а&nbsp;также удалять их&nbsp;из&nbsp;своего аккаунта
  - Смотреть список добавленных песен/альбомов
  - Производить поиск среди добавленных песен/альбомов
  - Изменять пользовательские данные (пароль, адрес электронной почты, имя пользователя)
- Авторизован с&nbsp;ролью _&quot;администратор&quot;_
  - Просматривать список существующих песен/альбомов/исполнителей
  - Добавлять новых исполнителей и&nbsp;новые альбомы, загружать новые песни
  - Изменять данные о&nbsp;песнях/альбомах/исполнителях
  - Удалять песни/альбомы/исполнителей
___
## Базы данных
В&nbsp;приложении используются 2&nbsp;базы данных:
- **PostgreSQL** для хранения данных о&nbsp;пользователях, 
исполнителях и&nbsp;альбомах, метаданных о&nbsp;песнях
- **Apache Cassandra** для хранения аудиофайлов
___
## Схема базы данных
<img width="797" height="341" alt="image" src="https://github.com/user-attachments/assets/dd5155ca-513b-4373-a689-b1fd9fb321e8" />

## Скриншоты интерфейса
Вход в аккаунт

<img width="599" height="571" alt="image" src="https://github.com/user-attachments/assets/6c7c1cb8-5cf1-4f35-a28c-5ba6055c9a18" />

Главная страница с проигрывателем

<img width="624" height="328" alt="image" src="https://github.com/user-attachments/assets/581dd4f7-81f7-4911-89da-f99d3f897b95" />

Страница исполнителей

<img width="624" height="319" alt="image" src="https://github.com/user-attachments/assets/2f06e0bb-5d50-4992-8655-8fd095d0be75" />

Страница альбомов

<img width="624" height="318" alt="image" src="https://github.com/user-attachments/assets/5369f611-c02b-46c6-be0e-79b9d1723cf0" />

Открытый альбом

<img width="622" height="342" alt="image" src="https://github.com/user-attachments/assets/ab81445b-d4b2-427a-8e28-c8d8b3e2d704" />

Открытый исполнитель

<img width="620" height="331" alt="image" src="https://github.com/user-attachments/assets/9fcbaf85-a4cc-41cc-ba7a-05344ab809b7" />

Страница артистов в панели администратора

<img width="624" height="300" alt="image" src="https://github.com/user-attachments/assets/ee03486c-825a-4689-a5a6-ce501e35dfeb" />

Загрузка песни администратором

<img width="624" height="327" alt="image" src="https://github.com/user-attachments/assets/5963e458-edaa-41d2-8c0f-c79bba384b3a" />
