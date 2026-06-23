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

## Как запустить Swagger
https://foodoptimizer-production.up.railway.app/swagger

## Endpoints
- GET /api/Cities — список городов, в котором есть хотя бы один ресторан
- GET /api/Brands?city={city} — список брендов по городу (у каждого бренда есть список точек (ресторанов) бренда)
- POST /api/orders/optimize — подобрать варианты заказа

## Enum'ы
OptimizationMode: 0 = MaxDiversity, 1 = MaxCalories
Category: 0 = Entree, 1 = Garnish, 2 = Dessert, 3 = Drinks, 4 = Sauces, 5 = Breakfas

## Как пользователь работает с приложением
- Открывает сайт
- GET /api/Cities - получает список городов
- GET /api/Brands?city={city} → получает список брендов по городу
- По тому же запросу получает список конретных точек у бренда (Например, Вкусно и Точка на ул. Московской 73)
Выбирает бюджет, режим (максимально сытно или максимально разнообразно) и категории блюд, которые будут входить в заказ
POST /api/orders/optimize → получает варианты заказа
В response есть manual - инструкция что и как купить для каждого варианта
