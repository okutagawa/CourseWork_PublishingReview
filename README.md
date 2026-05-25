# PublishingReviewApp — Интернет-магазин (экзаменационная адаптация)

Реализована предметная область интернет-магазина: товары, категории, отзывы и рейтинги.

## Сущности
User, Product, Category, ProductReview, ProductRating, ProductCategory.

## Связи
- User 1-N ProductReview
- User 1-N ProductRating
- Product 1-N ProductReview
- Product 1-N ProductRating
- Product N-M Category (через ProductCategory)

## Страницы
- `/Products/Index` список товаров + поиск/фильтры
- `/Products/Details`
- `/Products/Create`, `/Products/Edit`, `/Products/Delete`
- `/Reviews/Index` список отзывов + фильтры
- `/Reviews/MyReviews`
- `/Reviews/Create`, `/Reviews/Edit`, `/Reviews/Delete`
- `/Home/Register`, `/Home/Enter`, `/Home/Logout`

## CRUD
- Product: полный CRUD
- Review: полный CRUD (только автор для edit/delete)
- Rating: управление через создание/обновление оценки при создании отзыва

## Запуск
1. `dotnet restore`
2. `dotnet build`
3. `dotnet run --project PublishingReviewApp`

## Инициализация БД
Приложение вызывает `Database.EnsureCreated()` при старте и добавляет seed-данные для категорий и товаров.

## Тестовые пользователи
Зарегистрируйте через `/Home/Register` минимум 3 пользователя (customer/employee).

## Поиск и фильтрация
- Products: `q`, `category`, `minPrice`, `maxPrice`, `minRating`
- Reviews: `q`, `rating`, `from`
