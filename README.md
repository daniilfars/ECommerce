# ECommerce - Интернет-магазин на ASP.NET Core (микросервисы)

Микросервисная архитектура на ASP.NET Core с React-фронтендом. 4 независимых сервиса, API Gateway, своя БД у каждого, корзина на Redis, изображения в MinIO.

## Стек

**Backend:** C# 14, ASP.NET Core 10, Entity Framework Core, MediatR, JWT, YARP  
**Database:** PostgreSQL (отдельная БД на сервис), Redis  
**Storage:** MinIO (S3-совместимое)  
**Frontend:** React  
**DevOps:** Docker Compose, Serilog  

## Архитектура

4 микросервиса + API Gateway:

```
Microservices/
├── Gateway/                    # YARP API Gateway (порт 5000)
├── Catalog/                    # Товары, изображения, MinIO
├── Identity/                   # Пользователи, JWT-токены
├── Basket/                     # Корзина на Redis
├── Ordering/                   # Заказы, статусы
├── Shared/                     # Общие абстракции
├── docker-compose.yml
└── .env

src/
└── frontend/                   # React SPA
```

Взаимодействие между сервисами через HTTP с пробросом JWT-токена. В перспективе — RabbitMQ/MassTransit.

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

2. Создать файл `.env` в папке `Microservices/`:

```env
POSTGRES_PASSWORD=your_secure_password
JWT_SECRET_KEY=your_secret_key_at_least_32_characters_long
MINIO_ROOT_USER=minioadmin
MINIO_ROOT_PASSWORD=minioadmin
```

3. Запустить все микросервисы:

```bash
cd Microservices
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
- API Gateway: `http://localhost:5000`
- Swagger Catalog: `http://localhost:8081/swagger`
- Swagger Identity: `http://localhost:8082/swagger`
- Swagger Basket: `http://localhost:8083/swagger`
- Swagger Ordering: `http://localhost:8084/swagger`

### Учетные данные по умолчанию

При первом запуске автоматически создается администратор:

- Email: `admin@shop.com`
- Пароль: `Admin123!`

Обычного пользователя можно зарегистрировать через интерфейс.

## Сервисы

| Сервис | Порт | База данных |
|--------|------|-------------|
| API Gateway | 5000 | — |
| Identity | 8082 | IdentityDb (PostgreSQL) |
| Catalog | 8081 | CatalogDb (PostgreSQL) + MinIO |
| Basket | 8083 | Redis |
| Ordering | 8084 | OrderingDb (PostgreSQL) |

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

- Добавление товара (цена и название подтягиваются из Catalog через HTTP)
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

- Создание заказа из корзины (Checkout, через HTTP от Basket)
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

Каждый микросервис использует свою PostgreSQL базу данных. Корзина хранится в Redis.

Таблицы:

- `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles` — IdentityDb
- `Products` — CatalogDb
- `Orders`, `OrderItems` — OrderingDb

## Конфигурация

Все настройки через переменные окружения в `docker-compose.yml`.

| Переменная | Описание |
|---|---|
| `POSTGRES_PASSWORD` | Пароль PostgreSQL (все БД) |
| `JWT_SECRET_KEY` | Секретный ключ JWT (минимум 32 символа) |
| `MINIO_ROOT_USER` | Логин MinIO |
| `MINIO_ROOT_PASSWORD` | Пароль MinIO |
