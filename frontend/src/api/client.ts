import axios from "axios";

export const api = axios.create({
  baseURL: "/api",
  timeout: 30000,
});

api.interceptors.response.use(
  (r) => r,
  (err) => {
    console.error("[API error]", err?.response?.status, err?.config?.url, err?.message);
    return Promise.reject(err);
  }
);

export interface Range {
  from: string;
  to: string;
}

export const toDateOnly = (d: Date): string => {
  const y = d.getFullYear();
  const m = String(d.getMonth() + 1).padStart(2, "0");
  const day = String(d.getDate()).padStart(2, "0");
  return `${y}-${m}-${day}`;
};