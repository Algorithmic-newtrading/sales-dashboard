import { api } from "./client";
import type { Range } from "./client";
import type {
  Kpi,
  ManagerRating,
  TimelinePoint,
  CategoryStat,
  TopProduct,
  RecentSale,
} from "../types";

export const getKpi = (r: Range) =>
  api.get<Kpi>("/analytics/kpi", { params: r }).then((x) => x.data);

export const getManagers = (r: Range, sortBy: "grossProfit" | "avgCheck") =>
  api
    .get<ManagerRating[]>("/analytics/managers", { params: { ...r, sortBy } })
    .then((x) => x.data);

export const getTimeline = (r: Range, granularity: "day" | "month" = "day") =>
  api
    .get<TimelinePoint[]>("/analytics/timeline", { params: { ...r, granularity } })
    .then((x) => x.data);

export const getCategories = (r: Range) =>
  api.get<CategoryStat[]>("/analytics/categories", { params: r }).then((x) => x.data);

export const getTopProducts = (r: Range, limit = 10) =>
  api
    .get<TopProduct[]>("/analytics/top-products", { params: { ...r, limit } })
    .then((x) => x.data);

export const getRecentSales = (r: Range, limit = 20) =>
  api
    .get<RecentSale[]>("/analytics/recent-sales", { params: { ...r, limit } })
    .then((x) => x.data);