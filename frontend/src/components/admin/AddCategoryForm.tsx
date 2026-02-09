import React, { useState, useCallback } from 'react';
import { Form, Input, InputNumber, Switch, Button, Upload, message } from 'antd';
import { UploadOutlined } from '@ant-design/icons';
import type { UploadFile, UploadProps } from 'antd';
import apiClient, { API_ENDPOINTS } from '../../config/api';
import { useAppDispatch } from '../../store';
import { fetchCategories } from '../../store/menuSlice';

const AddCategoryForm: React.FC = () => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [fileList, setFileList] = useState<UploadFile[]>([]);
  const dispatch = useAppDispatch();

  const handleSubmit = useCallback(
    async (values: Record<string, unknown>) => {
      setLoading(true);
      try {
        const formData = new FormData();
        formData.append('NameKa', String(values.nameKa ?? ''));
        formData.append('NameEn', String(values.nameEn ?? ''));
        formData.append('DescriptionKa', String(values.descriptionKa ?? ''));
        formData.append('DescriptionEn', String(values.descriptionEn ?? ''));
        formData.append('DisplayOrder', String(values.displayOrder ?? 0));
        formData.append('IsActive', String(values.isActive ?? true));

        if (fileList.length > 0 && fileList[0].originFileObj) {
          formData.append('ImageFile', fileList[0].originFileObj);
        }

        await apiClient.post(API_ENDPOINTS.DISH_CATEGORIES, formData, {
          headers: { 'Content-Type': 'multipart/form-data' },
        });

        message.success('კატეგორია წარმატებით დაემატა!');
        form.resetFields();
        setFileList([]);
        dispatch(fetchCategories());
      } catch (error) {
        const msg =
          error instanceof Error ? error.message : 'კატეგორიის დამატება ვერ მოხერხდა';
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
      initialValues={{ isActive: true, displayOrder: 0 }}
    >
      <Form.Item
        label="სახელი (ქართული)"
        name="nameKa"
        rules={[{ required: true, message: 'სავალდებულოა' }]}
      >
        <Input placeholder="მაგ: მთავარი კერძები" />
      </Form.Item>

      <Form.Item
        label="სახელი (ინგლისური)"
        name="nameEn"
        rules={[{ required: true, message: 'სავალდებულოა' }]}
      >
        <Input placeholder="e.g: Main Dishes" />
      </Form.Item>

      <Form.Item label="აღწერა (ქართული)" name="descriptionKa">
        <Input.TextArea rows={2} />
      </Form.Item>

      <Form.Item label="აღწერა (ინგლისური)" name="descriptionEn">
        <Input.TextArea rows={2} />
      </Form.Item>

      <Form.Item label="რიგითობა" name="displayOrder">
        <InputNumber min={0} style={{ width: '100%' }} />
      </Form.Item>

      <Form.Item label="აქტიური" name="isActive" valuePropName="checked">
        <Switch />
      </Form.Item>

      <Form.Item label="სურათი" extra="მაქსიმუმ 5MB">
        <Upload {...uploadProps}>
          <Button icon={<UploadOutlined />}>სურათის ატვირთვა</Button>
        </Upload>
      </Form.Item>

      <Form.Item>
        <Button type="primary" htmlType="submit" loading={loading} block>
          კატეგორიის დამატება
        </Button>
      </Form.Item>
    </Form>
  );
};

export default AddCategoryForm;
