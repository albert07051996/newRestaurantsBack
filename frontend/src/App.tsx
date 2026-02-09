import React from 'react';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import { Layout, Menu } from 'antd';
import {
  HomeOutlined,
  ShoppingCartOutlined,
  CalendarOutlined,
  SearchOutlined,
  SettingOutlined,
} from '@ant-design/icons';
import RestaurantMenu from './components/RestaurantMenu';
import ReservationPage from './components/reservation/ReservationPage';
import OrderTracking from './components/orders/OrderTracking';
import LoginPage from './components/auth/LoginPage';
import AdminDashboard from './components/admin/AdminDashboard';

const { Header, Content } = Layout;

const PublicLayout: React.FC<{ children: React.ReactNode }> = ({ children }) => (
  <Layout style={{ minHeight: '100vh' }}>
    <Header
      style={{
        display: 'flex',
        alignItems: 'center',
        background: '#fff',
        borderBottom: '1px solid #f0f0f0',
        padding: '0 16px',
        position: 'sticky',
        top: 0,
        zIndex: 10,
      }}
    >
      <div style={{ fontWeight: 'bold', fontSize: 18, marginRight: 24 }}>
        <Link to="/" style={{ color: '#1890ff' }}>რესტორანი</Link>
      </div>
      <Menu
        mode="horizontal"
        style={{ flex: 1, border: 'none' }}
        items={[
          { key: '/', icon: <HomeOutlined />, label: <Link to="/">მენიუ</Link> },
          { key: '/track', icon: <SearchOutlined />, label: <Link to="/track">შეკვეთის ძიება</Link> },
          { key: '/reservation', icon: <CalendarOutlined />, label: <Link to="/reservation">რეზერვაცია</Link> },
          { key: '/admin/login', icon: <SettingOutlined />, label: <Link to="/admin/login">ადმინი</Link> },
        ]}
      />
    </Header>
    <Content style={{ background: '#f5f5f5' }}>{children}</Content>
  </Layout>
);

const App: React.FC = () => (
  <BrowserRouter>
    <Routes>
      <Route path="/admin/login" element={<LoginPage />} />
      <Route path="/admin/*" element={<AdminDashboard />} />
      <Route
        path="/"
        element={
          <PublicLayout>
            <RestaurantMenu />
          </PublicLayout>
        }
      />
      <Route
        path="/track"
        element={
          <PublicLayout>
            <OrderTracking />
          </PublicLayout>
        }
      />
      <Route
        path="/reservation"
        element={
          <PublicLayout>
            <ReservationPage />
          </PublicLayout>
        }
      />
    </Routes>
  </BrowserRouter>
);

export default App;
