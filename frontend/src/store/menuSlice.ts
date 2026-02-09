import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import apiClient, { API_ENDPOINTS } from '../config/api';
import type { MenuItem, DishCategory } from '../types/menu';

interface MenuState {
  items: MenuItem[];
  categories: DishCategory[];
  loading: boolean;
  error: string | null;
}

const initialState: MenuState = {
  items: [],
  categories: [],
  loading: false,
  error: null,
};

export const fetchMenuItems = createAsyncThunk(
  'menu/fetchItems',
  async (_, { rejectWithValue }) => {
    try {
      const response = await apiClient.get<MenuItem[]>(API_ENDPOINTS.DISHES);
      return response.data;
    } catch (error: unknown) {
      const message = error instanceof Error ? error.message : 'კერძების ჩატვირთვა ვერ მოხერხდა';
      return rejectWithValue(message);
    }
  }
);

export const fetchCategories = createAsyncThunk(
  'menu/fetchCategories',
  async (_, { rejectWithValue }) => {
    try {
      const response = await apiClient.get<DishCategory[]>(API_ENDPOINTS.CATEGORIES);
      return response.data;
    } catch (error: unknown) {
      const message = error instanceof Error ? error.message : 'კატეგორიების ჩატვირთვა ვერ მოხერხდა';
      return rejectWithValue(message);
    }
  }
);

const menuSlice = createSlice({
  name: 'menu',
  initialState,
  reducers: {
    clearMenuError(state) {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchMenuItems.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchMenuItems.fulfilled, (state, action) => {
        state.loading = false;
        state.items = action.payload;
      })
      .addCase(fetchMenuItems.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(fetchCategories.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchCategories.fulfilled, (state, action) => {
        state.loading = false;
        state.categories = action.payload;
      })
      .addCase(fetchCategories.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

export const { clearMenuError } = menuSlice.actions;
export default menuSlice.reducer;
