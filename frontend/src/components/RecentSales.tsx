import { useQuery } from "@tanstack/react-query";
import { getRecentSales } from "../api/analytics";
import { useRange } from "../hooks/useRange";

const fmtMoney = (n: number) =>
  n.toLocaleString("ru-RU", { maximumFractionDigits: 0 });

const fmtDate = (s: string) =>
  new Date(s).toLocaleString("ru-RU", {
    day: "2-digit",
    month: "2-digit",
    year: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  });

export function RecentSalesBlock() {
  const { asRange } = useRange();
  const range = asRange();

  const { data, isLoading, isError } = useQuery({
    queryKey: ["recent-sales", range.from, range.to],
    queryFn: () => getRecentSales(range, 15),
  });

  const sales = Array.isArray(data) ? data : [];

  return (
    <div className="rounded-2xl border border-[#232a44] bg-[#151a2d] p-5">
      <h2 className="text-lg font-semibold mb-3">Последние продажи</h2>

      {isLoading && (
        <div className="space-y-2">
          {[1, 2, 3, 4, 5].map((i) => (
            <div key={i} className="h-10 bg-[#0f1424] rounded-lg animate-pulse" />
          ))}
        </div>
      )}

      {isError && <div className="text-rose-400 text-sm">Ошибка загрузки</div>}

      {!isLoading && !isError && sales.length === 0 && (
        <div className="text-slate-400 text-sm">За период продаж нет</div>
      )}

      {sales.length > 0 && (
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="text-left text-slate-400 border-b border-[#232a44]">
                <th className="py-2 pr-3">Дата</th>
                <th className="py-2 pr-3">Менеджер</th>
                <th className="py-2 pr-3">Клиент</th>
                <th className="py-2 pr-3">Товары</th>
                <th className="py-2 pr-3">Статус</th>
                <th className="py-2 pr-3 text-right">Сумма</th>
                <th className="py-2 text-right">Прибыль</th>
              </tr>
            </thead>
            <tbody>
              {sales.map((s) => (
                <tr
                  key={s.id}
                  className="border-b border-[#1e2540] hover:bg-[#0f1424]"
                >
                  <td className="py-2 pr-3 text-slate-400 whitespace-nowrap">
                    {fmtDate(s.date)}
                  </td>
                  <td className="py-2 pr-3 whitespace-nowrap">{s.managerName}</td>
                  <td className="py-2 pr-3 text-slate-300 truncate max-w-[180px]">
                    {s.customerName}
                  </td>
                  <td className="py-2 pr-3 text-slate-400 truncate max-w-[200px]">
                    {s.products}
                  </td>
                  <td className="py-2 pr-3">
                    <span className="text-xs px-2 py-0.5 rounded-md bg-emerald-500/15 text-emerald-400">
                      {s.status}
                    </span>
                  </td>
                  <td className="py-2 pr-3 text-right whitespace-nowrap">
                    {fmtMoney(s.total)} ₽
                  </td>
                  <td className="py-2 text-right text-emerald-400 whitespace-nowrap">
                    {fmtMoney(s.grossProfit)} ₽
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}