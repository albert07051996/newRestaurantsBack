export enum OrderType {
  DineIn = 'DineIn',
  TakeAway = 'TakeAway',
  Delivery = 'Delivery',
}

export enum OrderStatus {
  Pending = 'Pending',
  Confirmed = 'Confirmed',
  Preparing = 'Preparing',
  Ready = 'Ready',
  Delivered = 'Delivered',
  Cancelled = 'Cancelled',
}

export enum ReservationStatus {
  Pending = 'Pending',
  Confirmed = 'Confirmed',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
}

export enum TableSessionStatus {
  Active = 'Active',
  Closed = 'Closed',
}

export interface OrderItemRequest {
  dishId: string;
  quantity: number;
  specialInstructions?: string;
}

export interface OrderItemResponse {
  id: string;
  dishId: string;
  dishNameKa: string;
  dishNameEn: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  specialInstructions?: string;
}

export interface OrderResponse {
  id: string;
  orderNumber: string;
  customerName: string;
  customerPhone: string;
  customerAddress?: string;
  orderType: string;
  status: string;
  tableNumber?: number;
  notes?: string;
  totalAmount: number;
  tableSessionId?: string;
  createdAt: string;
  updatedAt: string;
  items: OrderItemResponse[];
}

export interface CreateOrderRequest {
  customerName: string;
  customerPhone: string;
  customerAddress?: string;
  orderType: string;
  tableNumber?: number;
  notes?: string;
  tableSessionId?: string;
  items: OrderItemRequest[];
}

export interface ReservationItemRequest {
  dishId: string;
  quantity: number;
  specialInstructions?: string;
}

export interface ReservationItemResponse {
  id: string;
  dishId: string;
  dishNameKa: string;
  dishNameEn: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  specialInstructions?: string;
}

export interface ReservationResponse {
  id: string;
  reservationNumber: string;
  customerName: string;
  customerPhone: string;
  reservationDate: string;
  reservationTime: string;
  guestCount: number;
  tableNumber: number;
  notes?: string;
  status: string;
  totalAmount: number;
  createdAt: string;
  updatedAt: string;
  items: ReservationItemResponse[];
}

export interface CreateReservationRequest {
  customerName: string;
  customerPhone: string;
  reservationDate: string;
  reservationTime: string;
  guestCount: number;
  tableNumber: number;
  notes?: string;
  items: ReservationItemRequest[];
}

export interface TableSessionResponse {
  id: string;
  sessionNumber: string;
  tableNumber: number;
  customerName: string;
  customerPhone: string;
  status: string;
  totalAmount: number;
  createdAt: string;
  updatedAt: string;
  orders: OrderResponse[];
}

export interface CartItem {
  dishId: string;
  quantity: number;
  specialInstructions?: string;
}
