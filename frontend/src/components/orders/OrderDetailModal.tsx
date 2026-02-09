import React, { useEffect, useState } from 'react';
import { Modal, Descriptions, Table, Tag, Spin, message } from 'antd';
import apiClient, { API_ENDPOINTS } from '../../config/api';
import type { OrderResponse } from '../../types/order';
import { OrderStatus } from '../../types/order';

interface OrderDetailModalProps {
  orderId: string | null;
  visible: boolean;
  onClose: () => void;
}

const statusColor: Record<string, string> = {
  [OrderStatus.Pending]: 'orange',
  [OrderStatus.Confirmed]: 'blue',
  [OrderStatus.Preparing]: 'purple',
  [OrderStatus.Ready]: 'green',
  [OrderStatus.Delivered]: 'cyan',
  [OrderStatus.Cancelled]: 'red',
};

const OrderDetailModal: React.FC<OrderDetailModalProps> = ({ orderId, visible, onClose }) => {
  const [order, setOrder] = useState<OrderResponse | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!orderId || !visible) return;
    let cancelled = false;

    const loadOrder = async () => {
      setLoading(true);
      try {
        const res = await apiClient.get<OrderResponse>(API_ENDPOINTS.ORDER_BY_ID(orderId));
        if (!cancelled) setOrder(res.data);
      } catch {
        if (!cancelled) message.error('შეკვეთის ჩატვირთვა ვერ მოხერხდა');
      } finally {
        if (!cancelled) setLoading(false);
      }
    };

    loadOrder();
    return () => {
      cancelled = true;
    };
  }, [orderId, visible]);

  const columns = [
    { title: 'კერძი', dataIndex: 'dishNameKa', key: 'name' },
    { title: 'რაოდ.', dataIndex: 'quantity', key: 'qty', width: 70 },
    {
      title: 'ფასი',
      dataIndex: 'unitPrice',
      key: 'price',
      width: 80,
      render: (v: number) => `${v.toFixed(2)}₾`,
    },
    {
      title: 'სულ',
      dataIndex: 'totalPrice',
      key: 'total',
      width: 80,
      render: (v: number) => `${v.toFixed(2)}₾`,
    },
  ];

  return (
    <Modal title="შეკვეთის დეტალები" open={visible} onCancel={onClose} footer={null} width={600}>
      {loading ? (
        <div style={{ textAlign: 'center', padding: 40 }}>
          <Spin size="large" />
        </div>
      ) : order ? (
        <>
          <Descriptions bordered column={1} size="small" style={{ marginBottom: 16 }}>
            <Descriptions.Item label="ნომერი">{order.orderNumber}</Descriptions.Item>
            <Descriptions.Item label="სახელი">{order.customerName}</Descriptions.Item>
            <Descriptions.Item label="ტელეფონი">{order.customerPhone}</Descriptions.Item>
            <Descriptions.Item label="ტიპი">{order.orderType}</Descriptions.Item>
            <Descriptions.Item label="სტატუსი">
              <Tag color={statusColor[order.status] ?? 'default'}>{order.status}</Tag>
            </Descriptions.Item>
            {order.tableNumber != null && (
              <Descriptions.Item label="მაგიდა">#{order.tableNumber}</Descriptions.Item>
            )}
            {order.customerAddress && (
              <Descriptions.Item label="მისამართი">{order.customerAddress}</Descriptions.Item>
            )}
            {order.notes && (
              <Descriptions.Item label="შენიშვნა">{order.notes}</Descriptions.Item>
            )}
            <Descriptions.Item label="თარიღი">
              {new Date(order.createdAt).toLocaleString('ka-GE')}
            </Descriptions.Item>
          </Descriptions>
          <Table
            dataSource={order.items}
            columns={columns}
            rowKey="id"
            pagination={false}
            size="small"
            summary={() => (
              <Table.Summary.Row>
                <Table.Summary.Cell index={0} colSpan={3}>
                  <strong>სულ</strong>
                </Table.Summary.Cell>
                <Table.Summary.Cell index={1}>
                  <strong>{order.totalAmount.toFixed(2)}₾</strong>
                </Table.Summary.Cell>
              </Table.Summary.Row>
            )}
          />
        </>
      ) : null}
    </Modal>
  );
};

export default OrderDetailModal;
