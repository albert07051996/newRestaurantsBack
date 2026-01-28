import React, { useState } from 'react';
import { Form, Input, InputNumber, Button, Upload, Switch, message, Select } from 'antd';
import { UploadOutlined } from '@ant-design/icons';
import type { UploadFile, UploadProps } from 'antd';

interface AddDishFormData {
  nameKa: string;
  nameEn: string;
  descriptionKa: string;
  descriptionEn: string;
  price?: number;
  dishCategoryId: string;
  preparationTimeMinutes?: number;
  calories?: number;
  spicyLevel?: number;
  ingredients?: string;
  ingredientsEn?: string;
  volume?: string;
  alcoholContent?: string;
  isVeganDish: boolean;
  comment?: string;
  videoUrl?: string;
}

interface DishResponseDto {
  id: string;
  nameKa: string;
  nameEn: string;
  imageUrl?: string;
  price?: number;
  // ... სხვა ველები
}

const AddDishForm: React.FC = () => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [fileList, setFileList] = useState<UploadFile[]>([]);

  const handleSubmit = async (values: AddDishFormData) => {
    setLoading(true);
    
    try {
      // FormData-ს შექმნა
      const formData = new FormData();
      
      // ყველა ველის დამატება
      formData.append('NameKa', values.nameKa);
      formData.append('NameEn', values.nameEn);
      formData.append('DescriptionKa', values.descriptionKa || '');
      formData.append('DescriptionEn', values.descriptionEn || '');
      
      if (values.price !== undefined) {
        formData.append('Price', values.price.toString());
      }
      
      formData.append('DishCategoryId', values.dishCategoryId);
      
      if (values.preparationTimeMinutes !== undefined) {
        formData.append('PreparationTimeMinutes', values.preparationTimeMinutes.toString());
      }
      
      if (values.calories !== undefined) {
        formData.append('Calories', values.calories.toString());
      }
      
      if (values.spicyLevel !== undefined) {
        formData.append('SpicyLevel', values.spicyLevel.toString());
      }
      
      formData.append('Ingredients', values.ingredients || '');
      formData.append('IngredientsEn', values.ingredientsEn || '');
      formData.append('Volume', values.volume || '');
      formData.append('AlcoholContent', values.alcoholContent || '');
      formData.append('IsVeganDish', values.isVeganDish.toString());
      formData.append('Comment', values.comment || '');
      
      if (values.videoUrl) {
        formData.append('VideoUrl', values.videoUrl);
      }
      
      // სურათის დამატება
      if (fileList.length > 0 && fileList[0].originFileObj) {
        formData.append('ImageFile', fileList[0].originFileObj);
      }

      // API Request
      const response = await fetch('https://your-api.com/api/Dish', {
        method: 'POST',
        body: formData,
        // Headers-ში არ უნდა დავამატოთ 'Content-Type', ავტომატურად დაემატება
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Failed to add dish');
      }

      const result: DishResponseDto = await response.json();
      
      message.success('კერძი წარმატებით დაემატა!');
      console.log('Added dish:', result);
      console.log('Image URL:', result.imageUrl);
      
      // ფორმის გასუფთავება
      form.resetFields();
      setFileList([]);
      
    } catch (error) {
      console.error('Error adding dish:', error);
      message.error(error instanceof Error ? error.message : 'დაფიქსირდა შეცდომა');
    } finally {
      setLoading(false);
    }
  };

  const uploadProps: UploadProps = {
    onRemove: () => {
      setFileList([]);
    },
    beforeUpload: (file) => {
      // ვალიდაცია
      const isImage = file.type.startsWith('image/');
      if (!isImage) {
        message.error('მხოლოდ სურათის ფაილები დაშვებულია!');
        return false;
      }

      const isLt5M = file.size / 1024 / 1024 < 5;
      if (!isLt5M) {
        message.error('სურათი უნდა იყოს 5MB-ზე ნაკლები!');
        return false;
      }

      setFileList([file]);
      return false; // არ გავაგზავნოთ ავტომატურად
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
        rules={[{ required: true, message: 'გთხოვთ შეიყვანოთ სახელი ქართულად' }]}
      >
        <Input placeholder="მაგ: ხაჭაპური" />
      </Form.Item>

      <Form.Item
        label="სახელი (ინგლისური)"
        name="nameEn"
        rules={[{ required: true, message: 'გთხოვთ შეიყვანოთ სახელი ინგლისურად' }]}
      >
        <Input placeholder="e.g: Khachapuri" />
      </Form.Item>

      <Form.Item
        label="აღწერა (ქართული)"
        name="descriptionKa"
      >
        <Input.TextArea rows={3} placeholder="კერძის აღწერა ქართულად" />
      </Form.Item>

      <Form.Item
        label="აღწერა (ინგლისური)"
        name="descriptionEn"
      >
        <Input.TextArea rows={3} placeholder="Dish description in English" />
      </Form.Item>

      <Form.Item
        label="ფასი (₾)"
        name="price"
      >
        <InputNumber
          min={0}
          step={0.01}
          style={{ width: '100%' }}
          placeholder="0.00"
        />
      </Form.Item>

      <Form.Item
        label="კატეგორია"
        name="dishCategoryId"
        rules={[{ required: true, message: 'გთხოვთ აირჩიოთ კატეგორია' }]}
      >
        <Select placeholder="აირჩიეთ კატეგორია">
          {/* დაამატეთ თქვენი კატეგორიები */}
          <Select.Option value="guid-1">მთავარი კერძები</Select.Option>
          <Select.Option value="guid-2">საწყისი კერძები</Select.Option>
          <Select.Option value="guid-3">დესერტები</Select.Option>
        </Select>
      </Form.Item>

      <Form.Item
        label="მომზადების დრო (წუთი)"
        name="preparationTimeMinutes"
      >
        <InputNumber min={0} style={{ width: '100%' }} />
      </Form.Item>

      <Form.Item
        label="კალორია"
        name="calories"
      >
        <InputNumber min={0} style={{ width: '100%' }} />
      </Form.Item>

      <Form.Item
        label="სიცხარის დონე (0-5)"
        name="spicyLevel"
      >
        <InputNumber min={0} max={5} style={{ width: '100%' }} />
      </Form.Item>

      <Form.Item
        label="ინგრედიენტები (ქართული)"
        name="ingredients"
      >
        <Input.TextArea rows={2} placeholder="მაგ: ყველი, კვერცხი, ფქვილი" />
      </Form.Item>

      <Form.Item
        label="ინგრედიენტები (ინგლისური)"
        name="ingredientsEn"
      >
        <Input.TextArea rows={2} placeholder="e.g: Cheese, Egg, Flour" />
      </Form.Item>

      <Form.Item
        label="მოცულობა"
        name="volume"
      >
        <Input placeholder="მაგ: 500მლ, 300გრ" />
      </Form.Item>

      <Form.Item
        label="ალკოჰოლის შემცველობა"
        name="alcoholContent"
      >
        <Input placeholder="მაგ: 5%, 12%" />
      </Form.Item>

      <Form.Item
        label="ვეგანური კერძი"
        name="isVeganDish"
        valuePropName="checked"
      >
        <Switch />
      </Form.Item>

      <Form.Item
        label="კომენტარი"
        name="comment"
      >
        <Input.TextArea rows={2} placeholder="დამატებითი ინფორმაცია" />
      </Form.Item>

      <Form.Item
        label="სურათი"
        extra="მაქსიმუმ 5MB, ფორმატი: JPG, PNG, GIF, WEBP"
      >
        <Upload {...uploadProps}>
          <Button icon={<UploadOutlined />}>სურათის ატვირთვა</Button>
        </Upload>
      </Form.Item>

      <Form.Item
        label="ვიდეოს URL (არასავალდებულო)"
        name="videoUrl"
      >
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
