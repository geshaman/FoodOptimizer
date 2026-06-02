# FoodOptimizer

Приложение для подбора оптимального заказа в ресторанах быстрого питания.

## Описание
Пользователь выбирает ресторан, бюджет и режим оптимизации. 
Приложение генерирует несколько вариантов заказа с учётом скидок 
и выдаёт инструкцию к покупке.

## Стек
- Backend: ASP.NET Core 8, REST API
- База данных: PostgreSQL 18
- ORM: Entity Framework Core 8
- Архитектура: Layered Architecture

## Как запустить
1. Клонировать репозиторий
2. Установить PostgreSQL 18
3. В appsettings.json указать свою строку подключения
4. Применить миграции: dotnet ef database update --project FoodOptimizer.Infrastructure --startup-project FoodOptimizer.API
5. Запустить проект: dotnet run --project FoodOptimizer.API
6. Открыть Swagger
## Endpoints
- GET /api/restaurants?city={city} — список ресторанов по городу
- POST /api/orders/optimize — подобрать варианты заказа
