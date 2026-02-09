import { createSlice, type PayloadAction } from '@reduxjs/toolkit';
import type { CartItem } from '../types/order';

interface CartState {
  items: CartItem[];
}

// Only store dishId and quantity in localStorage (no prices - security)
const loadCart = (): CartItem[] => {
  try {
    const data = localStorage.getItem('cart');
    if (data) {
      const parsed = JSON.parse(data) as CartItem[];
      return parsed.map((item) => ({
        dishId: item.dishId,
        quantity: item.quantity,
        specialInstructions: item.specialInstructions,
      }));
    }
  } catch {
    // ignore
  }
  return [];
};

const saveCart = (items: CartItem[]) => {
  localStorage.setItem(
    'cart',
    JSON.stringify(items.map((i) => ({ dishId: i.dishId, quantity: i.quantity, specialInstructions: i.specialInstructions })))
  );
};

const initialState: CartState = {
  items: loadCart(),
};

const cartSlice = createSlice({
  name: 'cart',
  initialState,
  reducers: {
    addToCart(state, action: PayloadAction<CartItem>) {
      const existing = state.items.find((i) => i.dishId === action.payload.dishId);
      if (existing) {
        existing.quantity += action.payload.quantity;
      } else {
        state.items.push(action.payload);
      }
      saveCart(state.items);
    },
    removeFromCart(state, action: PayloadAction<string>) {
      state.items = state.items.filter((i) => i.dishId !== action.payload);
      saveCart(state.items);
    },
    updateQuantity(state, action: PayloadAction<{ dishId: string; quantity: number }>) {
      const item = state.items.find((i) => i.dishId === action.payload.dishId);
      if (item) {
        item.quantity = Math.max(1, action.payload.quantity);
        saveCart(state.items);
      }
    },
    clearCart(state) {
      state.items = [];
      localStorage.removeItem('cart');
    },
  },
});

export const { addToCart, removeFromCart, updateQuantity, clearCart } = cartSlice.actions;
export default cartSlice.reducer;
