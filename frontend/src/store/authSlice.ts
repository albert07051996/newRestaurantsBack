import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import apiClient, { API_ENDPOINTS } from '../config/api';

interface AuthState {
  token: string | null;
  expiresAt: string | null;
  loading: boolean;
  error: string | null;
}

const initialState: AuthState = {
  token: localStorage.getItem('authToken'),
  expiresAt: localStorage.getItem('tokenExpiresAt'),
  loading: false,
  error: null,
};

export const login = createAsyncThunk(
  'auth/login',
  async (credentials: { email: string; password: string }, { rejectWithValue }) => {
    try {
      const response = await apiClient.post<{ token: string; expiresAt: string }>(
        API_ENDPOINTS.LOGIN,
        credentials
      );
      return response.data;
    } catch (error: unknown) {
      if (error && typeof error === 'object' && 'response' in error) {
        const axiosError = error as { response?: { status?: number } };
        if (axiosError.response?.status === 401) {
          return rejectWithValue('არასწორი ელ-ფოსტა ან პაროლი');
        }
      }
      return rejectWithValue('შესვლა ვერ მოხერხდა');
    }
  }
);

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    logout(state) {
      state.token = null;
      state.expiresAt = null;
      localStorage.removeItem('authToken');
      localStorage.removeItem('tokenExpiresAt');
    },
    clearAuthError(state) {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(login.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(login.fulfilled, (state, action) => {
        state.loading = false;
        state.token = action.payload.token;
        state.expiresAt = action.payload.expiresAt;
        localStorage.setItem('authToken', action.payload.token);
        localStorage.setItem('tokenExpiresAt', action.payload.expiresAt);
      })
      .addCase(login.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

export const { logout, clearAuthError } = authSlice.actions;
export default authSlice.reducer;
