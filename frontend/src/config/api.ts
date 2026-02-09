import axios from 'axios';
import { message } from 'antd';

const API_BASE_URL = import.meta.env.VITE_API_URL || '/api';

export const API_ENDPOINTS = {
  // Auth
  LOGIN: '/Auth/login',

  // Dishes
  DISHES: '/Dish',
  DISH_BY_ID: (id: string) => `/Dish/${id}`,
  DISH_RESTORE: (id: string) => `/Dish/${id}/restore`,

  // Categories
  CATEGORIES: '/Categories',
  DISH_CATEGORIES: '/DishCategory',
  DISH_CATEGORY_BY_ID: (id: string) => `/DishCategory/${id}`,

  // Orders
  ORDERS: '/Order',
  ORDER_BY_ID: (id: string) => `/Order/${id}`,
  ORDER_BY_NUMBER: (orderNumber: string) => `/Order/by-number/${orderNumber}`,
  ORDERS_BY_STATUS: (status: string) => `/Order/status/${status}`,
  ORDER_STATUS: (id: string) => `/Order/${id}/status`,

  // Reservations
  RESERVATIONS: '/Reservation',
  RESERVATION_BY_ID: (id: string) => `/Reservation/${id}`,
  RESERVATION_BY_NUMBER: (number: string) => `/Reservation/by-number/${number}`,
  RESERVATIONS_BY_STATUS: (status: string) => `/Reservation/status/${status}`,
  RESERVATIONS_BY_DATE: (date: string) => `/Reservation/date/${date}`,
  RESERVATION_STATUS: (id: string) => `/Reservation/${id}/status`,

  // Table Sessions
  TABLE_SESSIONS: '/TableSession',
  TABLE_SESSION_BY_ID: (id: string) => `/TableSession/${id}`,
  TABLE_SESSIONS_ACTIVE: '/TableSession/active',
  TABLE_SESSION_FOR_TABLE: (tableNumber: number) => `/TableSession/table/${tableNumber}/active`,
  TABLE_SESSION_CLOSE: (id: string) => `/TableSession/${id}/close`,
} as const;

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor - attach auth token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      // Check token expiration
      const expiresAt = localStorage.getItem('tokenExpiresAt');
      if (expiresAt && new Date(expiresAt) < new Date()) {
        localStorage.removeItem('authToken');
        localStorage.removeItem('tokenExpiresAt');
        window.location.href = '/admin/login';
        return Promise.reject(new Error('Token expired'));
      }
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor - handle errors
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('authToken');
      localStorage.removeItem('tokenExpiresAt');
      if (window.location.pathname.startsWith('/admin')) {
        window.location.href = '/admin/login';
      }
    }
    if (error.code === 'ECONNABORTED') {
      message.error('მოთხოვნის დრო ამოიწურა. სცადეთ ხელახლა.');
    }
    return Promise.reject(error);
  }
);

export default apiClient;
