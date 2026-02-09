import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import apiClient, { API_ENDPOINTS } from '../config/api';
import type { ReservationResponse, CreateReservationRequest } from '../types/order';

interface ReservationState {
  reservations: ReservationResponse[];
  currentReservation: ReservationResponse | null;
  loading: boolean;
  error: string | null;
}

const initialState: ReservationState = {
  reservations: [],
  currentReservation: null,
  loading: false,
  error: null,
};

export const fetchReservations = createAsyncThunk(
  'reservations/fetchAll',
  async (_, { rejectWithValue }) => {
    try {
      const response = await apiClient.get<ReservationResponse[]>(API_ENDPOINTS.RESERVATIONS);
      return response.data;
    } catch (error: unknown) {
      const msg = error instanceof Error ? error.message : 'რეზერვაციების ჩატვირთვა ვერ მოხერხდა';
      return rejectWithValue(msg);
    }
  }
);

export const fetchReservationByNumber = createAsyncThunk(
  'reservations/fetchByNumber',
  async (number: string, { rejectWithValue }) => {
    try {
      const response = await apiClient.get<ReservationResponse>(API_ENDPOINTS.RESERVATION_BY_NUMBER(number));
      return response.data;
    } catch (error: unknown) {
      const msg = error instanceof Error ? error.message : 'რეზერვაცია ვერ მოიძებნა';
      return rejectWithValue(msg);
    }
  }
);

export const createReservation = createAsyncThunk(
  'reservations/create',
  async (data: CreateReservationRequest, { rejectWithValue }) => {
    try {
      const response = await apiClient.post<ReservationResponse>(API_ENDPOINTS.RESERVATIONS, data);
      return response.data;
    } catch (error: unknown) {
      const msg = error instanceof Error ? error.message : 'რეზერვაციის შექმნა ვერ მოხერხდა';
      return rejectWithValue(msg);
    }
  }
);

export const updateReservationStatus = createAsyncThunk(
  'reservations/updateStatus',
  async ({ id, status }: { id: string; status: string }, { rejectWithValue }) => {
    try {
      const response = await apiClient.put<ReservationResponse>(
        API_ENDPOINTS.RESERVATION_STATUS(id),
        { status }
      );
      return response.data;
    } catch (error: unknown) {
      const msg = error instanceof Error ? error.message : 'სტატუსის განახლება ვერ მოხერხდა';
      return rejectWithValue(msg);
    }
  }
);

export const cancelReservation = createAsyncThunk(
  'reservations/cancel',
  async (id: string, { rejectWithValue }) => {
    try {
      await apiClient.delete(API_ENDPOINTS.RESERVATION_BY_ID(id));
      return id;
    } catch (error: unknown) {
      const msg = error instanceof Error ? error.message : 'რეზერვაციის გაუქმება ვერ მოხერხდა';
      return rejectWithValue(msg);
    }
  }
);

const reservationSlice = createSlice({
  name: 'reservations',
  initialState,
  reducers: {
    clearReservationError(state) {
      state.error = null;
    },
    clearCurrentReservation(state) {
      state.currentReservation = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchReservations.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchReservations.fulfilled, (state, action) => {
        state.loading = false;
        state.reservations = action.payload;
      })
      .addCase(fetchReservations.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(fetchReservationByNumber.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchReservationByNumber.fulfilled, (state, action) => {
        state.loading = false;
        state.currentReservation = action.payload;
      })
      .addCase(fetchReservationByNumber.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(createReservation.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createReservation.fulfilled, (state, action) => {
        state.loading = false;
        state.currentReservation = action.payload;
        state.reservations.unshift(action.payload);
      })
      .addCase(createReservation.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(updateReservationStatus.fulfilled, (state, action) => {
        const idx = state.reservations.findIndex((r) => r.id === action.payload.id);
        if (idx !== -1) state.reservations[idx] = action.payload;
        if (state.currentReservation?.id === action.payload.id) {
          state.currentReservation = action.payload;
        }
      })
      .addCase(cancelReservation.fulfilled, (state, action) => {
        state.reservations = state.reservations.filter((r) => r.id !== action.payload);
      });
  },
});

export const { clearReservationError, clearCurrentReservation } = reservationSlice.actions;
export default reservationSlice.reducer;
