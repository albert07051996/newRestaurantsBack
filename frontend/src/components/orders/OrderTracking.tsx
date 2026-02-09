import React, { useState, useCallback } from 'react';
import { Input, Button, Card, Tag, Table, Empty, message, Steps } from 'antd';
import { SearchOutlined } from '@ant-design/icons';
import { useAppDispatch, useAppSelector } from '../../store';
import { fetchOrderByNumber, clearCurrentOrder } from '../../store/orderSlice';
import { OrderStatus } from '../../types/order';

const statusStep: Record<string, number> = {
  [OrderStatus.Pending]: 0,
  [OrderStatus.Confirmed]: 1,
  [OrderStatus.Preparing]: 2,
  [OrderStatus.Ready]: 3,
  [OrderStatus.Delivered]: 4,
};

const OrderTracking: React.FC = () => {
  const dispatch = useAppDispatch();
  const { currentOrder, loading, error } = useAppSelector((s) => s.orders);
  const [searchNumber, setSearchNumber] = useState('');

  const handleSearch = useCallback(() => {
    if (!searchNumber.trim()) {
      message.warning('გთხოვთ შეიყვანოთ შეკვეთის ნომერი');
      return;
    }
    dispatch(fetchOrderByNumber(searchNumber.trim()));
  }, [dispatch, searchNumber]);

  return (
    <div style={{ maxWidth: 600, margin: '0 auto', padding: 16 }}>
      <h2>შეკვეთის თვალყურის დევნება</h2>
      <div style={{ display: 'flex', gap: 8, marginBottom: 24 }}>
        <Input
          prefix={<SearchOutlined />}
          placeholder="შეკვეთის ნომერი"
          value={searchNumber}
          onChange={(e) => setSearchNumber(e.target.value)}
          onPressEnter={handleSearch}
          size="large"
        />
        <Button type="primary" size="large" onClick={handleSearch} loading={loading}>
          ძიება
        </Button>
      </div>

      {error && <Empty description={error} />}

      {currentOrder && (
        <Card>
          <h3>შეკვეთა #{currentOrder.orderNumber}</h3>

          {currentOrder.status === OrderStatus.Cancelled ? (
            <Tag color="red" style={{ fontSize: 16, padding: '4px 12px' }}>
              გაუქმებულია
            </Tag>
          ) : (
            <Steps
              current={statusStep[currentOrder.status] ?? 0}
              style={{ marginBottom: 24 }}
              items={[
                { title: 'მიღებულია' },
                { title: 'დადასტურებულია' },
                { title: 'მზადდება' },
                { title: 'მზადაა' },
                { title: 'მიტანილია' },
              ]}
            />
          )}

          <Table
            dataSource={currentOrder.items}
            columns={[
              { title: 'კერძი', dataIndex: 'dishNameKa', key: 'name' },
              { title: 'რაოდ.', dataIndex: 'quantity', key: 'qty', width: 60 },
              {
                title: 'ფასი',
                dataIndex: 'totalPrice',
                key: 'price',
                width: 80,
                render: (v: number) => `${v.toFixed(2)}₾`,
              },
            ]}
            rowKey="id"
            pagination={false}
            size="small"
          />

          <div style={{ textAlign: 'right', marginTop: 12, fontSize: 18, fontWeight: 'bold' }}>
            სულ: {currentOrder.totalAmount.toFixed(2)}₾
          </div>

          <Button
            style={{ marginTop: 12 }}
            onClick={() => {
              dispatch(clearCurrentOrder());
              setSearchNumber('');
            }}
          >
            ახალი ძიება
          </Button>
        </Card>
      )}
    </div>
  );
};

export default OrderTracking;
