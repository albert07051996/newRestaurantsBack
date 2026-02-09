import React from 'react';
import { Layout, Menu, Button, message } from 'antd';
import {
  MenuOutlined,
  ShoppingCartOutlined,
  CalendarOutlined,
  TableOutlined,
  PlusCircleOutlined,
  AppstoreOutlined,
  LogoutOutlined,
} from '@ant-design/icons';
import { Routes, Route, useNavigate, useLocation, Navigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../store';
import { logout } from '../../store/authSlice';
import OrderManagement from '../orders/OrderManagement';
import ReservationManagement from '../reservation/ReservationManagement';
import TableSessionManagement from '../orders/TableSessionManagement';
import AddDishForm from './AddDishForm';
import AddCategoryForm from './AddCategoryForm';
import MenuManagement from './MenuManagement';

const { Sider, Content, Header } = Layout;

const AdminDashboard: React.FC = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const location = useLocation();
  const { token } = useAppSelector((s) => s.auth);

  if (!token) {
    return <Navigate to="/admin/login" replace />;
  }

  const handleLogout = () => {
    dispatch(logout());
    message.success('გამოხვედით');
    navigate('/admin/login');
  };

  const menuItems = [
    { key: '/admin/orders', icon: <ShoppingCartOutlined />, label: 'შეკვეთები' },
    { key: '/admin/reservations', icon: <CalendarOutlined />, label: 'რეზერვაციები' },
    { key: '/admin/sessions', icon: <TableOutlined />, label: 'მაგიდის სესიები' },
    { key: '/admin/menu', icon: <MenuOutlined />, label: 'მენიუ' },
    { key: '/admin/add-dish', icon: <PlusCircleOutlined />, label: 'კერძის დამატება' },
    { key: '/admin/add-category', icon: <AppstoreOutlined />, label: 'კატეგორია' },
  ];

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider
        breakpoint="lg"
        collapsedWidth="0"
        style={{ position: 'fixed', left: 0, top: 0, bottom: 0, zIndex: 10 }}
      >
        <div style={{ padding: '16px', color: '#fff', textAlign: 'center', fontWeight: 'bold' }}>
          ადმინი
        </div>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[location.pathname]}
          onClick={({ key }) => navigate(key)}
          items={menuItems}
        />
        <div style={{ position: 'absolute', bottom: 16, width: '100%', textAlign: 'center' }}>
          <Button
            type="text"
            icon={<LogoutOutlined />}
            onClick={handleLogout}
            style={{ color: '#fff' }}
          >
            გამოსვლა
          </Button>
        </div>
      </Sider>
      <Layout style={{ marginLeft: 200 }}>
        <Header
          style={{
            background: '#fff',
            padding: '0 24px',
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            boxShadow: '0 1px 4px rgba(0,0,0,0.1)',
          }}
        >
          <h2 style={{ margin: 0 }}>სამართავი პანელი</h2>
        </Header>
        <Content style={{ margin: '24px 16px', padding: 24, background: '#fff', borderRadius: 8 }}>
          <Routes>
            <Route path="orders" element={<OrderManagement />} />
            <Route path="reservations" element={<ReservationManagement />} />
            <Route path="sessions" element={<TableSessionManagement />} />
            <Route path="menu" element={<MenuManagement />} />
            <Route path="add-dish" element={<AddDishForm />} />
            <Route path="add-category" element={<AddCategoryForm />} />
            <Route path="" element={<Navigate to="orders" replace />} />
          </Routes>
        </Content>
      </Layout>
    </Layout>
  );
};

export default AdminDashboard;
