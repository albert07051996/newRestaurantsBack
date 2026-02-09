import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import apiClient, { API_ENDPOINTS } from '../config/api';
import type { OrderResponse, CreateOrderRequest } from '../types/order';

interface OrderState {
  orders: OrderResponse[];
  currentOrder: OrderResponse | null;
  loading: boolean;
  error: string | null;
}

const initialState: OrderState = {
  orders: [],
  currentOrder: null,
  loading: false,
  error: null,
};

export const fetchOrders = createAsyncThunk(
  'orders/fetchAll',
  async (_, { rejectWithValue }) => {
    try {
      const response = await apiClient.get<OrderResponse[]>(API_ENDPOINTS.ORDERS);
      return response.data;
    } catch (error: unknown) {
      const message = error instanceof Error ? error.message : 'შეკვეთების ჩატვირთვა ვერ მოხერხდა';
      return rejectWithValue(message);
    }
  }
);

export const fetchOrderByNumber = createAsyncThunk(
  'orders/fetchByNumber',
  async (orderNumber: string, { rejectWithValue }) => {
    try {
      const response = await apiClient.get<OrderResponse>(API_ENDPOINTS.ORDER_BY_NUMBER(orderNumber));
      return response.data;
    } catch (error: unknown) {
      const message = error instanceof Error ? error.message : 'შეკვეთა ვერ მოიძებნა';
      return rejectWithValue(message);
    }
  }
);

export const fetchOrdersByStatus = createAsyncThunk(
  'orders/fetchByStatus',
  async (status: string, { rejectWithValue }) => {
    try {
      const response = await apiClient.get<OrderResponse[]>(API_ENDPOINTS.ORDERS_BY_STATUS(status));
      return response.data;
    } catch (error: unknown) {
      const message = error instanceof Error ? error.message : 'შეკვეთების ფილტრაცია ვერ მოხერხდა';
      return rejectWithValue(message);
    }
  }
);

export const createOrder = createAsyncThunk(
  'orders/create',
  async (orderData: CreateOrderRequest, { rejectWithValue }) => {
    try {
      const response = await apiClient.post<OrderResponse>(API_ENDPOINTS.ORDERS, orderData);
      return response.data;
    } catch (error: unknown) {
      const message = error instanceof Error ? error.message : 'შეკვეთის შექმნა ვერ მოხერხდა';
      return rejectWithValue(message);
    }
  }
);

export const updateOrderStatus = createAsyncThunk(
  'orders/updateStatus',
  async ({ id, status }: { id: string; status: string }, { rejectWithValue }) => {
    try {
      const response = await apiClient.put<OrderResponse>(
        API_ENDPOINTS.ORDER_STATUS(id),
        { status }
      );
      return response.data;
    } catch (error: unknown) {
      const message = error instanceof Error ? error.message : 'სტატუსის განახლება ვერ მოხერხდა';
      return rejectWithValue(message);
    }
  }
);

export const cancelOrder = createAsyncThunk(
  'orders/cancel',
  async (id: string, { rejectWithValue }) => {
    try {
      await apiClient.delete(API_ENDPOINTS.ORDER_BY_ID(id));
      return id;
    } catch (error: unknown) {
      const message = error instanceof Error ? error.message : 'შეკვეთის გაუქმება ვერ მოხერხდა';
      return rejectWithValue(message);
    }
  }
);

const orderSlice = createSlice({
  name: 'orders',
  initialState,
  reducers: {
    clearOrderError(state) {
      state.error = null;
    },
    clearCurrentOrder(state) {
      state.currentOrder = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // fetchOrders
      .addCase(fetchOrders.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchOrders.fulfilled, (state, action) => {
        state.loading = false;
        state.orders = action.payload;
      })
      .addCase(fetchOrders.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      // fetchOrderByNumber
      .addCase(fetchOrderByNumber.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchOrderByNumber.fulfilled, (state, action) => {
        state.loading = false;
        state.currentOrder = action.payload;
      })
      .addCase(fetchOrderByNumber.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      // fetchOrdersByStatus
      .addCase(fetchOrdersByStatus.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchOrdersByStatus.fulfilled, (state, action) => {
        state.loading = false;
        state.orders = action.payload;
      })
      .addCase(fetchOrdersByStatus.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      // createOrder
      .addCase(createOrder.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createOrder.fulfilled, (state, action) => {
        state.loading = false;
        state.currentOrder = action.payload;
        state.orders.unshift(action.payload);
      })
      .addCase(createOrder.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      // updateOrderStatus
      .addCase(updateOrderStatus.fulfilled, (state, action) => {
        const idx = state.orders.findIndex((o) => o.id === action.payload.id);
        if (idx !== -1) state.orders[idx] = action.payload;
        if (state.currentOrder?.id === action.payload.id) {
          state.currentOrder = action.payload;
        }
      })
      // cancelOrder
      .addCase(cancelOrder.fulfilled, (state, action) => {
        state.orders = state.orders.filter((o) => o.id !== action.payload);
        if (state.currentOrder?.id === action.payload) {
          state.currentOrder = null;
        }
      });
  },
});

export const { clearOrderError, clearCurrentOrder } = orderSlice.actions;
export default orderSlice.reducer;
