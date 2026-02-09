import { configureStore } from '@reduxjs/toolkit';
import { useDispatch, useSelector, type TypedUseSelectorHook } from 'react-redux';
import menuReducer from './menuSlice';
import orderReducer from './orderSlice';
import cartReducer from './cartSlice';
import authReducer from './authSlice';
import reservationReducer from './reservationSlice';
import tableSessionReducer from './tableSessionSlice';

export const store = configureStore({
  reducer: {
    menu: menuReducer,
    orders: orderReducer,
    cart: cartReducer,
    auth: authReducer,
    reservations: reservationReducer,
    tableSessions: tableSessionReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;
