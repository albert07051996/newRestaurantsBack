import React, { useEffect, useState, useCallback } from 'react';
import {
  Card,
  Row,
  Col,
  Button,
  Tag,
  Modal,
  InputNumber,
  Input,
  Badge,
  Skeleton,
  Empty,
  message,
  Tabs,
} from 'antd';
import {
  ShoppingCartOutlined,
  FireOutlined,
  ClockCircleOutlined,
  PlusOutlined,
  MinusOutlined,
  DeleteOutlined,
} from '@ant-design/icons';
import { useAppDispatch, useAppSelector } from '../store';
import { fetchMenuItems, fetchCategories } from '../store/menuSlice';
import { addToCart, removeFromCart, updateQuantity } from '../store/cartSlice';
import type { MenuItem } from '../types/menu';
import CheckoutModal from './orders/CheckoutModal';

const RestaurantMenu: React.FC = () => {
  const dispatch = useAppDispatch();
  const { items, categories, loading } = useAppSelector((s) => s.menu);
  const cartItems = useAppSelector((s) => s.cart.items);

  const [selectedDish, setSelectedDish] = useState<MenuItem | null>(null);
  const [detailVisible, setDetailVisible] = useState(false);
  const [cartVisible, setCartVisible] = useState(false);
  const [checkoutVisible, setCheckoutVisible] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState<string>('all');

  useEffect(() => {
    dispatch(fetchMenuItems());
    dispatch(fetchCategories());
  }, [dispatch]);

  const handleAddToCart = useCallback(
    (dish: MenuItem) => {
      dispatch(addToCart({ dishId: dish.id, quantity: 1 }));
      message.success(`${dish.nameKa} კალათაში დაემატა`);
    },
    [dispatch]
  );

  const handleShowDetail = useCallback((dish: MenuItem) => {
    setSelectedDish(dish);
    setDetailVisible(true);
  }, []);

  const filteredItems =
    selectedCategory === 'all'
      ? items
      : items.filter((i) => i.dishCategoryId === selectedCategory);

  const cartTotal = cartItems.reduce((sum, ci) => {
    const dish = items.find((d) => d.id === ci.dishId);
    return sum + (dish?.price ?? 0) * ci.quantity;
  }, 0);

  const cartCount = cartItems.reduce((sum, ci) => sum + ci.quantity, 0);

  const handleCheckout = useCallback(() => {
    if (cartItems.length === 0) {
      message.warning('კალათა ცარიელია');
      return;
    }
    setCartVisible(false);
    setCheckoutVisible(true);
  }, [cartItems.length]);

  const tabItems = [
    { key: 'all', label: 'ყველა' },
    ...categories
      .filter((c) => c.isActive)
      .sort((a, b) => a.displayOrder - b.displayOrder)
      .map((c) => ({ key: c.id, label: c.nameKa })),
  ];

  return (
    <div style={{ padding: '16px', maxWidth: 1200, margin: '0 auto' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <h1 style={{ margin: 0 }}>მენიუ</h1>
        <Badge count={cartCount} showZero={false}>
          <Button
            type="primary"
            icon={<ShoppingCartOutlined />}
            size="large"
            onClick={() => setCartVisible(true)}
          >
            კალათა {cartTotal > 0 && `(${cartTotal.toFixed(2)}₾)`}
          </Button>
        </Badge>
      </div>

      <Tabs
        activeKey={selectedCategory}
        onChange={setSelectedCategory}
        items={tabItems}
        style={{ marginBottom: 16 }}
      />

      {loading ? (
        <Row gutter={[16, 16]}>
          {[1, 2, 3, 4, 5, 6].map((i) => (
            <Col key={i} xs={24} sm={12} md={8} lg={6}>
              <Card>
                <Skeleton active avatar paragraph={{ rows: 3 }} />
              </Card>
            </Col>
          ))}
        </Row>
      ) : filteredItems.length === 0 ? (
        <Empty description="კერძები ვერ მოიძებნა" />
      ) : (
        <Row gutter={[16, 16]}>
          {filteredItems.map((dish) => (
            <Col key={dish.id} xs={24} sm={12} md={8} lg={6}>
              <Card
                hoverable
                cover={
                  dish.imageUrl ? (
                    <img
                      alt={dish.nameKa}
                      src={dish.imageUrl}
                      loading="lazy"
                      style={{ height: 200, objectFit: 'cover' }}
                    />
                  ) : (
                    <div
                      style={{
                        height: 200,
                        background: '#f0f0f0',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        color: '#999',
                      }}
                    >
                      სურათი არ არის
                    </div>
                  )
                }
                onClick={() => handleShowDetail(dish)}
                actions={[
                  <Button
                    key="add"
                    type="primary"
                    icon={<PlusOutlined />}
                    onClick={(e) => {
                      e.stopPropagation();
                      handleAddToCart(dish);
                    }}
                  >
                    დამატება
                  </Button>,
                ]}
              >
                <Card.Meta
                  title={dish.nameKa}
                  description={
                    <div>
                      <div style={{ color: '#1890ff', fontWeight: 'bold', fontSize: 16 }}>
                        {dish.price != null ? `${dish.price.toFixed(2)}₾` : 'ფასი მითითებული არ არის'}
                      </div>
                      <div style={{ marginTop: 4 }}>
                        {dish.isVeganDish && <Tag color="green">ვეგანური</Tag>}
                        {dish.spicyLevel != null && dish.spicyLevel > 0 && (
                          <Tag color="red" icon={<FireOutlined />}>
                            {dish.spicyLevel}/5
                          </Tag>
                        )}
                        {dish.preparationTimeMinutes != null && (
                          <Tag icon={<ClockCircleOutlined />}>
                            {dish.preparationTimeMinutes} წთ
                          </Tag>
                        )}
                      </div>
                    </div>
                  }
                />
              </Card>
            </Col>
          ))}
        </Row>
      )}

      {/* Dish Detail Modal */}
      <Modal
        open={detailVisible}
        onCancel={() => setDetailVisible(false)}
        footer={[
          <Button key="close" onClick={() => setDetailVisible(false)}>
            დახურვა
          </Button>,
          <Button
            key="add"
            type="primary"
            onClick={() => {
              if (selectedDish) {
                handleAddToCart(selectedDish);
                setDetailVisible(false);
              }
            }}
          >
            კალათაში დამატება
          </Button>,
        ]}
        width={600}
      >
        {selectedDish && (
          <div>
            {selectedDish.imageUrl && (
              <img
                src={selectedDish.imageUrl}
                alt={selectedDish.nameKa}
                style={{ width: '100%', maxHeight: 300, objectFit: 'cover', borderRadius: 8 }}
              />
            )}
            <h2>{selectedDish.nameKa}</h2>
            <p style={{ color: '#666' }}>{selectedDish.nameEn}</p>
            <p>{selectedDish.descriptionKa}</p>
            <div style={{ fontSize: 20, fontWeight: 'bold', color: '#1890ff', margin: '12px 0' }}>
              {selectedDish.price != null ? `${selectedDish.price.toFixed(2)}₾` : '—'}
            </div>
            <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap', marginBottom: 12 }}>
              {selectedDish.isVeganDish && <Tag color="green">ვეგანური</Tag>}
              {selectedDish.calories != null && <Tag>კალორია: {selectedDish.calories}</Tag>}
              {selectedDish.spicyLevel != null && selectedDish.spicyLevel > 0 && (
                <Tag color="red" icon={<FireOutlined />}>
                  სიცხარე: {selectedDish.spicyLevel}/5
                </Tag>
              )}
              {selectedDish.preparationTimeMinutes != null && (
                <Tag icon={<ClockCircleOutlined />}>{selectedDish.preparationTimeMinutes} წთ</Tag>
              )}
            </div>
            {selectedDish.ingredients && (
              <div>
                <strong>ინგრედიენტები:</strong> {selectedDish.ingredients}
              </div>
            )}
            {selectedDish.volume && (
              <div>
                <strong>მოცულობა:</strong> {selectedDish.volume}
              </div>
            )}
            {selectedDish.alcoholContent && (
              <div>
                <strong>ალკოჰოლი:</strong> {selectedDish.alcoholContent}
              </div>
            )}
          </div>
        )}
      </Modal>

      {/* Cart Drawer as Modal */}
      <Modal
        title="კალათა"
        open={cartVisible}
        onCancel={() => setCartVisible(false)}
        footer={[
          <div key="total" style={{ textAlign: 'left', fontSize: 18, fontWeight: 'bold' }}>
            სულ: {cartTotal.toFixed(2)}₾
          </div>,
          <Button key="checkout" type="primary" size="large" onClick={handleCheckout} disabled={cartItems.length === 0}>
            შეკვეთა
          </Button>,
        ]}
      >
        {cartItems.length === 0 ? (
          <Empty description="კალათა ცარიელია" />
        ) : (
          cartItems.map((ci) => {
            const dish = items.find((d) => d.id === ci.dishId);
            if (!dish) return null;
            return (
              <div
                key={ci.dishId}
                style={{
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'center',
                  padding: '8px 0',
                  borderBottom: '1px solid #f0f0f0',
                }}
              >
                <div>
                  <div style={{ fontWeight: 500 }}>{dish.nameKa}</div>
                  <div style={{ color: '#1890ff' }}>
                    {dish.price != null ? `${dish.price.toFixed(2)}₾` : '—'}
                  </div>
                </div>
                <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                  <Button
                    size="small"
                    icon={<MinusOutlined />}
                    onClick={() =>
                      ci.quantity > 1
                        ? dispatch(updateQuantity({ dishId: ci.dishId, quantity: ci.quantity - 1 }))
                        : dispatch(removeFromCart(ci.dishId))
                    }
                  />
                  <InputNumber
                    min={1}
                    value={ci.quantity}
                    size="small"
                    style={{ width: 50 }}
                    onChange={(val) => {
                      if (val) dispatch(updateQuantity({ dishId: ci.dishId, quantity: val }));
                    }}
                  />
                  <Button
                    size="small"
                    icon={<PlusOutlined />}
                    onClick={() =>
                      dispatch(updateQuantity({ dishId: ci.dishId, quantity: ci.quantity + 1 }))
                    }
                  />
                  <Button
                    size="small"
                    danger
                    icon={<DeleteOutlined />}
                    onClick={() => dispatch(removeFromCart(ci.dishId))}
                  />
                </div>
              </div>
            );
          })
        )}
      </Modal>

      {/* Checkout */}
      <CheckoutModal
        visible={checkoutVisible}
        onClose={() => setCheckoutVisible(false)}
        isQrMode={false}
      />
    </div>
  );
};

export default RestaurantMenu;
