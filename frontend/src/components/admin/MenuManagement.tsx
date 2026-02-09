import React, { useEffect, useState, useCallback } from 'react';
import {
  Table,
  Button,
  Tag,
  Space,
  Popconfirm,
  message,
  Image,
  Skeleton,
  Tabs,
  Empty,
} from 'antd';
import { ReloadOutlined, DeleteOutlined, UndoOutlined } from '@ant-design/icons';
import { useAppDispatch, useAppSelector } from '../../store';
import { fetchMenuItems, fetchCategories } from '../../store/menuSlice';
import apiClient, { API_ENDPOINTS } from '../../config/api';
import type { MenuItem, DishCategory } from '../../types/menu';

const MenuManagement: React.FC = () => {
  const dispatch = useAppDispatch();
  const { items, categories, loading } = useAppSelector((s) => s.menu);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  useEffect(() => {
    dispatch(fetchMenuItems());
    dispatch(fetchCategories());
  }, [dispatch]);

  const handleDelete = useCallback(
    async (id: string) => {
      setDeletingId(id);
      try {
        await apiClient.delete(API_ENDPOINTS.DISH_BY_ID(id));
        message.success('კერძი წაიშალა');
        dispatch(fetchMenuItems());
      } catch {
        message.error('წაშლა ვერ მოხერხდა');
      } finally {
        setDeletingId(null);
      }
    },
    [dispatch]
  );

  const handleRestore = useCallback(
    async (id: string) => {
      try {
        await apiClient.post(API_ENDPOINTS.DISH_RESTORE(id));
        message.success('კერძი აღდგა');
        dispatch(fetchMenuItems());
      } catch {
        message.error('აღდგენა ვერ მოხერხდა');
      }
    },
    [dispatch]
  );

  const handleDeleteCategory = useCallback(
    async (id: string) => {
      try {
        await apiClient.delete(API_ENDPOINTS.DISH_CATEGORY_BY_ID(id));
        message.success('კატეგორია წაიშალა');
        dispatch(fetchCategories());
      } catch {
        message.error('წაშლა ვერ მოხერხდა');
      }
    },
    [dispatch]
  );

  const dishColumns = [
    {
      title: 'სურათი',
      dataIndex: 'imageUrl',
      key: 'image',
      width: 80,
      render: (url: string | undefined) =>
        url ? (
          <Image src={url} width={50} height={50} style={{ objectFit: 'cover', borderRadius: 4 }} />
        ) : (
          '—'
        ),
    },
    { title: 'სახელი', dataIndex: 'nameKa', key: 'name' },
    {
      title: 'ფასი',
      dataIndex: 'price',
      key: 'price',
      width: 80,
      render: (p: number | undefined) => (p != null ? `${p.toFixed(2)}₾` : '—'),
    },
    {
      title: 'კატეგორია',
      dataIndex: 'dishCategoryId',
      key: 'category',
      width: 150,
      render: (catId: string) => {
        const cat = categories.find((c) => c.id === catId);
        return cat?.nameKa ?? '—';
      },
    },
    {
      title: 'მოქმედება',
      key: 'actions',
      width: 150,
      render: (_: unknown, record: MenuItem) => (
        <Space>
          <Popconfirm title="წაშალოთ კერძი?" onConfirm={() => handleDelete(record.id)}>
            <Button
              size="small"
              danger
              icon={<DeleteOutlined />}
              loading={deletingId === record.id}
            />
          </Popconfirm>
          <Button
            size="small"
            icon={<UndoOutlined />}
            onClick={() => handleRestore(record.id)}
            title="აღდგენა"
          />
        </Space>
      ),
    },
  ];

  const categoryColumns = [
    {
      title: 'სურათი',
      dataIndex: 'imageUrl',
      key: 'image',
      width: 80,
      render: (url: string | undefined) =>
        url ? (
          <Image src={url} width={50} height={50} style={{ objectFit: 'cover', borderRadius: 4 }} />
        ) : (
          '—'
        ),
    },
    { title: 'სახელი', dataIndex: 'nameKa', key: 'name' },
    { title: 'ინგლ.', dataIndex: 'nameEn', key: 'nameEn' },
    { title: 'რიგი', dataIndex: 'displayOrder', key: 'order', width: 60 },
    {
      title: 'აქტიური',
      dataIndex: 'isActive',
      key: 'active',
      width: 80,
      render: (v: boolean) => <Tag color={v ? 'green' : 'red'}>{v ? 'კი' : 'არა'}</Tag>,
    },
    {
      title: 'მოქმედება',
      key: 'actions',
      width: 100,
      render: (_: unknown, record: DishCategory) => (
        <Popconfirm title="წაშალოთ კატეგორია?" onConfirm={() => handleDeleteCategory(record.id)}>
          <Button size="small" danger icon={<DeleteOutlined />} />
        </Popconfirm>
      ),
    },
  ];

  return (
    <div>
      <div style={{ marginBottom: 16 }}>
        <Button
          icon={<ReloadOutlined />}
          onClick={() => {
            dispatch(fetchMenuItems());
            dispatch(fetchCategories());
          }}
          loading={loading}
        >
          განახლება
        </Button>
      </div>

      <Tabs
        items={[
          {
            key: 'dishes',
            label: `კერძები (${items.length})`,
            children: loading && items.length === 0 ? (
              <Skeleton active paragraph={{ rows: 6 }} />
            ) : items.length === 0 ? (
              <Empty description="კერძები ვერ მოიძებნა" />
            ) : (
              <Table
                dataSource={items}
                columns={dishColumns}
                rowKey="id"
                pagination={{ pageSize: 20 }}
                scroll={{ x: 600 }}
              />
            ),
          },
          {
            key: 'categories',
            label: `კატეგორიები (${categories.length})`,
            children: categories.length === 0 ? (
              <Empty description="კატეგორიები ვერ მოიძებნა" />
            ) : (
              <Table
                dataSource={categories}
                columns={categoryColumns}
                rowKey="id"
                pagination={false}
              />
            ),
          },
        ]}
      />
    </div>
  );
};

export default MenuManagement;
