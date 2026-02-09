import React, { useEffect } from 'react';
import { Form, Input, Button, Card, message, Alert } from 'antd';
import { LockOutlined, MailOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../store';
import { login, clearAuthError } from '../../store/authSlice';

const LoginPage: React.FC = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { token, loading, error } = useAppSelector((s) => s.auth);
  const [form] = Form.useForm();

  useEffect(() => {
    if (token) {
      navigate('/admin');
    }
  }, [token, navigate]);

  useEffect(() => {
    return () => {
      dispatch(clearAuthError());
    };
  }, [dispatch]);

  const handleSubmit = async (values: { email: string; password: string }) => {
    try {
      await dispatch(login(values)).unwrap();
      message.success('წარმატებით შეხვედით');
      navigate('/admin');
    } catch {
      // Error handled by redux
    }
  };

  return (
    <div
      style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: '100vh',
        background: '#f0f2f5',
      }}
    >
      <Card title="ადმინისტრატორის შესვლა" style={{ width: 400 }}>
        {error && (
          <Alert
            message={error}
            type="error"
            showIcon
            closable
            onClose={() => dispatch(clearAuthError())}
            style={{ marginBottom: 16 }}
          />
        )}
        <Form form={form} layout="vertical" onFinish={handleSubmit}>
          <Form.Item
            name="email"
            rules={[
              { required: true, message: 'შეიყვანეთ ელ-ფოსტა' },
              { type: 'email', message: 'არასწორი ფორმატი' },
            ]}
          >
            <Input prefix={<MailOutlined />} placeholder="ელ-ფოსტა" size="large" />
          </Form.Item>

          <Form.Item
            name="password"
            rules={[{ required: true, message: 'შეიყვანეთ პაროლი' }]}
          >
            <Input.Password
              prefix={<LockOutlined />}
              placeholder="პაროლი"
              size="large"
            />
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit" loading={loading} block size="large">
              შესვლა
            </Button>
          </Form.Item>
        </Form>
      </Card>
    </div>
  );
};

export default LoginPage;
