import React, { useEffect, useState, useCallback } from 'react';
import { Table, Tag, Button, Select, Space, Input, Popconfirm, message, Skeleton } from 'antd';
import { ReloadOutlined, SearchOutlined, EyeOutlined } from '@ant-design/icons';
import { useAppDispatch, useAppSelector } from '../../store';
import { fetchOrders, updateOrderStatus, cancelOrder } from '../../store/orderSlice';
import { OrderStatus } from '../../types/order';
import type { OrderResponse } from '../../types/order';
import OrderDetailModal from './OrderDetailModal';

const statusColor: Record<string, string> = {
  [OrderStatus.Pending]: 'orange',
  [OrderStatus.Confirmed]: 'blue',
  [OrderStatus.Preparing]: 'purple',
  [OrderStatus.Ready]: 'green',
  [OrderStatus.Delivered]: 'cyan',
  [OrderStatus.Cancelled]: 'red',
};

const statusFlow: Record<string, string[]> = {
  [OrderStatus.Pending]: [OrderStatus.Confirmed, OrderStatus.Cancelled],
  [OrderStatus.Confirmed]: [OrderStatus.Preparing, OrderStatus.Cancelled],
  [OrderStatus.Preparing]: [OrderStatus.Ready],
  [OrderStatus.Ready]: [OrderStatus.Delivered],
};

const OrderManagement: React.FC = () => {
  const dispatch = useAppDispatch();
  const { orders, loading } = useAppSelector((s) => s.orders);
  const [filterStatus, setFilterStatus] = useState<string>('all');
  const [searchText, setSearchText] = useState('');
  const [detailOrderId, setDetailOrderId] = useState<string | null>(null);
  const [updatingId, setUpdatingId] = useState<string | null>(null);

  useEffect(() => {
    dispatch(fetchOrders());
  }, [dispatch]);

  const handleStatusChange = useCallback(
    async (orderId: string, newStatus: string) => {
      setUpdatingId(orderId);
      try {
        await dispatch(updateOrderStatus({ id: orderId, status: newStatus })).unwrap();
        message.success('სტატუსი განახლდა');
      } catch {
        message.error('სტატუსის განახლება ვერ მოხერხდა');
      } finally {
        setUpdatingId(null);
      }
    },
    [dispatch]
  );

  const handleCancel = useCallback(
    async (orderId: string) => {
      try {
        await dispatch(cancelOrder(orderId)).unwrap();
        message.success('შეკვეთა გაუქმდა');
      } catch {
        message.error('შეკვეთის გაუქმება ვერ მოხერხდა');
      }
    },
    [dispatch]
  );

  const filteredOrders = orders.filter((o) => {
    const matchStatus = filterStatus === 'all' || o.status === filterStatus;
    const matchSearch =
      !searchText ||
      o.orderNumber.toLowerCase().includes(searchText.toLowerCase()) ||
      o.customerName.toLowerCase().includes(searchText.toLowerCase());
    return matchStatus && matchSearch;
  });

  const columns = [
    {
      title: '#',
      dataIndex: 'orderNumber',
      key: 'orderNumber',
      width: 100,
    },
    {
      title: 'მომხმარებელი',
      dataIndex: 'customerName',
      key: 'customer',
    },
    {
      title: 'ტიპი',
      dataIndex: 'orderType',
      key: 'type',
      width: 100,
    },
    {
      title: 'თანხა',
      dataIndex: 'totalAmount',
      key: 'amount',
      width: 100,
      render: (v: number) => `${v.toFixed(2)}₾`,
    },
    {
      title: 'სტატუსი',
      dataIndex: 'status',
      key: 'status',
      width: 120,
      render: (status: string) => (
        <Tag color={statusColor[status] ?? 'default'}>{status}</Tag>
      ),
    },
    {
      title: 'თარიღი',
      dataIndex: 'createdAt',
      key: 'date',
      width: 150,
      render: (d: string) => new Date(d).toLocaleString('ka-GE'),
    },
    {
      title: 'მოქმედება',
      key: 'actions',
      width: 250,
      render: (_: unknown, record: OrderResponse) => {
        const nextStatuses = statusFlow[record.status] ?? [];
        return (
          <Space size="small" wrap>
            <Button
              size="small"
              icon={<EyeOutlined />}
              onClick={() => setDetailOrderId(record.id)}
            />
            {nextStatuses.map((ns) =>
              ns === OrderStatus.Cancelled ? (
                <Popconfirm
                  key={ns}
                  title="დარწმუნებული ხართ?"
                  onConfirm={() => handleCancel(record.id)}
                >
                  <Button size="small" danger loading={updatingId === record.id}>
                    გაუქმება
                  </Button>
                </Popconfirm>
              ) : (
                <Button
                  key={ns}
                  size="small"
                  type="primary"
                  loading={updatingId === record.id}
                  onClick={() => handleStatusChange(record.id, ns)}
                >
                  {ns}
                </Button>
              )
            )}
          </Space>
        );
      },
    },
  ];

  return (
    <div>
      <div style={{ display: 'flex', gap: 8, marginBottom: 16, flexWrap: 'wrap' }}>
        <Input
          prefix={<SearchOutlined />}
          placeholder="ძიება..."
          value={searchText}
          onChange={(e) => setSearchText(e.target.value)}
          style={{ width: 200 }}
        />
        <Select
          value={filterStatus}
          onChange={setFilterStatus}
          style={{ width: 150 }}
        >
          <Select.Option value="all">ყველა</Select.Option>
          {Object.values(OrderStatus).map((s) => (
            <Select.Option key={s} value={s}>
              {s}
            </Select.Option>
          ))}
        </Select>
        <Button icon={<ReloadOutlined />} onClick={() => dispatch(fetchOrders())} loading={loading}>
          განახლება
        </Button>
      </div>

      {loading && orders.length === 0 ? (
        <Skeleton active paragraph={{ rows: 8 }} />
      ) : (
        <Table
          dataSource={filteredOrders}
          columns={columns}
          rowKey="id"
          pagination={{ pageSize: 20 }}
          scroll={{ x: 900 }}
          locale={{ emptyText: 'შეკვეთები არ მოიძებნა' }}
        />
      )}

      <OrderDetailModal
        orderId={detailOrderId}
        visible={!!detailOrderId}
        onClose={() => setDetailOrderId(null)}
      />
    </div>
  );
};

export default OrderManagement;
