# ECommerce - Интернет-магазин на ASP.NET Core + React

Модульный монолит на ASP.NET Core с React-фронтендом. Регистрация и вход, каталог товаров с изображениями, корзина на Redis, заказы с полным жизненным циклом, админ-панель для управления товарами и заказами.

## Стек

**Backend:** C# 14, ASP.NET Core 10, Entity Framework Core, MediatR, JWT  
**Database:** PostgreSQL, Redis  
**Storage:** MinIO (S3-совместимое)  
**Frontend:** React   
**DevOps:** Docker Compose, Serilog  

## Архитектура

Модульный монолит из 4 модулей с Clean Architecture внутри каждого:

```
src/
├── Host/                                    # Точка входа, Program.cs
├── Modules/
│   ├── Identity/                            # Пользователи, JWT, роли
│   │   ├── Modules.Identity.Domain/
│   │   ├── Modules.Identity.Application/
│   │   ├── Modules.Identity.Infrastructure/
│   │   └── Modules.Identity.Api/
│   ├── Catalog/                             # Товары, изображения
│   │   ├── Modules.Catalog.Domain/
│   │   ├── Modules.Catalog.Application/
│   │   ├── Modules.Catalog.Infrastructure/
│   │   └── Modules.Catalog.Api/
│   ├── Basket/                              # Корзина на Redis
│   │   ├── Modules.Basket.Domain/
│   │   ├── Modules.Basket.Application/
│   │   ├── Modules.Basket.Infrastructure/
│   │   └── Modules.Basket.Api/
│   └── Ordering/                            # Заказы, статусы
│       ├── Modules.Ordering.Domain/
│       ├── Modules.Ordering.Application/
│       ├── Modules.Ordering.Infrastructure/
│       └── Modules.Ordering.Api/
├── Shared/                                  # Общие абстракции
│   └── Shared.csproj
└── frontend/                                # React
    ├── src/
    │   ├── api/                             # API-клиент
    │   ├── components/                      # UI-компоненты
    │   ├── pages/                           # Страницы
    │   ├── context/                         # AuthContext
    │   └── router/                          # Роутер

Межмодульное взаимодействие через MediatR (команды и запросы). При переходе на микросервисы MediatR заменяется на RabbitMQ/MassTransit без изменения бизнес-логики.
```

## Быстрый старт

### Предварительные требования

- .NET 10 SDK
- Node.js 18+
- Docker Desktop

### Установка

1. Клонировать репозиторий:

```bash
git clone https://github.com/daniilfars/ECommerce
cd Shop
```

2. Создать файл `.env` в корне проекта:

```env
POSTGRES_PASSWORD=your_secure_password
JWT_SECRET_KEY=your_secret_key_at_least_32_characters_long
MINIO_ROOT_USER=minioadmin
MINIO_ROOT_PASSWORD=minioadmin
```

3. Запустить бэкенд через Docker Compose:

```bash
docker compose up
```

4. Запустить фронтенд локально:

```bash
cd src/frontend
npm install
npm run dev
```

5. Открыть в браузере:

- Frontend: `http://localhost:5173` (Vite выведет актуальный порт в консоли при запуске)
- Swagger API: `http://localhost:8080/swagger`

### Учетные данные по умолчанию

При первом запуске автоматически создается администратор:

- Email: `admin@shop.com`
- Пароль: `Admin123!`

Обычного пользователя можно зарегистрировать через интерфейс.

## Модули

### Identity

Регистрация и вход через JWT-токены.

- Access-токен (15 минут) передается в заголовке `Authorization: Bearer`
- Refresh-токен (7 дней) хранится в httpOnly-куке
- Автоматическое обновление access-токена при истечении (ротация refresh-токена)
- Обнаружение повторного использования refresh-токена (защита от replay-атак)
- Роли: `User`, `Admin`

Эндпоинты:

- `POST /api/Identity/register` — регистрация
- `POST /api/Identity/login` — вход
- `POST /api/Identity/refresh` — обновление токена
- `POST /api/Identity/logout` — выход

### Catalog

Управление товарами интернет-магазина.

- CRUD с пагинацией
- Загрузка изображений в MinIO
- Публичный просмотр для всех
- Создание, редактирование, удаление только для `Admin`
- При загрузке нового изображения старое удаляется из MinIO

Эндпоинты:

- `GET /api/Catalog?page=1&pageSize=12` — список товаров
- `GET /api/Catalog/{id}` — товар по ID
- `POST /api/Catalog` — создать товар (Admin)
- `PUT /api/Catalog/{id}` — обновить товар (Admin)
- `DELETE /api/Catalog/{id}` — удалить товар (Admin)
- `POST /api/Catalog/{id}/upload-image` — загрузить изображение (Admin)

### Basket

Корзина на Redis с TTL 7 дней.

- Добавление товара (цена и название подтягиваются из Catalog автоматически)
- Изменение количества
- Удаление товара
- Очистка после оформления заказа

Эндпоинты:

- `GET /api/Basket` — получить корзину
- `POST /api/Basket` — добавить товар
- `PATCH /api/Basket/{productId}` — изменить количество
- `DELETE /api/Basket/{productId}` — удалить товар
- `POST /api/Basket/checkout` — оформить заказ

### Ordering

Управление заказами с отслеживанием статусов.

- Создание заказа из корзины (Checkout)
- Жизненный цикл: `Pending` -> `Paid` -> `Shipped` -> `Delivered`
- Отмена заказа пользователем (до отправки)
- Управление статусами администратором
- Цена и название товара копируются в заказ на момент оформления

Эндпоинты:

- `GET /api/Ordering?page=1&pageSize=10` — заказы пользователя
- `GET /api/Ordering/all?page=1&pageSize=50` — все заказы (Admin)
- `GET /api/Ordering/{id}` — заказ по ID
- `POST /api/Ordering/{id}/pay` — оплатить
- `POST /api/Ordering/{id}/cancel` — отменить
- `POST /api/Ordering/{id}/ship` — отправить (Admin)
- `POST /api/Ordering/{id}/deliver` — доставить (Admin)

### Admin Panel

Веб-интерфейс для управления магазином.

- Управление товарами: добавление, редактирование, удаление, загрузка изображений
- Управление заказами: просмотр всех заказов, смена статусов
- Доступ только для роли `Admin`

## База данных

Все модули используют единую PostgreSQL базу данных (в перспективе — отдельные базы для каждого сервиса).

Таблицы:

- `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles` — Identity
- `Products` — Catalog
- `Orders`, `OrderItems` — Ordering
- Корзина хранится в Redis, не в PostgreSQL

## Конфигурация

Все настройки через переменные окружения в `docker-compose.yml`.

| Переменная | Описание | По умолчанию |
|---|---|---|
| `POSTGRES_PASSWORD` | Пароль PostgreSQL | — |
| `JWT_SECRET_KEY` | Секретный ключ JWT (минимум 32 символа) | — |
| `ConnectionStrings__Redis` | Адрес Redis | `redis:6379` |
| `Minio__Endpoint` | Адрес MinIO | `minio:9000` |

## Запуск для разработки (без Docker)

1. Запустить PostgreSQL, Redis и MinIO любым способом (локально или через Docker):

```bash
docker run -d --name postgres -p 5432:5432 -e POSTGRES_PASSWORD=password -e POSTGRES_DB=Shop postgres:15
docker run -d --name redis -p 6379:6379 redis:latest
docker run -d --name minio -p 9000:9000 -p 9001:9001 -e MINIO_ROOT_USER=minioadmin -e MINIO_ROOT_PASSWORD=minioadmin minio/minio server /data --console-address ":9001"
```

2. Настроить `appsettings.Development.json` или user-secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=Shop;Username=postgres;Password=password"
dotnet user-secrets set "JwtSettings:SecretKey" "your_secret_key_at_least_32_chars"
```

3. Запустить бэкенд:

```bash
cd src/Host
dotnet run
```

4. Запустить фронтенд (в другом терминале):

```bash
cd src/frontend
npm install
npm run dev
```
