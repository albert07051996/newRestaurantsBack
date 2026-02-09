export interface DishCategory {
  id: string;
  nameKa: string;
  nameEn: string;
  descriptionKa?: string;
  descriptionEn?: string;
  displayOrder: number;
  isActive: boolean;
  imageUrl?: string;
  createdAt: string;
  updatedAt: string;
}

export interface MenuItem {
  id: string;
  nameKa: string;
  nameEn: string;
  descriptionKa: string;
  descriptionEn: string;
  price?: number;
  dishCategoryId: string;
  preparationTimeMinutes?: number;
  calories?: number;
  spicyLevel?: number;
  ingredients: string;
  ingredientsEn: string;
  volume: string;
  alcoholContent: string;
  isVeganDish: boolean;
  comment: string;
  imageUrl?: string;
  videoUrl?: string;
  createdAt: string;
  updatedAt: string;
}
