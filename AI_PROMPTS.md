# AI Prompts Log

Журнал ключевых запросов к AI-ассистентам в процессе работы над проектом.
Формат: `## ЧЧ:ММ — Инструмент / Модель` + дословный запрос.

## 19:55 — Claude Code / Claude Opus 4.5

Помоги развернуть каркас Full-Stack проекта: ASP.NET Core 8 Web API + React 19 + TypeScript + PostgreSQL.

## 20:10 — Claude Code / Claude Opus 4.5

Сгенерируй 6 сущностей EF Core для домена продаж: Manager, Customer, Category, Product, Sale, SaleItem.

## 20:15 — Claude Code / Claude Opus 4.5

Напиши SeedData и SaleSeeder с воспроизводимым Random, сезонностью, отменами и возвратами.

## 20:20 — Claude Code / Claude Opus 4.5

Спроектируй AnalyticsService с методами для KPI, рейтинга, timeline, категорий, топ-продуктов, последних продаж.

## 20:40 — Claude Code / Claude Opus 4.5

Помоги с миграцией: dotnet ef migrations add InitialCreate. Индексы на Sales(Date), Sales(Status, Date), Sales(ManagerId).

## 21:20 — Claude Code / Claude Opus 4.5

Создай компоненты React для dashboard: KpiCard, PeriodPicker, ManagerRating, Timeline, Categories, RecentSales.

## 21:35 — Claude Code / Claude Opus 4.5

Ошибка в браузере: Cannot read properties of undefined (reading 'toLocaleString') в Dashboard.tsx.

## 21:40 — Claude Code / Claude Opus 4.5

Ошибка: Cannot write DateTime with Kind=Unspecified to PostgreSQL type 'timestamp with time zone'.

## 21:45 — Claude Code / Claude Opus 4.5

Помоги написать Dockerfile для frontend: multi-stage node:20-alpine → nginx:alpine с проксированием /api на backend:8080.

## 21:50 — Claude Code / Claude Opus 4.5

Сгенерируй README.md с инструкцией запуска через docker compose up --build.