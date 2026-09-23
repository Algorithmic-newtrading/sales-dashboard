export interface Kpi {
  revenue: number;
  grossProfit: number;
  margin: number;
  salesCount: number;
  averageCheck: number;
  bestManagerName: string | null;
  bestManagerGrossProfit: number | null;
  revenueDelta: number;
  grossProfitDelta: number;
  marginDelta: number;
  averageCheckDelta: number;
}

export interface ManagerRating {
  rank: number;
  managerId: number;
  fullName: string;
  team: string;
  position: string;
  avatarColor: string;
  salesCount: number;
  revenue: number;
  grossProfit: number;
  averageCheck: number;
  margin: number;
  deltaPercent: number;
}

export interface TimelinePoint {
  date: string;
  revenue: number;
  grossProfit: number;
  salesCount: number;
}

export interface CategoryStat {
  categoryId: number;
  categoryName: string;
  revenue: number;
  grossProfit: number;
  salesCount: number;
}

export interface TopProduct {
  productId: number;
  productName: string;
  categoryName: string;
  revenue: number;
  quantity: number;
}

export interface RecentSale {
  id: number;
  date: string;
  managerName: string;
  customerName: string;
  products: string;
  status: string;
  total: number;
  grossProfit: number;
}