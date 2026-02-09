import React, { useState, useEffect, useCallback } from 'react';
import { Form, Input, InputNumber, Button, Upload, Switch, message, Select } from 'antd';
import { UploadOutlined } from '@ant-design/icons';
import type { UploadFile, UploadProps } from 'antd';
import apiClient, { API_ENDPOINTS } from '../../config/api';
import { useAppDispatch, useAppSelector } from '../../store';
import { fetchCategories, fetchMenuItems } from '../../store/menuSlice';

const AddDishForm: React.FC = () => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [fileList, setFileList] = useState<UploadFile[]>([]);
  const dispatch = useAppDispatch();
  const { categories } = useAppSelector((s) => s.menu);

  useEffect(() => {
    dispatch(fetchCategories());
  }, [dispatch]);

  const handleSubmit = useCallback(
    async (values: Record<string, unknown>) => {
      setLoading(true);
      try {
        const formData = new FormData();
        formData.append('NameKa', String(values.nameKa ?? ''));
        formData.append('NameEn', String(values.nameEn ?? ''));
        formData.append('DescriptionKa', String(values.descriptionKa ?? ''));
        formData.append('DescriptionEn', String(values.descriptionEn ?? ''));
        if (values.price !== undefined && values.price !== null) {
          formData.append('Price', String(values.price));
        }
        formData.append('DishCategoryId', String(values.dishCategoryId ?? ''));
        if (values.preparationTimeMinutes !== undefined && values.preparationTimeMinutes !== null) {
          formData.append('PreparationTimeMinutes', String(values.preparationTimeMinutes));
        }
        if (values.calories !== undefined && values.calories !== null) {
          formData.append('Calories', String(values.calories));
        }
        if (values.spicyLevel !== undefined && values.spicyLevel !== null) {
          formData.append('SpicyLevel', String(values.spicyLevel));
        }
        formData.append('Ingredients', String(values.ingredients ?? ''));
        formData.append('IngredientsEn', String(values.ingredientsEn ?? ''));
        formData.append('Volume', String(values.volume ?? ''));
        formData.append('AlcoholContent', String(values.alcoholContent ?? ''));
        formData.append('IsVeganDish', String(values.isVeganDish ?? false));
        formData.append('Comment', String(values.comment ?? ''));
        if (values.videoUrl) {
          formData.append('VideoUrl', String(values.videoUrl));
        }
        if (fileList.length > 0 && fileList[0].originFileObj) {
          formData.append('ImageFile', fileList[0].originFileObj);
        }

        await apiClient.post(API_ENDPOINTS.DISHES, formData, {
          headers: { 'Content-Type': 'multipart/form-data' },
        });

        message.success('კერძი წარმატებით დაემატა!');
        form.resetFields();
        setFileList([]);
        dispatch(fetchMenuItems());
      } catch (error) {
        const msg =
          error instanceof Error ? error.message : 'კერძის დამატება ვერ მოხერხდა';
        message.error(msg);
      } finally {
        setLoading(false);
      }
    },
    [dispatch, fileList, form]
  );

  const uploadProps: UploadProps = {
    onRemove: () => setFileList([]),
    beforeUpload: (file) => {
      const isImage = ['image/jpeg', 'image/png', 'image/webp', 'image/gif'].includes(file.type);
      if (!isImage) {
        message.error('მხოლოდ JPG, PNG, WEBP ან GIF ფორმატი!');
        return false;
      }
      const isLt5M = file.size / 1024 / 1024 < 5;
      if (!isLt5M) {
        message.error('სურათი უნდა იყოს 5MB-ზე ნაკლები!');
        return false;
      }
      setFileList([file]);
      return false;
    },
    fileList,
  };

  return (
    <Form
      form={form}
      layout="vertical"
      onFinish={handleSubmit}
      initialValues={{ isVeganDish: false }}
    >
      <Form.Item
        label="სახელი (ქართული)"
        name="nameKa"
        rules={[{ required: true, message: 'სავალდებულოა' }]}
      >
        <Input placeholder="მაგ: ხაჭაპური" />
      </Form.Item>

      <Form.Item
        label="სახელი (ინგლისური)"
        name="nameEn"
        rules={[{ required: true, message: 'სავალდებულოა' }]}
      >
        <Input placeholder="e.g: Khachapuri" />
      </Form.Item>

      <Form.Item label="აღწერა (ქართული)" name="descriptionKa">
        <Input.TextArea rows={3} />
      </Form.Item>

      <Form.Item label="აღწერა (ინგლისური)" name="descriptionEn">
        <Input.TextArea rows={3} />
      </Form.Item>

      <Form.Item label="ფასი (₾)" name="price">
        <InputNumber min={0} step={0.01} style={{ width: '100%' }} />
      </Form.Item>

      <Form.Item
        label="კატეგორია"
        name="dishCategoryId"
        rules={[{ required: true, message: 'სავალდებულოა' }]}
      >
        <Select placeholder="აირჩიეთ კატეგორია">
          {categories
            .filter((c) => c.isActive)
            .map((c) => (
              <Select.Option key={c.id} value={c.id}>
                {c.nameKa}
              </Select.Option>
            ))}
        </Select>
      </Form.Item>

      <Form.Item label="მომზადების დრო (წუთი)" name="preparationTimeMinutes">
        <InputNumber min={0} style={{ width: '100%' }} />
      </Form.Item>

      <Form.Item label="კალორია" name="calories">
        <InputNumber min={0} style={{ width: '100%' }} />
      </Form.Item>

      <Form.Item label="სიცხარის დონე (0-5)" name="spicyLevel">
        <InputNumber min={0} max={5} style={{ width: '100%' }} />
      </Form.Item>

      <Form.Item label="ინგრედიენტები (ქართული)" name="ingredients">
        <Input.TextArea rows={2} />
      </Form.Item>

      <Form.Item label="ინგრედიენტები (ინგლისური)" name="ingredientsEn">
        <Input.TextArea rows={2} />
      </Form.Item>

      <Form.Item label="მოცულობა" name="volume">
        <Input placeholder="მაგ: 500მლ" />
      </Form.Item>

      <Form.Item label="ალკოჰოლის შემცველობა" name="alcoholContent">
        <Input placeholder="მაგ: 12%" />
      </Form.Item>

      <Form.Item label="ვეგანური კერძი" name="isVeganDish" valuePropName="checked">
        <Switch />
      </Form.Item>

      <Form.Item label="კომენტარი" name="comment">
        <Input.TextArea rows={2} />
      </Form.Item>

      <Form.Item label="სურათი" extra="მაქსიმუმ 5MB, ფორმატი: JPG, PNG, WEBP, GIF">
        <Upload {...uploadProps}>
          <Button icon={<UploadOutlined />}>სურათის ატვირთვა</Button>
        </Upload>
      </Form.Item>

      <Form.Item label="ვიდეოს URL" name="videoUrl">
        <Input placeholder="https://youtube.com/..." />
      </Form.Item>

      <Form.Item>
        <Button type="primary" htmlType="submit" loading={loading} block>
          კერძის დამატება
        </Button>
      </Form.Item>
    </Form>
  );
};

export default AddDishForm;
