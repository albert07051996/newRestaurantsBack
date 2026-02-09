import React, { useEffect, useState, useCallback } from 'react';
import {
  Form,
  Input,
  InputNumber,
  DatePicker,
  TimePicker,
  Button,
  Card,
  Table,
  message,
  Result,
  Select,
} from 'antd';
import { useAppDispatch, useAppSelector } from '../../store';
import { createReservation, clearCurrentReservation } from '../../store/reservationSlice';
import { fetchMenuItems } from '../../store/menuSlice';
import type { CreateReservationRequest, ReservationItemRequest } from '../../types/order';
import type { MenuItem } from '../../types/menu';
import dayjs from 'dayjs';

const ReservationPage: React.FC = () => {
  const dispatch = useAppDispatch();
  const { currentReservation, loading } = useAppSelector((s) => s.reservations);
  const menuItems = useAppSelector((s) => s.menu.items);
  const [form] = Form.useForm();
  const [selectedDishes, setSelectedDishes] = useState<ReservationItemRequest[]>([]);

  useEffect(() => {
    dispatch(fetchMenuItems());
    return () => {
      dispatch(clearCurrentReservation());
    };
  }, [dispatch]);

  const handleAddDish = useCallback(
    (dishId: string) => {
      if (selectedDishes.find((d) => d.dishId === dishId)) {
        message.warning('ეს კერძი უკვე დამატებულია');
        return;
      }
      setSelectedDishes((prev) => [...prev, { dishId, quantity: 1 }]);
    },
    [selectedDishes]
  );

  const handleSubmit = useCallback(
    async (values: { customerName: string; customerPhone: string; reservationDate: dayjs.Dayjs; reservationTime: dayjs.Dayjs; guestCount: number; tableNumber: number; notes?: string }) => {
      const data: CreateReservationRequest = {
        customerName: values.customerName,
        customerPhone: values.customerPhone,
        reservationDate: values.reservationDate.format('YYYY-MM-DD'),
        reservationTime: values.reservationTime.format('HH:mm'),
        guestCount: values.guestCount,
        tableNumber: values.tableNumber,
        notes: values.notes,
        items: selectedDishes,
      };

      try {
        await dispatch(createReservation(data)).unwrap();
        message.success('რეზერვაცია წარმატებით შეიქმნა!');
        form.resetFields();
        setSelectedDishes([]);
      } catch {
        message.error('რეზერვაციის შექმნა ვერ მოხერხდა');
      }
    },
    [dispatch, form, selectedDishes]
  );

  if (currentReservation) {
    return (
      <div style={{ maxWidth: 500, margin: '0 auto', padding: 16 }}>
        <Result
          status="success"
          title="რეზერვაცია შექმნილია!"
          subTitle={`რეზერვაციის ნომერი: ${currentReservation.reservationNumber}`}
          extra={
            <Button type="primary" onClick={() => dispatch(clearCurrentReservation())}>
              ახალი რეზერვაცია
            </Button>
          }
        />
      </div>
    );
  }

  const dishColumns = [
    {
      title: 'კერძი',
      key: 'name',
      render: (_: unknown, record: ReservationItemRequest) => {
        const dish = menuItems.find((m: MenuItem) => m.id === record.dishId);
        return dish?.nameKa ?? '—';
      },
    },
    {
      title: 'რაოდენობა',
      key: 'qty',
      width: 120,
      render: (_: unknown, record: ReservationItemRequest, idx: number) => (
        <InputNumber
          min={1}
          value={record.quantity}
          onChange={(val) => {
            if (val) {
              setSelectedDishes((prev) => {
                const copy = [...prev];
                copy[idx] = { ...copy[idx], quantity: val };
                return copy;
              });
            }
          }}
        />
      ),
    },
    {
      title: '',
      key: 'action',
      width: 80,
      render: (_: unknown, __: unknown, idx: number) => (
        <Button
          danger
          size="small"
          onClick={() => setSelectedDishes((prev) => prev.filter((_, i) => i !== idx))}
        >
          წაშლა
        </Button>
      ),
    },
  ];

  return (
    <div style={{ maxWidth: 600, margin: '0 auto', padding: 16 }}>
      <h2>რეზერვაცია</h2>
      <Card>
        <Form form={form} layout="vertical" onFinish={handleSubmit}>
          <Form.Item
            label="სახელი"
            name="customerName"
            rules={[{ required: true, message: 'სავალდებულოა' }]}
          >
            <Input placeholder="თქვენი სახელი" />
          </Form.Item>

          <Form.Item
            label="ტელეფონი"
            name="customerPhone"
            rules={[{ required: true, message: 'სავალდებულოა' }]}
          >
            <Input placeholder="5XX XXX XXX" />
          </Form.Item>

          <Form.Item
            label="თარიღი"
            name="reservationDate"
            rules={[{ required: true, message: 'სავალდებულოა' }]}
          >
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item
            label="დრო"
            name="reservationTime"
            rules={[{ required: true, message: 'სავალდებულოა' }]}
          >
            <TimePicker format="HH:mm" style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item
            label="სტუმრების რაოდენობა"
            name="guestCount"
            rules={[{ required: true, message: 'სავალდებულოა' }]}
          >
            <InputNumber min={1} max={50} style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item
            label="მაგიდის ნომერი"
            name="tableNumber"
            rules={[{ required: true, message: 'სავალდებულოა' }]}
          >
            <InputNumber min={1} style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item label="შენიშვნა" name="notes">
            <Input.TextArea rows={2} placeholder="დამატებითი ინფორმაცია" />
          </Form.Item>

          <div style={{ marginBottom: 16 }}>
            <div style={{ marginBottom: 8 }}>
              <strong>წინასწარი შეკვეთა (არასავალდებულო)</strong>
            </div>
            <Select
              placeholder="აირჩიეთ კერძი"
              style={{ width: '100%', marginBottom: 8 }}
              onChange={handleAddDish}
              value={undefined}
              showSearch
              filterOption={(input, option) =>
                (option?.label as string)?.toLowerCase().includes(input.toLowerCase()) ?? false
              }
              options={menuItems.map((m: MenuItem) => ({
                value: m.id,
                label: `${m.nameKa} ${m.price != null ? `- ${m.price}₾` : ''}`,
              }))}
            />
            {selectedDishes.length > 0 && (
              <Table
                dataSource={selectedDishes}
                columns={dishColumns}
                rowKey="dishId"
                pagination={false}
                size="small"
              />
            )}
          </div>

          <Form.Item>
            <Button type="primary" htmlType="submit" loading={loading} block size="large">
              რეზერვაციის შექმნა
            </Button>
          </Form.Item>
        </Form>
      </Card>
    </div>
  );
};

export default ReservationPage;
