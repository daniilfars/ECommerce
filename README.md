# ECommerce - Интернет-магазин на ASP.NET Core

<details>
  <summary><strong>Нажмите</strong>, чтобы посмотреть скриншоты проекта</summary>
  <br>
  <img src="https://github.com/user-attachments/assets/35ab9f00-a7e8-4032-acd5-341dadeccc67" alt="Регистрация" style="margin: 20px;" />
  <img src="https://github.com/user-attachments/assets/2a45959e-aac9-457f-af8a-745728f6cef5" alt="Главная страница" style="margin: 20px;" />
  <img src="https://github.com/user-attachments/assets/8ed8fe9d-2ea9-414d-a265-211f47721fed" alt="Каталог" style="margin: 20px;" />
  <img src="https://github.com/user-attachments/assets/a8126fb8-71bc-4b74-bc04-8ec1c5ccc055" alt="Страница товара" style="margin: 20px;" />
  <img src="https://github.com/user-attachments/assets/961a5f67-005b-456b-9c51-2528f0bfd051" alt="Корзина" style="margin: 20px;" />
  <img src="https://github.com/user-attachments/assets/a876613f-7f4e-46fd-abfc-26a3d48b5462" alt="Оформление заказа" style="margin: 20px;" />
  <img src="https://github.com/user-attachments/assets/2653a91e-c0c6-4efa-8e1c-f54fbe205ceb" alt="Оплата" style="margin: 20px;" />
  <img src="https://github.com/user-attachments/assets/179dc794-0407-460a-8d5a-456a44a29560" alt="Страница заказов" style="margin: 20px;" />
  <img src="https://github.com/user-attachments/assets/c7cdcd42-8eee-42d2-8b26-7deb3b617fb3" alt="Админ-панель" style="margin: 20px;" />
  <img src="https://github.com/user-attachments/assets/55abdfc3-70f1-4ee4-b39b-646fe35ae72f" alt="Мониторинг" style="margin: 20px;" />
</details>

<hr>

Микросервисная архитектура на ASP.NET Core с фронтендом на React. 5 независимых сервисов, API Gateway, своя БД у каждого, корзина на Redis, изображения в MinIO, оплата через ЮKassa, взаимодействие между микросервисами через MassTransit и gRPC.

## Стек

**Backend:** C# 14, ASP.NET Core 10, Entity Framework Core, MediatR, JWT, YARP, MassTransit, ElasticSearch
**Database:** PostgreSQL, Redis  
**Storage:** MinIO
**Payments:** ЮKassa  
**Frontend:** React  
**DevOps:** Docker Compose, Serilog, Grafana + Prometheus  

## Архитектура

5 микросервисов + API Gateway:
```
Microservices/
├── Gateway/                    # YARP API Gateway (порт 5000)
├── Catalog/                    # Товары, изображения, MinIO
├── Identity/                   # Пользователи, JWT-токены
├── Basket/                     # Корзина на Redis
├── Ordering/                   # Заказы, статусы, оплата через ЮKassa
├── Reviews/                    # Отзывы о купленных товаров
├── Shared/                     # Общие абстракции
├── docker-compose.yml
└── .env
src/
└── frontend/                   # React SPA
```

## Быстрый старт

### Предварительные требования

- .NET 10 SDK
- Node.js 18+
- Docker Desktop
- Аккаунт ЮKassa (тестовый магазин)

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
YOOKASSA_SHOP_ID=your_shop_id
YOOKASSA_SECRET_KEY=test_your_secret_key
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
- Swagger Catalog: `http://localhost:8081/swagger`
- Swagger Identity: `http://localhost:8082/swagger`
- Swagger Basket: `http://localhost:8083/swagger`
- Swagger Ordering: `http://localhost:8084/swagger`
- Swagger Reviews: `http://localhost:8085/swagger`

### Учетные данные по умолчанию

При первом запуске автоматически создается администратор:

- Email: `admin@shop.com`
- Пароль: `Admin123!`

Обычного пользователя можно зарегистрировать через интерфейс.

### Оплата (ЮKassa)

Для тестовой оплаты используется тестовый магазин ЮKassa. Тестовая карта: `5555 5555 5555 4477`, любой CVC, любая будущая дата.

## Сервисы

| Сервис | Порт | База данных |
|--------|------|-------------|
| API Gateway | 5000 | — |
| Identity | 8082 | IdentityDb (PostgreSQL) |
| Catalog | 8081 | CatalogDb (PostgreSQL) + MinIO |
| Basket | 8083 | Redis |
| Ordering | 8084 | OrderingDb (PostgreSQL) + ЮKassa |
| Reviews | 8085 | ReviewsDb (PostgreSQL) |

### Identity

Регистрация и вход через JWT-токены.

- Access-токен (15 минут) передается в заголовке `Authorization: Bearer`
- Refresh-токен (7 дней) хранится в httpOnly-куке
- Автоматическое обновление access-токена при истечении (ротация refresh-токена)
- Обнаружение повторного использования refresh-токена (защита от replay-атак)
- Роли: `User`, `Admin`

### Catalog

Управление товарами интернет-магазина.

- CRUD с пагинацией и фильтрами
- Загрузка изображений в MinIO
- Публичный просмотр для всех
- Создание, редактирование, удаление только для `Admin`
- При загрузке нового изображения старое удаляется из MinIO
- Полнотекстовый поиск через ElasticSearch

### Basket

Корзина на Redis с TTL 7 дней.

- Добавление товара (цена и название подтягиваются через gRPC)
- Изменение количества
- Удаление товара
- Очистка после оформления заказа

### Ordering

Управление заказами с отслеживанием статусов и оплатой через ЮKassa.

- Создание заказа из корзины (через MassTransit)
- Жизненный цикл: `Pending` -> `Confirmed` -> `Paid` -> `Shipped` -> `Delivered`
- Оплата через ЮKassa
- Отмена заказа пользователем
- Управление статусами администратором

### Reviews

Управление отзывами.

- Создание отзыва после доставки товара
- Возможность редактировать отзыв

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
- `Reviews` - ReviewsDb

## Конфигурация

Все настройки через переменные окружения в `docker-compose.yml`.

| Переменная | Описание |
|---|---|
| `POSTGRES_PASSWORD` | Пароль PostgreSQL (все БД) |
| `JWT_SECRET_KEY` | Секретный ключ JWT (минимум 32 символа) |
| `MINIO_ROOT_USER` | Логин MinIO |
| `MINIO_ROOT_PASSWORD` | Пароль MinIO |
| `YOOKASSA_SHOP_ID` | ID магазина ЮKassa |
| `YOOKASSA_SECRET_KEY` | Секретный ключ ЮKassa |

admin/admin - логин/пароль в Grafana
