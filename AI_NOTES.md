# AI Notes

## Инструменты

- Claude Code (Opus 4.5) — основной ассистент: каркас, сущности, seed, сервис аналитики, компоненты React, docker-конфиги, документация.
- VS Code / C# Dev Kit — редактирование, автодополнение, отладка.

## Что делегировал AI

- Структура проекта и каркас.
- 6 сущностей EF Core.
- SeedData и SaleSeeder (9507 продаж с сезонностью).
- AnalyticsService с 6 методами.
- Контроллер и DTO.
- Frontend-компоненты.
- Zustand store и QueryClient.
- Dockerfile для frontend и nginx.conf.
- README, AI_PROMPTS, AI_NOTES.

## Что проектировал сам

- Трактовка Refunded/Cancelled — исключаются из всех агрегатов.
- Business rules: формулы Revenue, Gross Profit, Margin, Average Check, Previous period.
- Индексы БД под конкретные запросы.
- Отказ от Clean Architecture / CQRS / Repository — для этой задачи проще.
- Финальная валидация — curl, Swagger, сравнение цифр.

## Где AI заметно ускорил работу

- Seed-генерация — 9507 продаж с реалистичными паттернами.
- 6 компонентов React с Tailwind — ~1.5 часа экономии.
- Dockerfile + nginx.conf.

## Где AI ошибался

1. Dockerfile с неверными путями (context = backend/, а csproj в backend/SalesDashboard.Api/).
2. Не учёл требование Npgsql UTC — добавлен Normalize с DateTime.SpecifyKind.
3. Recharts 3 + React 19 конфликт — откатили до 2.15.4 и добавили Array.isArray(data).
4. Vite-proxy не был настроен с первого раза — axios запрашивал HTML вместо JSON.
5. Глобальный ~/.gitignore-global с `*` игнорировал все файлы — обнаружили через git check-ignore.

## Как проверял

- Backend: curl по 6 эндпоинтам, Swagger UI, docker compose logs.
- Frontend: DevTools Console на ошибки, Network на статусы, ручное переключение.
- Docker: docker compose down -v && up --build — воспроизводимость с нуля.
- Edge cases: пустой диапазон, невалидный sortBy, короткий период.

## Что улучшить в AI-процессе

- Давать контекст о версиях сразу (React 19, Tailwind 3, Recharts 2).
- Просить проверять пути в Dockerfile с учётом контекста.
- Тестировать каждый шаг сразу, не накапливать.
- Спрашивать «почему», не только «как исправить».