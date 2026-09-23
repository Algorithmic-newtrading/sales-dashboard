export interface SmoothPoint {
  date: string;
  revenue: number;
  grossProfit: number;
  salesCount: number;
}

/**
 * Экспоненциальное сглаживание временного ряда.
 * @param data исходные точки
 * @param alpha коэффициент сглаживания (0..1). Чем меньше — тем плавнее.
 *              По умолчанию 0.3 — разумный баланс сигнал/шум.
 */
export function smoothTimeline(
  data: SmoothPoint[],
  alpha = 0.3,
): SmoothPoint[] {
  if (!Array.isArray(data) || data.length === 0) return [];

  const out: SmoothPoint[] = [];
  let prevRev = data[0].revenue;
  let prevGp = data[0].grossProfit;

  for (let i = 0; i < data.length; i++) {
    const p = data[i];
    const rev = i === 0 ? p.revenue : alpha * p.revenue + (1 - alpha) * prevRev;
    const gp =
      i === 0 ? p.grossProfit : alpha * p.grossProfit + (1 - alpha) * prevGp;
    prevRev = rev;
    prevGp = gp;

    out.push({
      date: p.date,
      revenue: Math.round(rev),
      grossProfit: Math.round(gp),
      salesCount: p.salesCount,
    });
  }

  return out;
}