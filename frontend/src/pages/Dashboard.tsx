import { useQuery } from "@tanstack/react-query";
import { getKpi } from "../api/analytics";
import { useRange } from "../hooks/useRange";
import { KpiCard } from "../components/KpiCard";
import { PeriodPicker } from "../components/PeriodPicker";
import { ManagerRatingBlock } from "../components/ManagerRating";
import { TimelineBlock } from "../components/Timeline";
import { CategoriesBlock } from "../components/Categories";
import { ManagerScatterBlock } from "../components/ManagerScatter";
import { RecentSalesBlock } from "../components/RecentSales";
import { ErrorBoundary } from "../components/ErrorBoundary";

// Безопасные хелперы — не падают, если значение не число
const fmtMoney = (n: unknown): string => {
  const v = typeof n === "number" && Number.isFinite(n) ? n : 0;
  return v.toLocaleString("ru-RU", { maximumFractionDigits: 0 });
};

const fmtPct = (n: unknown): string => {
  const v = typeof n === "number" && Number.isFinite(n) ? n : 0;
  return `${v.toFixed(1)}%`;
};

export function Dashboard() {
  const { asRange } = useRange();
  const range = asRange();

  const { data: kpi, isLoading, isError } = useQuery({
    queryKey: ["kpi", range.from, range.to],
    queryFn: () => getKpi(range),
  });

  return (
    <div className="min-h-screen bg-[#0b1020] text-slate-100 p-6">
      <div className="max-w-[1440px] mx-auto space-y-5">
        {/* Header */}
        <header className="flex items-center justify-between flex-wrap gap-3">
          <div>
            <h1 className="text-2xl font-semibold">Sales Dashboard</h1>
            <p className="text-sm text-slate-400">Аналитика продаж менеджеров</p>
          </div>
          <PeriodPicker />
        </header>

        {/* KPI */}
        {isError && (
          <div className="rounded-xl bg-rose-500/10 text-rose-400 p-4 text-sm">
            Не удалось загрузить KPI. Проверьте backend.
          </div>
        )}

        {isLoading && (
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4">
            {[1, 2, 3, 4, 5, 6].map((i) => (
              <div
                key={i}
                className="h-[130px] bg-[#151a2d] rounded-2xl animate-pulse"
              />
            ))}
          </div>
        )}

        {kpi && (
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4">
            <KpiCard
              title="Выручка"
              value={`${fmtMoney(kpi.revenue)} ₽`}
              delta={kpi.revenueDelta}
              accent="#6366f1"
            />
            <KpiCard
              title="Валовая прибыль"
              value={`${fmtMoney(kpi.grossProfit)} ₽`}
              delta={kpi.grossProfitDelta}
              accent="#10b981"
            />
            <KpiCard
              title="Маржинальность"
              value={fmtPct(kpi.margin)}
              delta={kpi.marginDelta}
              accent="#f59e0b"
            />
            <KpiCard
              title="Кол-во продаж"
              value={`${kpi.salesCount ?? 0}`}
              accent="#0ea5e9"
            />
            <KpiCard
              title="Средний чек"
              value={`${fmtMoney(kpi.averageCheck)} ₽`}
              delta={kpi.averageCheckDelta}
              accent="#8b5cf6"
            />
            <KpiCard
              title="Лучший менеджер"
              value={kpi.bestManagerName ?? "—"}
              hint={
                kpi.bestManagerGrossProfit != null
                  ? `${fmtMoney(kpi.bestManagerGrossProfit)} ₽ прибыли`
                  : undefined
              }
              accent="#ef4444"
            />
          </div>
        )}

        {/* Timeline */}
        <ErrorBoundary fallbackTitle="Не удалось загрузить динамику">
          <TimelineBlock />
        </ErrorBoundary>

        {/* Категории + топ продуктов */}
        <ErrorBoundary fallbackTitle="Не удалось загрузить категории">
          <CategoriesBlock />
        </ErrorBoundary>

        {/* Матрица объём vs маржа */}
        <ErrorBoundary fallbackTitle="Не удалось загрузить матрицу">
          <ManagerScatterBlock />
        </ErrorBoundary>

        {/* Рейтинг менеджеров */}
        <ErrorBoundary fallbackTitle="Не удалось загрузить рейтинг">
          <ManagerRatingBlock />
        </ErrorBoundary>

        {/* Последние продажи */}
        <ErrorBoundary fallbackTitle="Не удалось загрузить продажи">
          <RecentSalesBlock />
        </ErrorBoundary>
      </div>
    </div>
  );
}