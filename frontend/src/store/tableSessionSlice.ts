import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import apiClient, { API_ENDPOINTS } from '../config/api';
import type { TableSessionResponse } from '../types/order';

interface TableSessionState {
  sessions: TableSessionResponse[];
  currentSession: TableSessionResponse | null;
  loading: boolean;
  error: string | null;
}

const initialState: TableSessionState = {
  sessions: [],
  currentSession: null,
  loading: false,
  error: null,
};

export const fetchTableSessions = createAsyncThunk(
  'tableSessions/fetchAll',
  async (_, { rejectWithValue }) => {
    try {
      const response = await apiClient.get<TableSessionResponse[]>(API_ENDPOINTS.TABLE_SESSIONS);
      return response.data;
    } catch (error: unknown) {
      const msg = error instanceof Error ? error.message : 'სესიების ჩატვირთვა ვერ მოხერხდა';
      return rejectWithValue(msg);
    }
  }
);

export const fetchActiveSessions = createAsyncThunk(
  'tableSessions/fetchActive',
  async (_, { rejectWithValue }) => {
    try {
      const response = await apiClient.get<TableSessionResponse[]>(API_ENDPOINTS.TABLE_SESSIONS_ACTIVE);
      return response.data;
    } catch (error: unknown) {
      const msg = error instanceof Error ? error.message : 'აქტიური სესიების ჩატვირთვა ვერ მოხერხდა';
      return rejectWithValue(msg);
    }
  }
);

export const fetchActiveSessionForTable = createAsyncThunk(
  'tableSessions/fetchForTable',
  async (tableNumber: number, { rejectWithValue }) => {
    try {
      const response = await apiClient.get<TableSessionResponse>(API_ENDPOINTS.TABLE_SESSION_FOR_TABLE(tableNumber));
      return response.data;
    } catch (error: unknown) {
      const msg = error instanceof Error ? error.message : 'მაგიდის სესია ვერ მოიძებნა';
      return rejectWithValue(msg);
    }
  }
);

export const closeTableSession = createAsyncThunk(
  'tableSessions/close',
  async (id: string, { rejectWithValue }) => {
    try {
      const response = await apiClient.put<TableSessionResponse>(API_ENDPOINTS.TABLE_SESSION_CLOSE(id));
      return response.data;
    } catch (error: unknown) {
      const msg = error instanceof Error ? error.message : 'სესიის დახურვა ვერ მოხერხდა';
      return rejectWithValue(msg);
    }
  }
);

const tableSessionSlice = createSlice({
  name: 'tableSessions',
  initialState,
  reducers: {
    clearSessionError(state) {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchTableSessions.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchTableSessions.fulfilled, (state, action) => {
        state.loading = false;
        state.sessions = action.payload;
      })
      .addCase(fetchTableSessions.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(fetchActiveSessions.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchActiveSessions.fulfilled, (state, action) => {
        state.loading = false;
        state.sessions = action.payload;
      })
      .addCase(fetchActiveSessions.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(fetchActiveSessionForTable.fulfilled, (state, action) => {
        state.currentSession = action.payload;
      })
      .addCase(closeTableSession.fulfilled, (state, action) => {
        const idx = state.sessions.findIndex((s) => s.id === action.payload.id);
        if (idx !== -1) state.sessions[idx] = action.payload;
      });
  },
});

export const { clearSessionError } = tableSessionSlice.actions;
export default tableSessionSlice.reducer;
