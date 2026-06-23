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
- GET /api/restaurants?city={city} — список ресторанов по городу
- POST /api/orders/optimize — подобрать варианты заказа

## Enum'ы
OptimizationMode: 0 = MaxDiversity, 1 = MaxCalories
Category: 0 = Entree, 1 = Garnish, 2 = Dessert, 3 = Drinks, 4 = Sauces, 5 = Breakfas

## Как пользователь работает с приложением
Открывает сайт → выбирает город
GET /api/restaurants?city=Пенза → получает список ресторанов
Выбирает ресторан, бюджет, режим (максимально сытно или максимально разнообразно) и категории блюд, которые будут входить в заказ
POST /api/orders/optimize → получает варианты заказа
В response есть manual - инструкция что и как купить для каждого варианта
