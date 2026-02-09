import React, { useEffect, useState, useCallback } from 'react';
import { Table, Tag, Button, Select, Space, Input, Popconfirm, message, Skeleton } from 'antd';
import { ReloadOutlined, SearchOutlined } from '@ant-design/icons';
import { useAppDispatch, useAppSelector } from '../../store';
import {
  fetchReservations,
  updateReservationStatus,
  cancelReservation,
} from '../../store/reservationSlice';
import { ReservationStatus } from '../../types/order';
import type { ReservationResponse } from '../../types/order';

const statusColor: Record<string, string> = {
  [ReservationStatus.Pending]: 'orange',
  [ReservationStatus.Confirmed]: 'blue',
  [ReservationStatus.Completed]: 'green',
  [ReservationStatus.Cancelled]: 'red',
};

const statusFlow: Record<string, string[]> = {
  [ReservationStatus.Pending]: [ReservationStatus.Confirmed, ReservationStatus.Cancelled],
  [ReservationStatus.Confirmed]: [ReservationStatus.Completed, ReservationStatus.Cancelled],
};

const ReservationManagement: React.FC = () => {
  const dispatch = useAppDispatch();
  const { reservations, loading } = useAppSelector((s) => s.reservations);
  const [filterStatus, setFilterStatus] = useState<string>('all');
  const [searchText, setSearchText] = useState('');
  const [updatingId, setUpdatingId] = useState<string | null>(null);

  useEffect(() => {
    dispatch(fetchReservations());
  }, [dispatch]);

  const handleStatusChange = useCallback(
    async (id: string, status: string) => {
      setUpdatingId(id);
      try {
        await dispatch(updateReservationStatus({ id, status })).unwrap();
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
    async (id: string) => {
      try {
        await dispatch(cancelReservation(id)).unwrap();
        message.success('რეზერვაცია გაუქმდა');
      } catch {
        message.error('რეზერვაციის გაუქმება ვერ მოხერხდა');
      }
    },
    [dispatch]
  );

  const filtered = reservations.filter((r) => {
    const matchStatus = filterStatus === 'all' || r.status === filterStatus;
    const matchSearch =
      !searchText ||
      r.reservationNumber.toLowerCase().includes(searchText.toLowerCase()) ||
      r.customerName.toLowerCase().includes(searchText.toLowerCase());
    return matchStatus && matchSearch;
  });

  const columns = [
    { title: '#', dataIndex: 'reservationNumber', key: 'num', width: 100 },
    { title: 'სახელი', dataIndex: 'customerName', key: 'name' },
    { title: 'ტელეფონი', dataIndex: 'customerPhone', key: 'phone', width: 130 },
    {
      title: 'თარიღი',
      key: 'date',
      width: 120,
      render: (_: unknown, r: ReservationResponse) =>
        `${new Date(r.reservationDate).toLocaleDateString('ka-GE')} ${r.reservationTime}`,
    },
    { title: 'სტუმრები', dataIndex: 'guestCount', key: 'guests', width: 80 },
    { title: 'მაგიდა', dataIndex: 'tableNumber', key: 'table', width: 80 },
    {
      title: 'სტატუსი',
      dataIndex: 'status',
      key: 'status',
      width: 120,
      render: (s: string) => <Tag color={statusColor[s] ?? 'default'}>{s}</Tag>,
    },
    {
      title: 'მოქმედება',
      key: 'actions',
      width: 200,
      render: (_: unknown, record: ReservationResponse) => {
        const nextStatuses = statusFlow[record.status] ?? [];
        return (
          <Space size="small" wrap>
            {nextStatuses.map((ns) =>
              ns === ReservationStatus.Cancelled ? (
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
        <Select value={filterStatus} onChange={setFilterStatus} style={{ width: 150 }}>
          <Select.Option value="all">ყველა</Select.Option>
          {Object.values(ReservationStatus).map((s) => (
            <Select.Option key={s} value={s}>
              {s}
            </Select.Option>
          ))}
        </Select>
        <Button
          icon={<ReloadOutlined />}
          onClick={() => dispatch(fetchReservations())}
          loading={loading}
        >
          განახლება
        </Button>
      </div>

      {loading && reservations.length === 0 ? (
        <Skeleton active paragraph={{ rows: 8 }} />
      ) : (
        <Table
          dataSource={filtered}
          columns={columns}
          rowKey="id"
          pagination={{ pageSize: 20 }}
          scroll={{ x: 900 }}
          locale={{ emptyText: 'რეზერვაციები არ მოიძებნა' }}
        />
      )}
    </div>
  );
};

export default ReservationManagement;
