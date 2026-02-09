import React, { useState, useCallback } from 'react';
import { Modal, Form, Input, Select, InputNumber, Button, message, Result } from 'antd';
import { useAppDispatch, useAppSelector } from '../../store';
import { createOrder } from '../../store/orderSlice';
import { clearCart } from '../../store/cartSlice';
import { OrderType } from '../../types/order';
import type { CreateOrderRequest } from '../../types/order';

interface CheckoutModalProps {
  visible: boolean;
  onClose: () => void;
  isQrMode: boolean;
  tableNumber?: number;
  tableSessionId?: string;
}

const CheckoutModal: React.FC<CheckoutModalProps> = ({
  visible,
  onClose,
  isQrMode,
  tableNumber,
  tableSessionId,
}) => {
  const dispatch = useAppDispatch();
  const cartItems = useAppSelector((s) => s.cart.items);
  const { loading } = useAppSelector((s) => s.orders);
  const [form] = Form.useForm();
  const [orderNumber, setOrderNumber] = useState<string | null>(null);

  const orderType = isQrMode ? OrderType.DineIn : OrderType.TakeAway;

  const handleSubmit = useCallback(
    async (values: { customerName: string; customerPhone: string; customerAddress?: string; orderType: string; tableNumber?: number; notes?: string }) => {
      if (cartItems.length === 0) {
        message.warning('კალათა ცარიელია');
        return;
      }

      const orderData: CreateOrderRequest = {
        customerName: values.customerName,
        customerPhone: values.customerPhone,
        customerAddress: values.customerAddress,
        orderType: values.orderType,
        tableNumber: values.tableNumber ?? tableNumber,
        notes: values.notes,
        tableSessionId,
        items: cartItems.map((ci) => ({
          dishId: ci.dishId,
          quantity: ci.quantity,
          specialInstructions: ci.specialInstructions,
        })),
      };

      try {
        const result = await dispatch(createOrder(orderData)).unwrap();
        setOrderNumber(result.orderNumber);
        dispatch(clearCart());
        message.success('შეკვეთა წარმატებით გაიგზავნა!');
      } catch {
        message.error('შეკვეთის გაგზავნა ვერ მოხერხდა');
      }
    },
    [cartItems, dispatch, tableNumber, tableSessionId]
  );

  const handleClose = useCallback(() => {
    setOrderNumber(null);
    form.resetFields();
    onClose();
  }, [form, onClose]);

  if (orderNumber) {
    return (
      <Modal open={visible} onCancel={handleClose} footer={null}>
        <Result
          status="success"
          title="შეკვეთა მიღებულია!"
          subTitle={`შეკვეთის ნომერი: ${orderNumber}`}
          extra={
            <Button type="primary" onClick={handleClose}>
              დახურვა
            </Button>
          }
        />
      </Modal>
    );
  }

  return (
    <Modal
      title="შეკვეთის გაფორმება"
      open={visible}
      onCancel={handleClose}
      footer={null}
      width={500}
    >
      <Form
        form={form}
        layout="vertical"
        onFinish={handleSubmit}
        initialValues={{ orderType }}
      >
        <Form.Item
          label="სახელი"
          name="customerName"
          rules={[{ required: true, message: 'გთხოვთ შეიყვანოთ სახელი' }]}
        >
          <Input placeholder="თქვენი სახელი" />
        </Form.Item>

        <Form.Item
          label="ტელეფონი"
          name="customerPhone"
          rules={[{ required: true, message: 'გთხოვთ შეიყვანოთ ტელეფონი' }]}
        >
          <Input placeholder="5XX XXX XXX" />
        </Form.Item>

        <Form.Item label="შეკვეთის ტიპი" name="orderType">
          <Select disabled={isQrMode}>
            <Select.Option value={OrderType.DineIn}>ადგილზე</Select.Option>
            <Select.Option value={OrderType.TakeAway}>წასაღები</Select.Option>
            <Select.Option value={OrderType.Delivery}>მიტანის სერვისი</Select.Option>
          </Select>
        </Form.Item>

        <Form.Item
          noStyle
          shouldUpdate={(prev, cur) => prev.orderType !== cur.orderType}
        >
          {({ getFieldValue }) =>
            getFieldValue('orderType') === OrderType.Delivery ? (
              <Form.Item
                label="მისამართი"
                name="customerAddress"
                rules={[{ required: true, message: 'გთხოვთ შეიყვანოთ მისამართი' }]}
              >
                <Input placeholder="მიტანის მისამართი" />
              </Form.Item>
            ) : null
          }
        </Form.Item>

        <Form.Item
          noStyle
          shouldUpdate={(prev, cur) => prev.orderType !== cur.orderType}
        >
          {({ getFieldValue }) =>
            getFieldValue('orderType') === OrderType.DineIn && !isQrMode ? (
              <Form.Item label="მაგიდის ნომერი" name="tableNumber">
                <InputNumber min={1} style={{ width: '100%' }} placeholder="მაგიდის ნომერი" />
              </Form.Item>
            ) : null
          }
        </Form.Item>

        <Form.Item label="შენიშვნა" name="notes">
          <Input.TextArea rows={2} placeholder="დამატებითი ინფორმაცია" />
        </Form.Item>

        <Form.Item>
          <Button
            type="primary"
            htmlType="submit"
            loading={loading}
            block
            size="large"
            disabled={cartItems.length === 0}
          >
            შეკვეთის გაგზავნა
          </Button>
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default CheckoutModal;
