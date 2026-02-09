import React, { useEffect, useCallback, useState } from 'react';
import { Table, Tag, Button, Popconfirm, message, Skeleton, Card, Collapse, Space } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';
import { useAppDispatch, useAppSelector } from '../../store';
import {
  fetchTableSessions,
  fetchActiveSessions,
  closeTableSession,
} from '../../store/tableSessionSlice';
import { TableSessionStatus } from '../../types/order';
import type { TableSessionResponse, OrderResponse } from '../../types/order';

const TableSessionManagement: React.FC = () => {
  const dispatch = useAppDispatch();
  const { sessions, loading } = useAppSelector((s) => s.tableSessions);
  const [showActive, setShowActive] = useState(true);
  const [closingId, setClosingId] = useState<string | null>(null);

  useEffect(() => {
    if (showActive) {
      dispatch(fetchActiveSessions());
    } else {
      dispatch(fetchTableSessions());
    }
  }, [dispatch, showActive]);

  const handleClose = useCallback(
    async (id: string) => {
      setClosingId(id);
      try {
        await dispatch(closeTableSession(id)).unwrap();
        message.success('სესია დაიხურა');
      } catch {
        message.error('სესიის დახურვა ვერ მოხერხდა');
      } finally {
        setClosingId(null);
      }
    },
    [dispatch]
  );

  const orderColumns = [
    { title: '#', dataIndex: 'orderNumber', key: 'num', width: 100 },
    { title: 'სტატუსი', dataIndex: 'status', key: 'status', width: 100 },
    {
      title: 'თანხა',
      dataIndex: 'totalAmount',
      key: 'amount',
      width: 100,
      render: (v: number) => `${v.toFixed(2)}₾`,
    },
  ];

  const columns = [
    { title: '#', dataIndex: 'sessionNumber', key: 'num', width: 100 },
    { title: 'მაგიდა', dataIndex: 'tableNumber', key: 'table', width: 80 },
    { title: 'სახელი', dataIndex: 'customerName', key: 'name' },
    {
      title: 'სტატუსი',
      dataIndex: 'status',
      key: 'status',
      width: 100,
      render: (s: string) => (
        <Tag color={s === TableSessionStatus.Active ? 'green' : 'default'}>{s}</Tag>
      ),
    },
    {
      title: 'თანხა',
      dataIndex: 'totalAmount',
      key: 'amount',
      width: 100,
      render: (v: number) => `${v.toFixed(2)}₾`,
    },
    {
      title: 'მოქმედება',
      key: 'actions',
      width: 120,
      render: (_: unknown, record: TableSessionResponse) =>
        record.status === TableSessionStatus.Active ? (
          <Popconfirm title="დარწმუნებული ხართ?" onConfirm={() => handleClose(record.id)}>
            <Button size="small" danger loading={closingId === record.id}>
              დახურვა
            </Button>
          </Popconfirm>
        ) : null,
    },
  ];

  return (
    <div>
      <div style={{ display: 'flex', gap: 8, marginBottom: 16 }}>
        <Space>
          <Button
            type={showActive ? 'primary' : 'default'}
            onClick={() => setShowActive(true)}
          >
            აქტიური
          </Button>
          <Button
            type={!showActive ? 'primary' : 'default'}
            onClick={() => setShowActive(false)}
          >
            ყველა
          </Button>
        </Space>
        <Button
          icon={<ReloadOutlined />}
          onClick={() =>
            showActive ? dispatch(fetchActiveSessions()) : dispatch(fetchTableSessions())
          }
          loading={loading}
        >
          განახლება
        </Button>
      </div>

      {loading && sessions.length === 0 ? (
        <Skeleton active paragraph={{ rows: 6 }} />
      ) : (
        <Table
          dataSource={sessions}
          columns={columns}
          rowKey="id"
          pagination={{ pageSize: 20 }}
          locale={{ emptyText: 'სესიები არ მოიძებნა' }}
          expandable={{
            expandedRowRender: (record: TableSessionResponse) =>
              record.orders.length > 0 ? (
                <Table
                  dataSource={record.orders}
                  columns={orderColumns}
                  rowKey="id"
                  pagination={false}
                  size="small"
                />
              ) : (
                <span>შეკვეთები არ არის</span>
              ),
          }}
        />
      )}
    </div>
  );
};

export default TableSessionManagement;
